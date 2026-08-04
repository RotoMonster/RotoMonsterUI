using System.Collections.Generic;

namespace RotoMonsterUI.Samples
{
    /// <summary>
    /// Worked example of filling MembershipPromoInput with the current Basketball
    /// Monster front page contents, so the shape of each tab is obvious without
    /// reading the model.
    ///
    /// The five panels on the live front page map onto three tab shapes:
    ///
    ///   Intro   - the "helping users win since 2002" paragraph
    ///   List    - Benefits, and New for 2025-26   (Term + Description pairs)
    ///   Blocks  - Questions Answered, and Testimonials   (Title + Body pairs)
    ///
    /// Usage:
    ///     var promo = MembershipPromoSample.Build();
    ///     var html  = new MembershipPromo(promo).Render();
    ///
    /// The two data-driven tabs carry one entry each, as agreed - the real ones
    /// come from your data.
    ///
    /// TWO THINGS WORTH KNOWING, both found while writing this:
    ///
    /// 1. PromoTabBlock.Body is emitted with AppendHtml, so it is RAW HTML and is
    ///    not escaped. PromoTabBlock.Title is escaped. That is fine for copy you
    ///    write yourself, but testimonials are user-submitted - putting a user's
    ///    text straight into Body would let them inject markup into the front
    ///    page. Either encode it before assigning, or the component should use
    ///    .Text() for Body.
    ///
    /// 2. Blocks renders Title ABOVE Body. On the live site the testimonial
    ///    attribution sits BELOW the quote. So a testimonial does not map cleanly
    ///    onto Title + Body. Below it is done by putting the attribution inside
    ///    Body as markup, which works but is a bit of a workaround - a dedicated
    ///    shape, or an optional Footer on the block, would be cleaner if you want
    ///    testimonials to be a first-class case.
    /// </summary>
    public static class MembershipPromoSample
    {
        public static MembershipPromoInput Build()
        {
            return new MembershipPromoInput
            {
                Id = "frontpromo",
                CtaText = "View Membership Options",
                CtaUrl = "/memberships",
                ShowCtaBall = true,
                SelectedTabIndex = 0,
                Tabs = new List<MembershipPromoTab>
                {
                    BuildAboutTab(),
                    BuildBenefitsTab(),
                    BuildNewTab(),
                    BuildQuestionsTab(),
                    BuildTestimonialsTab()
                }
            };
        }

        /// <summary>
        /// Intro shape. Uses LogoUrl, Heading, Subtitle and Body. Body takes HTML,
        /// so paragraph breaks can go in directly.
        /// </summary>
        private static MembershipPromoTab BuildAboutTab()
        {
            return new MembershipPromoTab
            {
                Label = "About",
                Shape = PromoTabShape.Intro,
                Heading = "Helping fantasy managers win since 2002",
                Subtitle = "Fantasy Basketball Projections and Tools",
                Body =
                    "Basketball Monster has been helping users win their Fantasy Basketball leagues since 2002. " +
                    "We generate two types of projections: full-season and daily/short-term. The full-season " +
                    "projections are updated multiple times per day while the daily/short-term are updated " +
                    "immediately once news occurs. This allows you to make quick, informed decisions for your " +
                    "short and long-term planning.<br /><br />" +
                    "Our projections, along with our detailed tools, allow you to effectively and easily manage " +
                    "your fantasy teams. All of the tools are customized to your settings with automated " +
                    "interaction for Yahoo, ESPN, FanTrax, CBS, Sleeper, FleaFlicker, and Ottoneu."
            };
        }

