using HtmlTags;

namespace RotoMonsterUI
{
    /// <summary>
    /// The drafting tier as a small filled badge, so the same tier looks the
    /// same on the draft board, the tiers page, or anywhere else it appears.
    ///
    /// The colour comes from --tier-1 through --tier-9 in the shared css, so
    /// nothing here knows what any tier looks like.
    /// </summary>
    public class TierBadge
    {
        /// <summary>
        /// Matt's tiers currently top out at nine. Anything past that falls
        /// back to a neutral badge rather than rendering with no background,
        /// which would read as broken rather than as an extra tier.
        /// </summary>
        public const int MaxTier = 9;

        private readonly int _tier;
        private string _label;
        private string _title;
        private bool _outline;

        public TierBadge(int tier)
        {
            _tier = tier;
        }

        /// <summary>What the badge reads. Defaults to the tier number.</summary>
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

        /// <summary>
        /// Outlined rather than filled. Worth using where a row already has a
        /// lot going on and a solid block would shout.
        /// </summary>
        public TierBadge WithOutline()
        {
            _outline = true;
            return this;
        }

        /// <summary>
        /// The class to put on a player's name so it takes the same colour.
        /// A class rather than an inline style, so a page can turn the whole
        /// thing off with one rule.
        /// </summary>
        public static string NameCssClass(int tier)
        {
            return "tier-name " + TierClass(tier);
        }

        public static string TierClass(int tier)
        {
            if (tier < 1 || tier > MaxTier) return "tier-none";
            return "tier-" + tier;
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