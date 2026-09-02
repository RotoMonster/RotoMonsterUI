using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class DraftingTiersPlayer
    {
        public DisplayPlayerInput DisplayPlayerInput { get; set; }
        public string NoteHtml { get; set; }
        public List<string> FilterPositions { get; set; } = new List<string>();
    }

    public class DraftingTier
    {
        public string TierLabel { get; set; }
        public string NoteText { get; set; }
        public List<DraftingTiersPlayer> Players { get; set; } = new List<DraftingTiersPlayer>();

        public int TierNumber { get; set; }
    }

    public class DraftingTiersInput
    {
        public string Id { get; set; }

        public string IntroHtml { get; set; }

        public List<DraftingTier> Tiers { get; set; } = new List<DraftingTier>();
        public List<string> Positions { get; set; } = new List<string>();
        public string AllPositionsText { get; set; } = "Overall";

        public bool ShowSearch { get; set; } = true;
        public string SearchPlaceholder { get; set; } = "Find a player...";

        public bool ShowJumpToTier { get; set; } = true;
        public string JumpLabel { get; set; } = "Jump to tier";

        public bool ColorByTier { get; set; } = true;

        public bool ShowColorToggle { get; set; } = true;
        public string ColorToggleText { get; set; } = "Color by tier";

        public string EmptyText { get; set; } = "No players match that.";
        public string PlayerWord { get; set; } = "player";
        public string PlayersWord { get; set; } = "players";
    }
}