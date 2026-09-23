using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class PlayerShapeRadarSeries
    {
        public string Name { get; set; }
        public double[] Values { get; set; }
        public string ColorCode { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PlayerShapeRadarInput
    {
        public static readonly string[] DefaultLabels = { "PTS", "3PM", "REB", "AST", "STL", "BLK", "FG%", "FT%" };

        public static readonly string[] DefaultColors = { "#e11d48", "#2563eb", "#16a34a", "#f59e0b", "#9333ea", "#0891b2" };

        public List<string> Labels { get; set; } = DefaultLabels.ToList();
        public List<string> CategoryNames { get; set; } = PlayerShapeService.DefaultCategoryNames.ToList();
        public List<PlayerShapeRadarSeries> Series { get; set; } = new List<PlayerShapeRadarSeries>();
        public int Size { get; set; } = 360;

        public static PlayerShapeRadarInput FromResult(PlayerShapeResult result, int radarMatches)
        {
            var input = new PlayerShapeRadarInput();
            if (result != null && result.CategoryNames != null && result.CategoryNames.Count > 0)
                input.CategoryNames = result.CategoryNames.ToList();
            if (result == null || result.Selected == null || result.SelectedPercentiles == null) return input;

            input.Series.Add(new PlayerShapeRadarSeries
            {
                Name = result.Selected.Name,
                Values = result.SelectedPercentiles,
                ColorCode = DefaultColors[0],
                IsSelected = true
            });

            var count = System.Math.Max(0, System.Math.Min(radarMatches, result.Matches.Count));
            for (var i = 0; i < count; i++)
            {
                input.Series.Add(new PlayerShapeRadarSeries
                {
                    Name = result.Matches[i].Player.Name,
                    Values = result.Matches[i].Percentiles,
                    ColorCode = DefaultColors[(i + 1) % DefaultColors.Length]
                });
            }

            return input;
        }
    }

    public class PlayerShapeMatchesInput
    {
        public List<PlayerShapeMatch> Matches { get; set; } = new List<PlayerShapeMatch>();
        public int ColoredRows { get; set; }
    }
}