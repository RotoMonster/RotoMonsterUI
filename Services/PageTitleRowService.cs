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

    formValues.TryGetValue("__EVENTTARGET", out var eventTarget);

    if (eventTarget == LeagueDropdownName
        && formValues.TryGetValue(LeagueDropdownName, out var leagueValue)
        && !string.IsNullOrEmpty(leagueValue))
    {
        result.SelectedLeagueValue = leagueValue;
    }

    if (formValues.ContainsKey(RefreshRostersName) || eventTarget == RefreshRostersName)
        result.RefreshRostersClicked = true;

    if (formValues.ContainsKey(RefreshAllRostersName) || eventTarget == RefreshAllRostersName)
        result.RefreshAllRostersClicked = true;

    if (formValues.ContainsKey(DarkModeToggleName) || eventTarget == DarkModeToggleName)
        result.DarkModeTogglePressed = true;

    var playerResult = _playerSearchService.Process(PlayerSearchId, formValues);
    result.SelectedPlayerId = playerResult.SelectedPlayerId;

    var favResult = _favoritesToolbarService.Process(FavoritesId, formValues);
    result.AddFavoritePageId = favResult.AddPageId;
    result.HideFavoritePageId = favResult.HidePageId;
    result.ReorderedFavoritePageIds = favResult.ReorderedPageIds;

    return result;
}
    }
}