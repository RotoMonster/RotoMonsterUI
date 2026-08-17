using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class PricingAddOn
    {
        public string Title { get; set; }
        public string Note { get; set; }
        public bool IsIncluded { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PricingHighlight
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class PricingFeature
    {
        public string Title { get; set; }

        public string TitleHtml { get; set; }

        public string Description { get; set; }

        public string DescriptionHtml { get; set; }

        public string Note { get; set; }
    }

    public class PricingPlan
    {
        public string PlanId { get; set; }
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Price { get; set; }
        public string BadgeText { get; set; }
        public bool IsFeatured { get; set; }
        public PricingAddOn AddOn { get; set; }
        public string PurchaseText { get; set; } = "Purchase";
        public string FeaturesHeading { get; set; }
        public List<PricingFeature> Features { get; set; } = new List<PricingFeature>();
        public PricingHighlight Highlight { get; set; }
    }

    public class PricingTableInput
    {
        public string Id { get; set; }
        public List<PricingPlan> Plans { get; set; } = new List<PricingPlan>();
    }
}