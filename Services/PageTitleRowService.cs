using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class PageTitleRowService
    {
        public const string LeagueDropdownName = "pageTitleRowLeague";
        public const string RefreshRostersName = "pageTitleRowRefresh";
        public const string RefreshAllRostersName = "pageTitleRowRefreshAll";
        public const string PlayerSearchId = "pageTitleRowSearch";
        public const string FavoritesId = "pageTitleRowFavorites";
        public const string DarkModeToggleName = "pageTitleRowDarkToggle";

        private readonly PlayerSearchService _playerSearchService = new PlayerSearchService();
        private readonly FavoritesToolbarService _favoritesToolbarService = new FavoritesToolbarService();

        public PageTitleRowResult Process(Dictionary<string, string> formValues)
        {
            var result = new PageTitleRowResult();

            if (formValues == null)
                return result;

            // League selection.
            if (formValues.TryGetValue(LeagueDropdownName, out var leagueValue)
                && !string.IsNullOrEmpty(leagueValue))
            {
                result.SelectedLeagueValue = leagueValue;
            }

            // Refresh rosters
            formValues.TryGetValue("__EVENTTARGET", out var eventTarget);
            if (formValues.ContainsKey(RefreshRostersName) || eventTarget == RefreshRostersName)
                result.RefreshRostersClicked = true;

            if (formValues.ContainsKey(RefreshAllRostersName) || eventTarget == RefreshAllRostersName)
                result.RefreshAllRostersClicked = true;

            // Dark mode toggle 
            if (formValues.ContainsKey(DarkModeToggleName) || eventTarget == DarkModeToggleName)
                result.DarkModeTogglePressed = true;

            // Player search 
            var playerResult = _playerSearchService.Process(PlayerSearchId, formValues);
            result.SelectedPlayerId = playerResult.SelectedPlayerId;

            // Favorites toolbar 
            var favResult = _favoritesToolbarService.Process(FavoritesId, formValues);
            result.AddFavoritePageId = favResult.AddPageId;
            result.HideFavoritePageId = favResult.HidePageId;
            result.ReorderedFavoritePageIds = favResult.ReorderedPageIds;

            return result;
        }
    }
}