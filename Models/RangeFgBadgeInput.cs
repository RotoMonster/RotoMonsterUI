using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class RangeFgBadgeInput
    {
        public string Label { get; set; }

        public List<RangeFgBadgeItem> Ranges { get; set; } = new List<RangeFgBadgeItem>();
    }

    public class RangeFgBadgeItem
    {
        public double? Percent { get; set; }


        public string DisplayText { get; set; }

        public string ColorCode { get; set; }

        public string TextColorCode { get; set; }
    }
}