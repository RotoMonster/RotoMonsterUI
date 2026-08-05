using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class DisplayColumns
    {
        private readonly DisplayColumnsInput _input;

        public DisplayColumns(DisplayColumnsInput input)
        {
            _input = input;
        }

        private string Key(string prefix)
        {
            return prefix + "_" + _input.Id;
        }

        private string ColumnKey(string columnId)
        {
            return "dccol_" + _input.Id + "_" + columnId;
        }

        private string WrapId()
        {
            return "display-columns-" + _input.Id;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("display-columns");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", WrapId());

            var bar = RenderSearchBar();
            if (bar != null) wrap.Append(bar);

            if (!string.IsNullOrEmpty(_input.Message))
                wrap.Append(new HtmlTag("div")
                    .AddClass("display-columns-message")
                    .Text(_input.Message));

            wrap.Append(RenderGroups());

            if (_input.ShowFooter) wrap.Append(RenderFooter());

            return wrap.ToString();
        }

        private HtmlTag RenderSearchBar()
        {
            if (!_input.ShowSearch && !_input.ShowSelectAll) return null;

            var bar = new HtmlTag("div").AddClass("display-columns-bar");

            if (_input.ShowSearch)
            {
                var search = new HtmlTag("div").AddClass("display-columns-search");

                search.AppendHtml(
                    "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke-width=\"2.2\" " +
                    "stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
                    "<circle cx=\"11\" cy=\"11\" r=\"7\"/>" +
                    "<line x1=\"16.5\" y1=\"16.5\" x2=\"21\" y2=\"21\"/></svg>");

                search.Append(new HtmlTag("input")
                    .Attr("type", "text")
                    .Attr("placeholder", _input.SearchPlaceholder ?? "")
                    .Attr("data-dc-search", WrapId()));

                bar.Append(search);
            }

            bar.Append(new HtmlTag("span").AddClass("display-columns-bar-spacer"));

            if (_input.ShowSelectAll)
            {
                bar.Append(BarLink("Select all", "all"));
                bar.Append(BarLink("Clear all", "none"));
            }

            return bar;
        }

        private HtmlTag BarLink(string text, string mode)
        {
            return new HtmlTag("button")
                .Attr("type", "button")
                .AddClass("display-columns-link")
                .Attr("data-dc-setall", mode)
                .Attr("data-dc-target", WrapId())
                .Text(text);
        }

        private HtmlTag RenderGroups()
        {
            var groups = new HtmlTag("div").AddClass("display-columns-groups");

            var count = _input.ColumnCount > 0 ? _input.ColumnCount : 3;
            groups.Attr("style", "column-count:" + count + ";");

            foreach (var group in _input.Groups ?? new List<ColumnGroup>())
            {
                if (group == null) continue;
                groups.Append(RenderGroup(group));
            }

            return groups;
        }

        private HtmlTag RenderGroup(ColumnGroup group)
        {
            var items = group.Items ?? new List<ColumnItem>();
            var selectable = items.Where(i => i != null && !i.IsMembership).ToList();

            var block = new HtmlTag("div").AddClass("display-columns-group");

            var head = new HtmlTag("div").AddClass("display-columns-group-head");

            head.Append(new HtmlTag("span")
                .AddClass("display-columns-group-title")
                .Text(group.Title ?? ""));

            if (_input.ShowGroupCounts)
                head.Append(new HtmlTag("span")
                    .AddClass("display-columns-group-count")
                    .Text(selectable.Count(i => i.IsChecked) + "/" + selectable.Count));

            if (_input.ShowSelectAll && selectable.Count > 0)
                head.Append(new HtmlTag("button")
                    .Attr("type", "button")
                    .AddClass("display-columns-group-all")
                    .Attr("data-dc-groupall", "1")
                    .Text("all"));

            block.Append(head);

            foreach (var item in items)
            {
                if (item == null) continue;
                block.Append(item.IsMembership ? RenderMembershipItem(item) : RenderItem(item));
            }

            return block;
        }

        private HtmlTag RenderItem(ColumnItem item)
        {
            var row = new HtmlTag("label").AddClass("display-columns-item");
            if (item.IsChecked) row.AddClass("display-columns-item--on");
            row.Attr("data-dc-label", (item.Label ?? "").ToLowerInvariant());

            var box = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("name", ColumnKey(item.Id))
                .Attr("id", ColumnKey(item.Id))
                .Attr("value", "1");

            if (item.IsChecked) box.Attr("checked", "checked");

            row.Append(box);

            row.Append(new HtmlTag("span")
                .AddClass("display-columns-item-text")
                .Text(item.Label ?? ""));

            var tip = RenderTooltip(item);
            if (tip != null) row.Append(tip);

            return row;
        }

        private HtmlTag RenderMembershipItem(ColumnItem item)
        {
            var isLink = !string.IsNullOrEmpty(_input.MembershipUrl);

            var row = new HtmlTag(isLink ? "a" : "div")
                .AddClass("display-columns-item")
                .AddClass("display-columns-item--membership");

            if (isLink) row.Attr("href", _input.MembershipUrl);
            row.Attr("data-dc-label", (item.Label ?? "").ToLowerInvariant());

            var lockHtml = new Icon(new IconInput
            {
                Type = IconType.Lock,
                Size = 14,
                Color = "var(--brand-primary)"
            }).Render();

            if (string.IsNullOrEmpty(_input.MembershipTooltip))
            {
                row.AppendHtml(lockHtml);
            }
            else
            {
                var lockTip = new CustomTooltip(
                        "<span class=\"display-columns-lock\">" + lockHtml + "</span>",
                        _input.MembershipTooltip)
                    .WithMaxWidth(200)
                    .WithCentered();

                row.Append(new HtmlTag("span")
                    .AddClass("display-columns-lock-wrap")
                    .AppendHtml(lockTip.Render()));
            }

            row.Append(new HtmlTag("span")
                .AddClass("display-columns-item-text")
                .Text(item.Label ?? ""));

            var tip = RenderTooltip(item);
            if (tip != null) row.Append(tip);

            return row;
        }

        private HtmlTag RenderTooltip(ColumnItem item)
        {
            if (string.IsNullOrEmpty(item.Tooltip)) return null;

            var icon = new Icon(new IconInput
            {
                Type = IconType.Info,
                Size = 14
            }).Render();

            var trigger = "<span class=\"display-columns-tip\">" + icon + "</span>";

            var tooltip = new CustomTooltip(trigger, item.Tooltip)
                .WithMaxWidth(220)
                .WithCentered();

            return new HtmlTag("span")
                .AddClass("display-columns-tip-wrap")
                .AppendHtml(tooltip.Render());
        }

        private HtmlTag RenderFooter()
        {
            var footer = new HtmlTag("div").AddClass("display-columns-footer");

            var total = 0;
            var on = 0;
            var locked = 0;

            foreach (var group in _input.Groups ?? new List<ColumnGroup>())
            {
                foreach (var item in group.Items ?? new List<ColumnItem>())
                {
                    if (item == null) continue;
                    if (item.IsMembership) { locked++; continue; }
                    total++;
                    if (item.IsChecked) on++;
                }
            }

            var summary = on + " of " + total + " columns selected";
            if (locked > 0) summary += " \u00b7 " + locked + " need a membership";

            footer.Append(new HtmlTag("span")
                .AddClass("display-columns-count")
                .Attr("data-dc-count", "1")
                .Text(summary));

            if (_input.ShowMembershipLegend && locked > 0
                && !string.IsNullOrEmpty(_input.MembershipLegendText))
            {
                var legend = new HtmlTag(
                    string.IsNullOrEmpty(_input.MembershipUrl) ? "span" : "a")
                    .AddClass("display-columns-legend");

                if (!string.IsNullOrEmpty(_input.MembershipUrl))
                    legend.Attr("href", _input.MembershipUrl);

                legend.AppendHtml(new Icon(new IconInput
                {
                    Type = IconType.Lock,
                    Size = 12,
                    Color = "var(--brand-primary)"
                }).Render());

                legend.Append(new HtmlTag("span").Text(_input.MembershipLegendText));

                footer.Append(legend);
            }

            if (!string.IsNullOrEmpty(_input.ResetButtonText))
                footer.AppendHtml(new Button(_input.ResetButtonText)
                    .WithStyle(ButtonStyle.Secondary)
                    .WithName(Key("dcreset"))
                    .WithPostBack()
                    .Render());

            if (!string.IsNullOrEmpty(_input.SaveButtonText))
                footer.AppendHtml(new Button(_input.SaveButtonText)
                    .WithStyle(ButtonStyle.Secondary)
                    .WithName(Key("dcsave"))
                    .WithPostBack()
                    .Render());

            if (!string.IsNullOrEmpty(_input.ApplyButtonText))
                footer.AppendHtml(new Button(_input.ApplyButtonText)
                    .WithStyle(ButtonStyle.Primary)
                    .WithName(Key("dcapply"))
                    .WithPostBack()
                    .Render());

            return footer;
        }
    }
}