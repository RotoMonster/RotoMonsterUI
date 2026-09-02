using HtmlTags;

namespace RotoMonsterUI
{
    public class TierBadge
    {
        public const int MaxTier = 9;

        private readonly int _tier;
        private string _label;
        private string _title;
        private bool _outline;

        public TierBadge(int tier)
        {
            _tier = tier;
        }

        public TierBadge WithLabel(string label)
        {
            _label = label;
            return this;
        }

        public TierBadge WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public TierBadge WithOutline()
        {
            _outline = true;
            return this;
        }

        public static string NameCssClass(int tier)
        {
            return "tier-name " + TierClass(tier);
        }

        public static string TierClass(int tier)
        {
            if (tier < 1 || tier > MaxTier) return "tier-none";
            return "tier-" + tier;
        }

        public static string RenderPosition(string abbreviation, string colorCSS)
        {
            if (string.IsNullOrEmpty(abbreviation)) return "";

            var badge = new HtmlTag("span").AddClass("tier-pos-badge").Text(abbreviation);

            if (!string.IsNullOrEmpty(colorCSS))
                badge.Attr("style", "--pos-color:" + CssColor(colorCSS) + ";");

            return badge.ToString();
        }

        public static string RenderCell(string abbreviation, string positionColorCSS,
            int tier, int? rankInTier = null)
        {
            var html = RenderPosition(abbreviation, positionColorCSS);

            if (tier > 0) html += new TierBadge(tier).Render();

            if (rankInTier.HasValue && rankInTier.Value > 0)
                html += new HtmlTag("span")
                    .AddClass("tier-rank")
                    .Text("#" + rankInTier.Value)
                    .ToString();

            return html;
        }

        private static string CssColor(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            var trimmed = value.Trim();
            if (trimmed.Length == 0) return null;

            return trimmed.StartsWith("--") ? "var(" + trimmed + ")" : trimmed;
        }

        public string Render()
        {
            if (_tier < 1) return "";

            var badge = new HtmlTag("span")
                .AddClass("tier-badge")
                .AddClass(TierClass(_tier));

            if (_outline) badge.AddClass("tier-badge--outline");

            if (!string.IsNullOrEmpty(_title)) badge.Attr("title", _title);

            badge.Text(string.IsNullOrEmpty(_label) ? _tier.ToString() : _label);

            return badge.ToString();
        }
    }
}