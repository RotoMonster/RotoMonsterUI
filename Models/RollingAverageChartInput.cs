using System.Collections.Generic;

namespace RotoMonsterUI
{

    public class RollingAverageChartInput
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string XAxisLabel { get; set; } = "Week";
        public string YAxisLabel { get; set; }
        public List<ChartSeries> Series { get; set; } = new List<ChartSeries>();
        public int Height { get; set; } = 320;

        public double? YAxisMin { get; set; }
        public double? YAxisMax { get; set; }
    }
}