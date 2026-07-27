using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class MembershipPromo
    {
        private readonly MembershipPromoInput _input;

        public MembershipPromo(MembershipPromoInput input)
        {
            _input = input;
        }

        private string PanelId(int index)
        {
            return _input.Id + "-panel-" + index;
        }

        public string Render()
        {
            var tabs = _input.Tabs ?? new List<MembershipPromoTab>();
            if (tabs.Count == 0) return "";

            var selected = _input.SelectedTabIndex;
            if (selected < 0 || selected >= tabs.Count) selected = 0;

            var wrap = new HtmlTag("div").AddClass("membership-promo");
            if (!string.IsNullOrEmpty(_input.Id))
                wrap.Attr("id", _input.Id);

            if (!string.IsNullOrEmpty(_input.CtaText))
            {

                var cta = new HtmlTag("a")
                    .AddClass("membership-promo-cta")
                    .Attr("href", string.IsNullOrEmpty(_input.CtaUrl) ? "#" : _input.CtaUrl);

                if (_input.ShowCtaBall)
                    cta.AddClass("membership-promo-cta--ball");

                cta.Append(new HtmlTag("span")
                    .AddClass("membership-promo-cta-text")
                    .Text(_input.CtaText));

                wrap.Append(cta);
            }

            // Tab strip. type="button" matters - this sits inside a form on
            // every real page, and a bare button would submit it.
            var strip = new HtmlTag("div").AddClass("membership-promo-tabs");
            for (int i = 0; i < tabs.Count; i++)
            {
                var button = new HtmlTag("button")
                    .Attr("type", "button")
                    .AddClass("membership-promo-tab")
                    .Attr("data-promo-target", PanelId(i))
                    .Text(tabs[i].Label ?? "");

                if (i == selected)
                    button.AddClass("membership-promo-tab--active");

                strip.Append(button);
            }
            wrap.Append(strip);

            var panels = new HtmlTag("div").AddClass("membership-promo-panels");
            for (int i = 0; i < tabs.Count; i++)
            {
                var panel = RenderPanel(tabs[i]);
                panel.Attr("id", PanelId(i));
                if (i != selected)
                    panel.Attr("style", "display:none;");
                panels.Append(panel);
            }
            wrap.Append(panels);

            return wrap.ToString();
        }

        private HtmlTag RenderPanel(MembershipPromoTab tab)
        {
            var panel = new HtmlTag("div").AddClass("membership-promo-panel");

            switch (tab.Shape)
            {
                case PromoTabShape.List:
                    RenderList(panel, tab);
                    break;
                case PromoTabShape.Blocks:
                    RenderBlocks(panel, tab);
                    break;
                default:
                    RenderIntro(panel, tab);
                    break;
            }

            return panel;
        }

        private void RenderIntro(HtmlTag panel, MembershipPromoTab tab)
        {
            panel.AddClass("membership-promo-panel--intro");

            if (!string.IsNullOrEmpty(tab.LogoUrl))
            {
                panel.Append(new HtmlTag("img")
                    .AddClass("membership-promo-logo")
                    .Attr("src", tab.LogoUrl)
                    .Attr("alt", ""));
            }

            if (!string.IsNullOrEmpty(tab.Heading))
                panel.Append(new HtmlTag("div")
                    .AddClass("membership-promo-since")
                    .Text(tab.Heading));

            if (!string.IsNullOrEmpty(tab.Subtitle))
                panel.Append(new HtmlTag("div")
                    .AddClass("membership-promo-subtitle")
                    .Text(tab.Subtitle));

            if (!string.IsNullOrEmpty(tab.Body))
                panel.Append(new HtmlTag("p")
                    .AddClass("membership-promo-body")
                    .AppendHtml(tab.Body));
        }

        private void RenderList(HtmlTag panel, MembershipPromoTab tab)
        {
            if (!string.IsNullOrEmpty(tab.Heading))
                panel.Append(new HtmlTag("div")
                    .AddClass("membership-promo-heading")
                    .Text(tab.Heading));

            var items = tab.Items ?? new List<PromoTabItem>();
            foreach (var item in items)
            {
                if (item == null) continue;

                var row = new HtmlTag("div").AddClass("membership-promo-item");
                row.AppendHtml(new Icon(new IconInput
                {
                    Type = IconType.Success,
                    Size = 15
                }).Render());

                var text = new HtmlTag("span").AddClass("membership-promo-item-text");
                if (!string.IsNullOrEmpty(item.Term))
                    text.Append(new HtmlTag("strong").Text(item.Term));
                if (!string.IsNullOrEmpty(item.Description))
                    text.AppendHtml(" &mdash; " + item.Description);

                row.Append(text);
                panel.Append(row);
            }
        }

        private void RenderBlocks(HtmlTag panel, MembershipPromoTab tab)
        {
            if (!string.IsNullOrEmpty(tab.Heading))
                panel.Append(new HtmlTag("div")
                    .AddClass("membership-promo-heading")
                    .Text(tab.Heading));

            var blocks = tab.Blocks ?? new List<PromoTabBlock>();
            foreach (var block in blocks)
            {
                if (block == null) continue;

                if (!string.IsNullOrEmpty(block.Title))
                    panel.Append(new HtmlTag("div")
                        .AddClass("membership-promo-block-title")
                        .Text(block.Title));

                if (!string.IsNullOrEmpty(block.Body))
                    panel.Append(new HtmlTag("p")
                        .AddClass("membership-promo-block-body")
                        .AppendHtml(block.Body));
            }

            if (!string.IsNullOrEmpty(tab.FooterLinkText))
            {
                panel.Append(new HtmlTag("a")
                    .AddClass("membership-promo-footer-link")
                    .Attr("href", string.IsNullOrEmpty(tab.FooterLinkUrl) ? "#" : tab.FooterLinkUrl)
                    .Text(tab.FooterLinkText));
            }
        }
    }
}