using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class PlayerShapePlayer
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
        public int Rank { get; set; }
        public int Round { get; set; }
        public double Value { get; set; }
        public double[] Categories { get; set; }
    }

    public class PlayerShapeOptions
    {
        public int MinRoundsLater { get; set; } = 2;
        public int MaxRound { get; set; } = 20;
        public int MatchCount { get; set; } = 5;
        public double[] Weights { get; set; }
    }

    public class PlayerShapeMatch
    {
        public PlayerShapePlayer Player { get; set; }
        public double Match { get; set; }
        public string BestCategory { get; set; }
        public string WeakestCategory { get; set; }
        public double[] Percentiles { get; set; }
    }

    public class PlayerShapeResult
    {
        public PlayerShapePlayer Selected { get; set; }
        public double[] SelectedPercentiles { get; set; }
        public List<string> CategoryNames { get; set; } = new List<string>();
        public List<PlayerShapeMatch> Matches { get; set; } = new List<PlayerShapeMatch>();
    }
}