using System.Collections.Generic;
using System.Globalization;
using HtmlTags;

namespace RotoMonsterUI
{
    public class RangeFgBadge
    {
        private readonly RangeFgBadgeInput _input;

        public RangeFgBadge(RangeFgBadgeInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("range-badge range-badge--fg");

            if (_input == null) return wrap.ToString();

            if (!string.IsNullOrEmpty(_input.Label))
                wrap.Append(new HtmlTag("span").AddClass("range-badge-label").Text(_input.Label));

            var cells = new HtmlTag("div").AddClass("range-badge-cells");

            if (_input.Ranges != null)
            {
                foreach (var range in _input.Ranges)
                {
                    if (range == null) continue;
                    cells.Append(RenderCell(range));
                }
            }

            wrap.Append(cells);
            return wrap.ToString();
        }

        private HtmlTag RenderCell(RangeFgBadgeItem range)
        {
            var cell = new HtmlTag("span").AddClass("range-badge-cell");

            var styleParts = new List<string>();

            if (!string.IsNullOrEmpty(range.ColorCode))
            {
                cell.AddClass("range-badge-cell--shaded");
                styleParts.Add("background:" + NormalizeColor(range.ColorCode));
            }

            if (!string.IsNullOrEmpty(range.TextColorCode))
                styleParts.Add("color:" + NormalizeColor(range.TextColorCode));

            if (styleParts.Count > 0)
                cell.Attr("style", string.Join("; ", styleParts) + ";");

            cell.Append(new HtmlTag("span").AddClass("range-badge-value").Text(FormatPercent(range)));

            return cell;
        }

        private static string FormatPercent(RangeFgBadgeItem range)
        {
            if (!string.IsNullOrEmpty(range.DisplayText)) return range.DisplayText;
            if (!range.Percent.HasValue) return "";
            return range.Percent.Value.ToString("0.0", CultureInfo.InvariantCulture) + "%";
        }

        private static string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return color;
            if (color.StartsWith("var(") || color.StartsWith("#")) return color;
            return "#" + color;
        }
    }
}