using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class SeasonHighLowRow
    {
        public string StatName { get; set; }
        public string HighValue { get; set; }
        public string LowValue { get; set; }
        public bool HighIsGood { get; set; } = true;
    }

    public class SeasonHighLowTableInput
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string StatColumnLabel { get; set; } = "Stat";
        public string HighColumnLabel { get; set; } = "High";
        public string LowColumnLabel { get; set; } = "Low";
        public List<SeasonHighLowRow> Rows { get; set; } = new List<SeasonHighLowRow>();
    }
}