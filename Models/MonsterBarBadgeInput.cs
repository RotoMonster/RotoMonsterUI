using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum MonsterBarGroup
    {
        None,
        Projection,
        LastSeason,
        CurrentSeason
    }

    public enum MonsterBarEmphasis
    {
        Top,
        Ownable,
        Dim
    }

    public class MonsterBarBadgeInput
    {
        public string Label { get; set; }

        public List<MonsterBarBadgeItem> Items { get; set; } = new List<MonsterBarBadgeItem>();
    }

    public class MonsterBarHeaderCell
    {
        public string Label { get; set; }

        public string Description { get; set; }
    }

    public class MonsterBarBadgeItem
    {
        public string Description { get; set; }

        public string GamesText { get; set; }

        public string MeasureText { get; set; }

        public string ColorCode { get; set; }

        public MonsterBarEmphasis Emphasis { get; set; } = MonsterBarEmphasis.Dim;

        public MonsterBarGroup Group { get; set; } = MonsterBarGroup.None;

        public bool IsEmpty { get; set; }
    }
}