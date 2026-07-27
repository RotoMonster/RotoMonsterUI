using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class SeasonHighLowTable
    {
        private readonly SeasonHighLowTableInput _input;

        public SeasonHighLowTable(SeasonHighLowTableInput input)
        {
            _input = input;
        }

        // Empty value renders a dash rather than a blank cell, so a stat with
        // no recorded value still lines up with the rows around it.
        private HtmlTag ValueCell(string cssClass, string value)
        {
            var td = new HtmlTag("td").AddClass(cssClass);

            td.Append(new HtmlTag("span")
                .AddClass("shl-val")
                .Text(string.IsNullOrEmpty(value) ? "-" : value));

            return td;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("season-highlow");
            if (!string.IsNullOrEmpty(_input.Id))
                wrap.Attr("id", _input.Id);

            if (!string.IsNullOrEmpty(_input.Title))
                wrap.Append(new HtmlTag("div")
                    .AddClass("season-highlow-title")
                    .Text(_input.Title));

            var table = new HtmlTag("table").AddClass("season-highlow-table");

            var thead = new HtmlTag("thead");
            var headRow = new HtmlTag("tr");
            headRow.Append(new HtmlTag("th").Text(_input.StatColumnLabel));
            headRow.Append(new HtmlTag("th").Text(_input.HighColumnLabel));
            headRow.Append(new HtmlTag("th").Text(_input.LowColumnLabel));
            thead.Append(headRow);
            table.Append(thead);

            var tbody = new HtmlTag("tbody");
            var rows = _input.Rows ?? new List<SeasonHighLowRow>();
            foreach (var row in rows)
            {
                var tr = new HtmlTag("tr");


                if (!row.HighIsGood)
                    tr.AddClass("shl-flip");

                tr.Append(new HtmlTag("td").AddClass("shl-stat").Text(row.StatName ?? ""));
                tr.Append(ValueCell("shl-high", row.HighValue));
                tr.Append(ValueCell("shl-low", row.LowValue));
                tbody.Append(tr);
            }
            table.Append(tbody);
            wrap.Append(table);

            return wrap.ToString();
        }
    }
}