using System.Collections.Generic;
using System.Globalization;
using System.Text;
using HtmlTags;

namespace RotoMonsterUI
{
    public class StatBarChart
    {
        private readonly StatBarChartInput _input;

        public StatBarChart(StatBarChartInput input)
        {
            _input = input;
        }

        private string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string Num(double d)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }

        private string SerializeSpecJson()
        {
            var sb = new StringBuilder("{");
            sb.Append("\"type\":\"bar\",");
            sb.Append($"\"title\":\"{EscapeJson(_input.Title)}\",");
            sb.Append($"\"xAxisLabel\":\"{EscapeJson(_input.XAxisLabel)}\",");
            sb.Append($"\"yAxisLabel\":\"{EscapeJson(_input.YAxisLabel)}\",");

            // Only emitted when set, so the JS can treat "absent" as "auto".
            if (_input.YAxisMin.HasValue)
                sb.Append($"\"yAxisMin\":{Num(_input.YAxisMin.Value)},");
            if (_input.YAxisMax.HasValue)
                sb.Append($"\"yAxisMax\":{Num(_input.YAxisMax.Value)},");
                
            sb.Append("\"series\":[");

            var series = _input.Series ?? new List<ChartSeries>();
            for (int i = 0; i < series.Count; i++)
            {
                var s = series[i];
                if (i > 0) sb.Append(",");
                sb.Append("{");
                sb.Append($"\"name\":\"{EscapeJson(s.Name)}\",");
                sb.Append("\"points\":[");

                var pts = s.Points ?? new List<ChartPoint>();
                for (int j = 0; j < pts.Count; j++)
                {
                    if (j > 0) sb.Append(",");
                    sb.Append($"{{\"x\":\"{EscapeJson(pts[j].X)}\",\"y\":{Num(pts[j].Y)}}}");
                }
                sb.Append("]}");
            }
            sb.Append("]}");
            return sb.ToString();
        }

        public string Render()
        {
            // Chart draws into .bm-chart; JSON sits as a sibling so Google
            // Charts rendering into the div doesn't wipe it out.
            var wrap = new HtmlTag("div").AddClass("bm-chart-wrap");

            var chartDiv = new HtmlTag("div")
                .AddClass("bm-chart bm-bar-chart")
                .Attr("id", _input.Id)
                .Attr("data-chart-type", "bar")
                .Attr("style", $"width:100%;height:{_input.Height}px;");
            wrap.Append(chartDiv);

            wrap.AppendHtml($"<script type=\"application/json\" id=\"{_input.Id}-chartdata\">{SerializeSpecJson()}</script>");

            return wrap.ToString();
        }
    }
}