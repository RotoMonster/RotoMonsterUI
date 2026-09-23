using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HtmlTags;

namespace RotoMonsterUI
{
    public class PlayerShapeRadar
    {
        private readonly PlayerShapeRadarInput _input;

        public PlayerShapeRadar(PlayerShapeRadarInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("player-shape-radar");
            if (_input == null || _input.Labels == null || _input.Labels.Count < 3) return wrap.ToString();

            var size = Math.Max(200, _input.Size);
            var center = size / 2.0;
            var radius = size / 2.0 - 48;
            var count = _input.Labels.Count;

            var svg = new StringBuilder();
            svg.Append("<svg class=\"player-shape-radar-svg\" viewBox=\"0 0 ").Append(size).Append(' ').Append(size)
               .Append("\" width=\"100%\" role=\"img\" aria-label=\"Category shape radar chart\">");

            foreach (var ring in new[] { 0.25, 0.5, 0.75, 1.0 })
            {
                svg.Append("<polygon class=\"player-shape-radar-ring\" points=\"")
                   .Append(Points(Enumerable.Repeat(ring, count).ToArray(), center, radius))
                   .Append("\" />");
            }

            for (var i = 0; i < count; i++)
            {
                var edge = Point(i, count, 1.0, center, radius);
                svg.Append("<line class=\"player-shape-radar-axis\" x1=\"").Append(F(center)).Append("\" y1=\"").Append(F(center))
                   .Append("\" x2=\"").Append(F(edge.Item1)).Append("\" y2=\"").Append(F(edge.Item2)).Append("\" />");

                var label = Point(i, count, 1.0, center, radius + 22);
                var anchor = Math.Abs(label.Item1 - center) < 1 ? "middle" : (label.Item1 > center ? "start" : "end");
                svg.Append("<text class=\"player-shape-radar-label\" x=\"").Append(F(label.Item1)).Append("\" y=\"").Append(F(label.Item2))
                   .Append("\" text-anchor=\"").Append(anchor).Append("\" dominant-baseline=\"middle\">")
                   .Append(Escape(_input.Labels[i])).Append("</text>");
            }

            var series = (_input.Series ?? new List<PlayerShapeRadarSeries>())
                .Where(s => s != null && s.Values != null && s.Values.Length == count)
                .OrderBy(s => s.IsSelected)
                .ToList();

            foreach (var s in series)
            {
                var color = string.IsNullOrEmpty(s.ColorCode) ? "#2563eb" : s.ColorCode;
                svg.Append("<polygon class=\"player-shape-radar-shape")
                   .Append(s.IsSelected ? " player-shape-radar-shape--selected" : "")
                   .Append("\" points=\"").Append(Points(s.Values, center, radius))
                   .Append("\" style=\"stroke:").Append(color).Append(";fill:").Append(color).Append("\"><title>")
                   .Append(Escape(s.Name)).Append("</title></polygon>");
            }

            foreach (var s in series)
            {
                var color = string.IsNullOrEmpty(s.ColorCode) ? "#2563eb" : s.ColorCode;
                for (var i = 0; i < count; i++)
                {
                    var v = Math.Max(0, Math.Min(1, s.Values[i]));
                    var p = Point(i, count, v, center, radius);
                    svg.Append("<circle class=\"player-shape-radar-dot")
                       .Append(s.IsSelected ? " player-shape-radar-dot--selected" : "")
                       .Append("\" cx=\"").Append(F(p.Item1)).Append("\" cy=\"").Append(F(p.Item2))
                       .Append("\" r=\"").Append(s.IsSelected ? "4" : "3.5")
                       .Append("\" style=\"fill:").Append(color).Append("\"><title>")
                       .Append(Escape(s.Name + ", " + CategoryName(i) + ": " + Percentile(v)))
                       .Append("</title></circle>");
                }
            }

            svg.Append("</svg>");
            wrap.AppendHtml(svg.ToString());

            var legend = new HtmlTag("div").AddClass("player-shape-radar-legend");
            foreach (var s in series.OrderByDescending(x => x.IsSelected))
            {
                var item = new HtmlTag("span").AddClass("player-shape-radar-legend-item");
                if (s.IsSelected) item.AddClass("player-shape-radar-legend-item--selected");
                item.Append(new HtmlTag("span").AddClass("player-shape-radar-swatch").Attr("style", "background:" + (s.ColorCode ?? "#2563eb")));
                item.Append(new HtmlTag("span").Text(s.Name));
                legend.Append(item);
            }
            wrap.Append(legend);

            return wrap.ToString();
        }

        private string CategoryName(int index)
        {
            if (_input.CategoryNames != null && index < _input.CategoryNames.Count && !string.IsNullOrEmpty(_input.CategoryNames[index]))
                return _input.CategoryNames[index];
            return _input.Labels[index];
        }

        private static string Percentile(double value)
        {
            var pct = (int)Math.Round(value * 100, MidpointRounding.AwayFromZero);
            if (pct <= 0) return "lowest in the league";
            if (pct >= 100) return "highest in the league";
            return OrdinalHelper.GetOrdinal(pct) + " percentile";
        }

        private static string Points(double[] values, double center, double radius)
        {
            var parts = new List<string>();
            for (var i = 0; i < values.Length; i++)
            {
                var v = Math.Max(0, Math.Min(1, values[i]));
                var p = Point(i, values.Length, v, center, radius);
                parts.Add(F(p.Item1) + "," + F(p.Item2));
            }
            return string.Join(" ", parts);
        }

        private static Tuple<double, double> Point(int index, int count, double value, double center, double radius)
        {
            var angle = -Math.PI / 2 + 2 * Math.PI * index / count;
            return Tuple.Create(center + Math.Cos(angle) * radius * value, center + Math.Sin(angle) * radius * value);
        }

        private static string F(double v)
        {
            return v.ToString("0.#", CultureInfo.InvariantCulture);
        }

        private static string Escape(string s)
        {
            return System.Net.WebUtility.HtmlEncode(s ?? "");
        }
    }
}