using System.Collections.Generic;
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

            var items = new List<MonsterBarBadgeItem>();
            if (_input.Items != null)
            {
                foreach (var item in _input.Items)
                {
                    if (item != null) items.Add(item);
                }
            }

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var isFirst = i == 0;
                var isLast = i == items.Count - 1;

                if (item.IsEmpty)
                {
                    cells.Append(RenderEmptyCell(isFirst, isLast));
                    continue;
                }

                var cellHtml = RenderCell(item, isFirst, isLast).ToString();

                if (!string.IsNullOrEmpty(item.Description))
                    cells.AppendHtml(new CustomTooltip(cellHtml, item.Description).Render());
                else
                    cells.AppendHtml(cellHtml);
            }

            wrap.Append(cells);
            return wrap.ToString();
        }

        public static string RenderHeader(List<string> columnLabels, string label = null)
        {
            var columns = new List<MonsterBarHeaderCell>();

            if (columnLabels != null)
            {
                foreach (var columnLabel in columnLabels)
                    columns.Add(new MonsterBarHeaderCell { Label = columnLabel });
            }

            return RenderHeader(columns, label);
        }

        public static string RenderHeader(List<MonsterBarHeaderCell> columns, string label = null)
        {
            var wrap = new HtmlTag("div").AddClass("range-badge range-badge--monster range-badge--header");

            if (!string.IsNullOrEmpty(label))
                wrap.Append(new HtmlTag("span").AddClass("range-badge-label").Text(label));

            var cells = new HtmlTag("div").AddClass("range-badge-cells");

            if (columns != null)
            {
                foreach (var column in columns)
                {
                    if (column == null) continue;

                    var cell = new HtmlTag("span")
                        .AddClass("range-badge-header-cell")
                        .Text(DecodeLabel(column.Label));

                    if (!string.IsNullOrEmpty(column.Description))
                        cells.AppendHtml(new CustomTooltip(cell.ToString(), column.Description).Render());
                    else
                        cells.Append(cell);
                }
            }

            wrap.Append(cells);
            return wrap.ToString();
        }

        private static HtmlTag RenderCell(MonsterBarBadgeItem item, bool isFirst, bool isLast)
        {
            var cell = new HtmlTag("span").AddClass("range-badge-cell monster-bar-cell");
            cell.AddClass(EmphasisClass(item.Emphasis));

            if (isFirst) cell.AddClass("monster-bar-cell--first");
            if (isLast) cell.AddClass("monster-bar-cell--last");

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

        private static HtmlTag RenderEmptyCell(bool isFirst, bool isLast)
        {
            var cell = new HtmlTag("span")
                .AddClass("range-badge-cell monster-bar-cell monster-bar-cell--empty");

            if (isFirst) cell.AddClass("monster-bar-cell--first");
            if (isLast) cell.AddClass("monster-bar-cell--last");

            return cell.AppendHtml("&nbsp;");
        }

        private static string DecodeLabel(string label)
        {
            if (string.IsNullOrEmpty(label)) return "";
            return System.Net.WebUtility.HtmlDecode(label);
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