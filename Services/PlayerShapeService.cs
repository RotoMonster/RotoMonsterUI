using System;
using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class PlayerShapeService
    {
        public static readonly string[] DefaultCategoryNames =
        {
            "Points", "Threes", "Rebounds", "Assists", "Steals", "Blocks", "Field goal impact", "Free throw impact"
        };

        private readonly string[] _categoryNames;

        public PlayerShapeService(string[] categoryNames = null)
        {
            _categoryNames = categoryNames ?? DefaultCategoryNames;
        }

        public PlayerShapeResult Find(IEnumerable<PlayerShapePlayer> players, PlayerShapePlayer selected, PlayerShapeOptions options = null)
        {
            options = options ?? new PlayerShapeOptions();
            var result = new PlayerShapeResult { Selected = selected, CategoryNames = _categoryNames.ToList() };

            if (players == null || selected == null || !IsUsable(selected)) return result;

            var pool = players.Where(IsUsable).ToList();
            var weights = ResolveWeights(options.Weights);
            var selectedShape = Shape(selected.Categories, weights);

            result.SelectedPercentiles = Percentiles(pool, selected);

            var minRound = selected.Round + options.MinRoundsLater;

            result.Matches = pool
                .Where(p => !ReferenceEquals(p, selected) && p.Name != selected.Name)
                .Where(p => p.Round >= minRound && p.Round <= options.MaxRound)
                .Select(p => new PlayerShapeMatch
                {
                    Player = p,
                    Match = (Cosine(selectedShape, Shape(p.Categories, weights), weights) + 1.0) / 2.0,
                    BestCategory = _categoryNames[IndexOfMax(p.Categories)],
                    WeakestCategory = _categoryNames[IndexOfMin(p.Categories)]
                })
                .OrderByDescending(m => m.Match)
                .Take(Math.Max(0, options.MatchCount))
                .ToList();

            foreach (var match in result.Matches)
                match.Percentiles = Percentiles(pool, match.Player);

            return result;
        }

        private bool IsUsable(PlayerShapePlayer p)
        {
            return p != null && p.Categories != null && p.Categories.Length == _categoryNames.Length;
        }

        private double[] ResolveWeights(double[] weights)
        {
            if (weights != null && weights.Length == _categoryNames.Length) return weights;
            return Enumerable.Repeat(1.0, _categoryNames.Length).ToArray();
        }

        private static double[] Shape(double[] values, double[] weights)
        {
            var totalWeight = weights.Sum();
            var mean = 0.0;
            for (var i = 0; i < values.Length; i++) mean += values[i] * weights[i];
            mean = totalWeight > 0 ? mean / totalWeight : 0;

            var centred = new double[values.Length];
            for (var i = 0; i < values.Length; i++) centred[i] = values[i] - mean;
            return centred;
        }

        private static double Cosine(double[] a, double[] b, double[] weights)
        {
            double dot = 0, aa = 0, bb = 0;
            for (var i = 0; i < a.Length; i++)
            {
                dot += weights[i] * a[i] * b[i];
                aa += weights[i] * a[i] * a[i];
                bb += weights[i] * b[i] * b[i];
            }
            if (aa <= 0 || bb <= 0) return 0;
            return dot / Math.Sqrt(aa * bb);
        }

        private double[] Percentiles(List<PlayerShapePlayer> pool, PlayerShapePlayer player)
        {
            var result = new double[_categoryNames.Length];
            var n = pool.Count;
            if (n < 2) return result;

            for (var i = 0; i < result.Length; i++)
            {
                var value = player.Categories[i];
                var below = pool.Count(p => p.Categories[i] < value);
                result[i] = Math.Round((double)below / (n - 1), 3, MidpointRounding.AwayFromZero);
            }
            return result;
        }

        private static int IndexOfMax(double[] values)
        {
            var best = 0;
            for (var i = 1; i < values.Length; i++) if (values[i] > values[best]) best = i;
            return best;
        }

        private static int IndexOfMin(double[] values)
        {
            var worst = 0;
            for (var i = 1; i < values.Length; i++) if (values[i] < values[worst]) worst = i;
            return worst;
        }
    }
}