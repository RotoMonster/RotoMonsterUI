using HtmlTags;

namespace RotoMonsterUI
{
    public class MonsterBarBadge
    {
        private readonly MonsterBarBadgeInput _input;

        public MonsterBarBadge(MonsterBarBadgeInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("range-badge range-badge--monster");

            if (_input == null) return wrap.ToString();

            if (!string.IsNullOrEmpty(_input.Label))
                wrap.Append(new HtmlTag("span").AddClass("range-badge-label").Text(_input.Label));

            var cells = new HtmlTag("div").AddClass("range-badge-cells");

            if (_input.Items != null)
            {
                foreach (var item in _input.Items)
                {
                    if (item == null) continue;

                    if (item.IsEmpty)
                    {
                        cells.Append(RenderEmptyCell());
                        continue;
                    }

                    var cellHtml = RenderCell(item).ToString();

                    if (!string.IsNullOrEmpty(item.Description))
                        cells.AppendHtml(new CustomTooltip(cellHtml, item.Description).Render());
                    else
                        cells.AppendHtml(cellHtml);
                }
            }

            wrap.Append(cells);
            return wrap.ToString();
        }

        private static HtmlTag RenderCell(MonsterBarBadgeItem item)
        {
            var cell = new HtmlTag("span").AddClass("range-badge-cell monster-bar-cell");
            cell.AddClass(EmphasisClass(item.Emphasis));

            var background = NormalizeColor(item.ColorCode);
            if (!string.IsNullOrEmpty(background))
            {
                cell.AddClass("range-badge-cell--shaded");
                cell.Attr("style", "background:" + background + ";");
            }

            cell.Append(new HtmlTag("span")
                .AddClass("monster-bar-games")
                .Text(item.GamesText ?? ""));

            if (!string.IsNullOrEmpty(item.MeasureText))
                cell.Append(new HtmlTag("span")
                    .AddClass("monster-bar-measure")
                    .Text(item.MeasureText));

            return cell;
        }

        private static HtmlTag RenderEmptyCell()
        {
            return new HtmlTag("span")
                .AddClass("range-badge-cell monster-bar-cell monster-bar-cell--empty")
                .AppendHtml("&nbsp;");
        }

        private static string EmphasisClass(MonsterBarEmphasis emphasis)
        {
            switch (emphasis)
            {
                case MonsterBarEmphasis.Top:
                    return "monster-bar-cell--top";
                case MonsterBarEmphasis.Ownable:
                    return "monster-bar-cell--ownable";
                default:
                    return "monster-bar-cell--dim";
            }
        }

        private static string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return color;
            if (color.StartsWith("var(") || color.StartsWith("#")) return color;
            return "#" + color;
        }
    }
}