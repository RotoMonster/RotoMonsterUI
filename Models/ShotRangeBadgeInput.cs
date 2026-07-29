using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class ShotRangeBadgeInput
    {

        public string Label { get; set; }

        public List<ShotRangeBadgeItem> Ranges { get; set; } = new List<ShotRangeBadgeItem>();
    }

    public class ShotRangeBadgeItem
    {
        public string RangeText { get; set; }

        public double? Percent { get; set; }
        public string DisplayText { get; set; }

        public string ColorCode { get; set; }
    }
}