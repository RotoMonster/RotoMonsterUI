using HtmlTags;

namespace RotoMonsterUI
{
    public class PageTitleRow
    {
        private readonly PageTitleRowInput _input;

        public PageTitleRow(PageTitleRowInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var output = new System.Text.StringBuilder();

            var row = new HtmlTag("div").AddClass("page-title-row");

            if (!string.IsNullOrEmpty(_input.HeaderHtml))
                row.AppendHtml(_input.HeaderHtml);

            // League selector - plain Dropdown, unchanged component.
            if (_input.Leagues != null && _input.Leagues.Count > 0)
            {
                var leagueDd = new Dropdown(_input.LeagueDropdownLabel)
                    .WithName(PageTitleRowService.LeagueDropdownName)
                    .Wide()
                    .WithSelectedValue(_input.SelectedLeagueValue);

                foreach (var league in _input.Leagues)
                    leagueDd.AddItem(league.Text, league.Value);

                var leagueWrap = new HtmlTag("span")
                    .AddClass("page-title-row-league")
                    .AppendHtml(leagueDd.Render());
                row.Append(leagueWrap);
            }
            else if (!string.IsNullOrEmpty(_input.LeagueDropdownHtml))
            {
                row.AppendHtml(_input.LeagueDropdownHtml);
            }

            if (_input.ShowRefreshRosters)
            {
                var refreshBtn = new IconButton("Refresh Rosters", IconType.RefreshRosters)
                    .WithName(PageTitleRowService.RefreshRostersName)
                    .WithStyle(ButtonStyle.Secondary)
                    .WithIconOnly()
                    .Render();
                row.AppendHtml(refreshBtn);
            }

            if (_input.ShowRefreshAllRosters)
            {
                // Labelled rather than icon-only: at this size no shape reliably reads as
                // "the same thing, but for every league".
                var refreshAllBtn = new IconButton("All", IconType.RefreshRosters)
                    .WithName(PageTitleRowService.RefreshAllRostersName)
                    .WithStyle(ButtonStyle.Secondary)
                    .Render();
                row.AppendHtml(refreshAllBtn);
            }

            if (_input.PlayerSearch != null)
            {
                _input.PlayerSearch.Id = PageTitleRowService.PlayerSearchId;
                var searchWrap = new HtmlTag("span")
                    .AddClass("page-title-row-search")
                    .AppendHtml(new PlayerSearch(_input.PlayerSearch).Render());
                row.Append(searchWrap);
            }
            else if (!string.IsNullOrEmpty(_input.SearchBoxHtml))
            {
                var searchWrap = new HtmlTag("span").AddClass("page-title-row-search").AppendHtml(_input.SearchBoxHtml);
                row.Append(searchWrap);
            }

            if (_input.ShowDarkModeToggle)
            {
                // Shows a moon (switch to dark) when currently light, a sun
                // (switch to light) when currently dark.
                var toggleIcon = _input.IsDarkMode ? IconType.Sun : IconType.Moon;
                var darkToggle = new IconButton(_input.IsDarkMode ? "Switch to light mode" : "Switch to dark mode", toggleIcon)
                    .WithName(PageTitleRowService.DarkModeToggleName)
                    .WithStyle(ButtonStyle.Secondary)
                    .WithIconOnly()
                    .Render();
                row.AppendHtml(darkToggle);
            }

            if (_input.SeasonProgress != null)
            {
                var seasonHtml = new SeasonProgress(_input.SeasonProgress).Render();
                var seasonWrap = new HtmlTag("span").AddClass("page-title-row-season").AppendHtml(seasonHtml);
                row.Append(seasonWrap);
            }

            output.Append(row.ToString());

            if (_input.FavoritesToolbar != null)
            {
                _input.FavoritesToolbar.Id = PageTitleRowService.FavoritesId;
                output.Append(new FavoritesToolbar(_input.FavoritesToolbar).Render());
            }

            return output.ToString();
        }
    }
}