        /// <summary>
        /// List shape. Each item is a Term and a Description - the bold name and
        /// the short explanation after it.
        /// </summary>
        private static MembershipPromoTab BuildBenefitsTab()
        {
            return new MembershipPromoTab
            {
                Label = "Benefits",
                Shape = PromoTabShape.List,
                Heading = "Basketball Monster Benefits",
                Items = new List<PromoTabItem>
                {
                    new PromoTabItem { Term = "Player Projections",  Description = "up-to-date, detailed projections" },
                    new PromoTabItem { Term = "Draft Tracking",      Description = "for mocks and real drafts" },
                    new PromoTabItem { Term = "Customized Projections", Description = "projections based on your settings" },
                    new PromoTabItem { Term = "Dynasty Rankings",    Description = "Josh Lloyd's DURANT rankings" },
                    new PromoTabItem { Term = "Matchup Tools",       Description = "plan/track your h2h matchups" },
                    new PromoTabItem { Term = "Analysis Monster",    Description = "detailed standings analysis" },
                    new PromoTabItem { Term = "Trade Monster",       Description = "find trades/adds to improve your team" },
                    new PromoTabItem { Term = "Box Scores Comments", Description = "analyst commentary after all games" },
                    new PromoTabItem { Term = "Frustration Value",   Description = "how frustrating it's been owning a player" },
                    new PromoTabItem { Term = "Community",           Description = "player comments, message boards, and polls" }
                }
            };
        }

        /// <summary>
        /// Also List shape. Note the last item has no Description - a Term on its
        /// own renders fine, which suits a closing line.
        /// </summary>
        private static MembershipPromoTab BuildNewTab()
        {
            return new MembershipPromoTab
            {
                Label = "New",
                Shape = PromoTabShape.List,
                Heading = "New for 2025-26",
                Items = new List<PromoTabItem>
                {
                    new PromoTabItem { Term = "Projection Ranges",   Description = "our estimated high/low projection for each player" },
                    new PromoTabItem { Term = "Draft Monster",       Description = "upgraded drafting tool" },
                    new PromoTabItem { Term = "Matchup Monster",     Description = "detailed management of h2h matchups" },
                    new PromoTabItem { Term = "Advanced ADP",        Description = "adp adjusted to your settings" },
                    new PromoTabItem { Term = "Advanced Ownership %", Description = "based on your league size/type" },
                    new PromoTabItem { Term = "Ottoneu Support",     Description = "import leagues/rosters" },
                    new PromoTabItem { Term = "Plus we're always adding during the season" }
                }
            };
        }

        /// <summary>
        /// Blocks shape, plus a footer link. This is the data-driven one - one
        /// entry here, the real set comes from your questions data.
        /// </summary>
        private static MembershipPromoTab BuildQuestionsTab()
        {
            return new MembershipPromoTab
            {
                Label = "Questions",
                Shape = PromoTabShape.Blocks,
                Heading = "Questions Answered",
                Blocks = new List<PromoTabBlock>
                {
                    new PromoTabBlock
                    {
                        Title = "How have players performed in the past?",
                        Body =
                            "Our z-score-based <strong>Player Rankings</strong> provide objective rankings " +
                            "customized to your settings. Choose current season, past seasons, date periods, " +
                            "past # of days, or past # of games."
                    }
                },
                FooterLinkText = "View all",
                FooterLinkUrl = "/questions"
            };
        }

        /// <summary>
        /// Also Blocks, and the awkward one - see note 2 in the class summary.
        /// The quote goes in Body with the attribution as trailing markup, because
        /// Title renders above Body and the attribution belongs below.
        ///
        /// Also see note 1: this Body is raw HTML. Real testimonials are
        /// user-submitted, so encode them before they get here.
        /// </summary>
        private static MembershipPromoTab BuildTestimonialsTab()
        {
            return new MembershipPromoTab
            {
                Label = "Reviews",
                Shape = PromoTabShape.Blocks,
                Heading = "User Testimonials",
                Blocks = new List<PromoTabBlock>
                {
                    new PromoTabBlock
                    {
                        Body =
                            "Basketball Monster is an absolute must-have for any serious fantasy basketball " +
                            "coach. I've been playing for 7 years and have won multiple league Championships " +
                            "and have never once finished lower than 3rd place... in any league, all of which " +
                            "are very competitive." +
                            "<br /><br />&mdash; asteams"
                    }
                }
            };
        }
    }
}
