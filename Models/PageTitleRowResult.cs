using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class PageTitleRowResult
    {
        // The league value the user selected in the dropdown, if any.
        public string SelectedLeagueValue { get; set; }

        // True when the user clicked the refresh-rosters button.
        public bool RefreshRostersClicked { get; set; }
        public bool RefreshAllRostersClicked { get; set; }

        // True when the user clicked the dark/light mode toggle. The caller
        // flips their own setting, since they already know the current mode.
        public bool DarkModeTogglePressed { get; set; }

        // The player the user picked from the search box, if any.
        public int? SelectedPlayerId { get; set; }

        // Set when the user added a page from the favorites "Add a page" dropdown.
        public string AddFavoritePageId { get; set; }

        // Set when the user removed a page from the favorites bar.
        public string HideFavoritePageId { get; set; }

        // Set when the user drag-reordered the favorites bar - the full new
        // page order. Caller persists this. Null when no reorder happened.
        public List<string> ReorderedFavoritePageIds { get; set; }
    }
}