using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class DraftingTiersPlayer
    {
        public DisplayPlayerInput DisplayPlayerInput { get; set; }
        public string NoteHtml { get; set; }

        /// <summary>
        /// Slots this player counts for in the position filter, e.g. PG and SG.
        /// Left empty, the player only shows under Overall.
        /// </summary>
        public List<string> FilterPositions { get; set; } = new List<string>();
    }

    public class DraftingTiersSummary
    {
        public string Position { get; set; }
        public string Html { get; set; }
    }

    public class DraftingTier
    {
        public string TierLabel { get; set; }
        public string NoteText { get; set; }
        public List<DraftingTiersPlayer> Players { get; set; } = new List<DraftingTiersPlayer>();

        /// <summary>
        /// Which tier colour this uses, from the shared palette. Left at zero
        /// the tier renders uncoloured, which suits a page that groups by
        /// something other than Matt's tiers.
        /// </summary>
        public int TierNumber { get; set; }

        public string Position { get; set; }
    }

    public class DraftingTiersInput
    {
        public string Id { get; set; }

        public string IntroHtml { get; set; }

        public List<DraftingTiersSummary> PositionSummaries { get; set; }
            = new List<DraftingTiersSummary>();

        public List<DraftingTier> Tiers { get; set; } = new List<DraftingTier>();

        /// <summary>
        /// The position filter. Empty renders no filter at all, which suits a
        /// page that only has one list.
        /// </summary>
        public List<string> Positions { get; set; } = new List<string>();
        public string AllPositionsText { get; set; } = "Overall";

        public string SelectedPosition { get; set; }

        public bool ShowSearch { get; set; } = true;
        public string SearchPlaceholder { get; set; } = "Find a player...";

        public bool ShowJumpToTier { get; set; } = true;
        public string JumpLabel { get; set; } = "Jump to tier";

        /// <summary>
        /// Colour the tiers and the player names from the shared tier palette.
        /// Off leaves the grouping and the headings, which already say which
        /// tier you are in - the colour is a shortcut, not the information.
        /// </summary>
        public bool ColorByTier { get; set; } = true;

        public bool ShowColorToggle { get; set; } = true;
        public string ColorToggleText { get; set; } = "Color by tier";

        public string EmptyText { get; set; } = "No players match that.";
        public string PlayerWord { get; set; } = "player";
        public string PlayersWord { get; set; } = "players";
    }
}