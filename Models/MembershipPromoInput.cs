using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum PromoTabShape
    {
        Intro,
        List,
        Blocks
    }

    public class PromoTabItem
    {
        public string Term { get; set; }
        public string Description { get; set; }
    }

    public class PromoTabBlock
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class MembershipPromoTab
    {
        public string Label { get; set; }
        public PromoTabShape Shape { get; set; }
        public string Heading { get; set; }

        public string LogoUrl { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }

        public List<PromoTabItem> Items { get; set; } = new List<PromoTabItem>();

        public List<PromoTabBlock> Blocks { get; set; } = new List<PromoTabBlock>();
        public string FooterLinkText { get; set; }
        public string FooterLinkUrl { get; set; }
    }

    public class MembershipPromoInput
    {
        public string Id { get; set; }
        public string CtaText { get; set; } = "View Membership Options";
        public string CtaUrl { get; set; }
        public bool ShowCtaBall { get; set; }
        public int SelectedTabIndex { get; set; } = 0;
        public List<MembershipPromoTab> Tabs { get; set; } = new List<MembershipPromoTab>();
    }
}