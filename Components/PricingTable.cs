using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class PricingTable
    {
        private readonly PricingTableInput _input;

        public PricingTable(PricingTableInput input)
        {
            _input = input;
        }

        public static string BuyName(string tableId, string planId)
        {
            return "pricingbuy_" + tableId + "_" + planId;
        }

        public static string AddOnName(string tableId, string planId)
        {
            return "pricingaddon_" + tableId + "_" + planId;
        }

        public string Render()
        {
            var plans = _input.Plans ?? new List<PricingPlan>();
            if (plans.Count == 0) return "";

            var wrap = new HtmlTag("div").AddClass("pricing-table");
            if (!string.IsNullOrEmpty(_input.Id))
                wrap.Attr("id", _input.Id);

            foreach (var plan in plans)
            {
                if (plan == null) continue;
                wrap.Append(RenderPlan(plan));
            }

            return wrap.ToString();
        }

        private HtmlTag RenderPlan(PricingPlan plan)
        {
            var card = new HtmlTag("div").AddClass("pricing-plan");
            if (plan.IsFeatured)
                card.AddClass("pricing-plan--featured");

            if (!string.IsNullOrEmpty(plan.BadgeText))
                card.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-badge")
                    .Text(plan.BadgeText));

            if (!string.IsNullOrEmpty(plan.Name))
                card.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-name")
                    .Text(plan.Name));

            if (!string.IsNullOrEmpty(plan.Subtitle))
                card.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-subtitle")
                    .Text(plan.Subtitle));

            if (!string.IsNullOrEmpty(plan.Price))
                card.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-price")
                    .Text(plan.Price));

            if (plan.AddOn != null)
                card.Append(RenderAddOn(plan));

            var buy = new HtmlTag("button")
                .Attr("type", "submit")
                .AddClass("pricing-plan-buy")
                .Attr("name", BuyName(_input.Id, plan.PlanId))
                .Text(string.IsNullOrEmpty(plan.PurchaseText) ? "Purchase" : plan.PurchaseText);
            card.Append(buy);

            if (!string.IsNullOrEmpty(plan.FeaturesHeading))
                card.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-features-heading")
                    .Text(plan.FeaturesHeading));

            var features = plan.Features ?? new List<PricingFeature>();
            foreach (var feature in features)
            {
                if (feature == null) continue;
                card.Append(RenderFeature(feature));
            }

            if (plan.Highlight != null)
                card.Append(RenderHighlight(plan.Highlight));

            return card;
        }

        private HtmlTag RenderFeature(PricingFeature feature)
        {
            var box = new HtmlTag("div").AddClass("pricing-plan-feature");

            if (!string.IsNullOrEmpty(feature.TitleHtml))
                box.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-feature-title")
                    .AppendHtml(feature.TitleHtml));
            else if (!string.IsNullOrEmpty(feature.Title))
                box.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-feature-title")
                    .Text(feature.Title));

            if (!string.IsNullOrEmpty(feature.DescriptionHtml))
                box.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-feature-description")
                    .AppendHtml(feature.DescriptionHtml));
            else if (!string.IsNullOrEmpty(feature.Description))
                box.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-feature-description")
                    .Text(feature.Description));

            if (!string.IsNullOrEmpty(feature.Note))
                box.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-feature-note")
                    .Text(feature.Note));

            return box;
        }

        private HtmlTag RenderAddOn(PricingPlan plan)
        {
            var row = new HtmlTag("div").AddClass("pricing-plan-addon");
            var addOn = plan.AddOn;

            if (addOn.IsIncluded)
            {
                row.AddClass("pricing-plan-addon--included");
                row.AppendHtml(new Icon(new IconInput
                {
                    Type = IconType.Success,
                    Size = 16
                }).Render());
            }
            else
            {
                var name = AddOnName(_input.Id, plan.PlanId);
                var check = new HtmlTag("input")
                    .Attr("type", "checkbox")
                    .Attr("name", name)
                    .Attr("id", name);

                if (addOn.IsSelected)
                    check.Attr("checked", "checked");

                row.Append(check);
            }

            var text = new HtmlTag("div").AddClass("pricing-plan-addon-text");

            if (!string.IsNullOrEmpty(addOn.Title))
                text.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-addon-title")
                    .Text(addOn.Title));

            if (!string.IsNullOrEmpty(addOn.Note))
                text.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-addon-note")
                    .Text(addOn.Note));

            row.Append(text);

            return row;
        }

        private HtmlTag RenderHighlight(PricingHighlight highlight)
        {
            var box = new HtmlTag("div").AddClass("pricing-plan-highlight");

            if (!string.IsNullOrEmpty(highlight.Title))
                box.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-highlight-title")
                    .Text(highlight.Title));

            if (!string.IsNullOrEmpty(highlight.Body))
                box.Append(new HtmlTag("div")
                    .AddClass("pricing-plan-highlight-body")
                    .Text(highlight.Body));

            return box;
        }
    }
}