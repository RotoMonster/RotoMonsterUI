using System.Globalization;
using HtmlTags;

namespace RotoMonsterUI
{

    public class ShotRangeBadge
    {
        private readonly ShotRangeBadgeInput _input;

        public ShotRangeBadge(ShotRangeBadgeInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("range-badge range-badge--shots");

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

        private HtmlTag RenderCell(ShotRangeBadgeItem range)
        {
            var cell = new HtmlTag("span").AddClass("range-badge-cell");

            var background = ResolveColor(range);
            if (!string.IsNullOrEmpty(background))
            {
                cell.AddClass("range-badge-cell--shaded");
                cell.Attr("style", "background:" + background + ";");
            }

            if (!string.IsNullOrEmpty(range.RangeText))
                cell.Append(new HtmlTag("span").AddClass("range-badge-distance").Text(range.RangeText));

            cell.Append(new HtmlTag("span").AddClass("range-badge-value").Text(FormatPercent(range)));

            return cell;
        }

        private string ResolveColor(ShotRangeBadgeItem range)
        {
            if (!string.IsNullOrEmpty(range.ColorCode))
                return NormalizeColor(range.ColorCode);

            if (!range.Percent.HasValue)
                return null;

            return "#" + ColorHelper.GetCyanColorCode((float)range.Percent.Value, 0f, 100f, true);
        }

        private static string FormatPercent(ShotRangeBadgeItem range)
        {
            if (!string.IsNullOrEmpty(range.DisplayText)) return range.DisplayText;
            if (!range.Percent.HasValue) return "";
            return range.Percent.Value.ToString("0", CultureInfo.InvariantCulture) + "%";
        }

        private static string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return color;
            if (color.StartsWith("var(") || color.StartsWith("#")) return color;
            return "#" + color;
        }
    }
}