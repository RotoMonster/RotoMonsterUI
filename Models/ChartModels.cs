using System.Collections.Generic;

namespace RotoMonsterUI
{

    public class ChartPoint
    {
        public string X { get; set; }
        public double Y { get; set; }
    }

    public class ChartSeries
    {
        public string Name { get; set; }
        public List<ChartPoint> Points { get; set; } = new List<ChartPoint>();

        public string Color { get; set; }
    }
}