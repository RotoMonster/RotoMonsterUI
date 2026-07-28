using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class DisplayGames
    {
        private List<GameInput> _games;
        private string _id;
        private bool _showSettingsLink;
        private bool _showToggle;

        public DisplayGames(List<GameInput> games, string id = "game-date-control", bool showSettingsLink = false, bool showToggle = false)
        {
            _games = games;
            _id = id;
            _showSettingsLink = showSettingsLink;
            _showToggle = showToggle;
        }

        public DisplayGames(GameInput game, string id = "game-date-control", bool showSettingsLink = false, bool showToggle = false)
        {
            _games = new List<GameInput> { game };
            _id = id;
            _showSettingsLink = showSettingsLink;
            _showToggle = showToggle;
        }

        public string RenderSingleGameInner(bool hideTeamCells = false)
        {
            return new DisplayGame(_games[0], hideTeamCells: hideTeamCells).RenderInner();
        }

        public string RenderSingleGame()
        {
            return new DisplayGame(_games[0], noBorder: true).Render();
        }

        public string Render()
        {
            var wrapper = new HtmlTag("div").AddClass("game-date-control").Attr("id", _id);

            foreach (var game in _games)
                wrapper.AppendHtml(new DisplayGame(game, showToggle: _showToggle, toggleId: $"{_id}-game-{game.GameId}").Render());

            if (_games.Count > 0 && _games[0].DisplayTimezone != null)
            {
                var tz = _games[0].DisplayTimezone;
                var tzDisplay = tz.DisplayName;
                var footer = new HtmlTag("div").AddClass("game-date-timezone");

                if (_showSettingsLink)
                    footer.AppendHtml($"{tzDisplay} &middot; <a href='{RotoMonsterUIUrls.UserSettingsUrl}' class='game-date-tz-link'>Change timezone</a>");
                else
                    footer.AppendHtml(tzDisplay);

                wrapper.Append(footer);
            }

            return wrapper.ToString();
        }
    }
}