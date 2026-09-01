using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class LiveResults
    {
        private readonly LiveResultsInput _input;

        public LiveResults(LiveResultsInput input)
        {
            _input = input;
        }

        private string Key(string prefix)
        {
            return prefix + "_" + _input.Id;
        }

        private string RowKey(string prefix, string value)
        {
            return prefix + "_" + _input.Id + "_" + value;
        }

        private static string StateSlug(LiveResultsGameState state)
        {
            switch (state)
            {
                case LiveResultsGameState.Live: return "live";
                case LiveResultsGameState.Final: return "final";
                default: return "soon";
            }
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("live-results");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", "live-results-" + _input.Id);

            wrap.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("name", Key("lrcurrent"))
                .Attr("value", _input.SelectedGameId ?? ""));

            wrap.Append(RenderDayBar());

            if (!string.IsNullOrEmpty(_input.Message))
                wrap.Append(new HtmlTag("div").AddClass("lr-message").Text(_input.Message));

            if (!string.IsNullOrEmpty(_input.SettingsHtml))
                wrap.AppendHtml(_input.SettingsHtml);

            wrap.Append(RenderGames());

            if (!string.IsNullOrEmpty(_input.ContentHtml))
                wrap.Append(new HtmlTag("div")
                    .AddClass("lr-content")
                    .AppendHtml(_input.ContentHtml));

            return wrap.ToString();
        }

        private HtmlTag RenderDayBar()
        {
            var bar = new HtmlTag("div").AddClass("lr-daybar");

            if (_input.ShowPreviousDay)
                bar.Append(NavButton(Key("lrprev"), "\u2039", "Previous day"));

            if (_input.ShowNextDay)
                bar.Append(NavButton(Key("lrnext"), "\u203a", "Next day"));

            if (!string.IsNullOrEmpty(_input.DayText))
                bar.Append(new HtmlTag("span").AddClass("lr-day").Text(_input.DayText));

            if (!string.IsNullOrEmpty(_input.CountText))
                bar.Append(new HtmlTag("span").AddClass("lr-count").Text(_input.CountText));

            bar.Append(new HtmlTag("span").AddClass("lr-spacer"));

            if (_input.ShowMyPlayersToggle)
                bar.Append(Switch(Key("lrmine"), _input.MyPlayersToggleText, _input.MyPlayersOnly));

            bar.AppendHtml(new Button(_input.RefreshButtonText)
                .WithStyle(ButtonStyle.Primary)
                .WithName(Key("lrrefresh"))
                .WithPostBack()
                .Render());

            return bar;
        }

        private static HtmlTag NavButton(string name, string glyph, string title)
        {
            return new HtmlTag("button")
                .AddClass("lr-nav")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("title", title)
                .Attr("aria-label", title)
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text(glyph);
        }

        private HtmlTag RenderGames()
        {
            var strip = new HtmlTag("div").AddClass("lr-games");

            var games = _input.Games ?? new List<LiveResultsGame>();
            if (!games.Any()) return strip;

            strip.Append(GameButton("all", _input.AllGamesText,
                string.IsNullOrEmpty(_input.SelectedGameId)));

            foreach (var game in games)
            {
                if (game == null) continue;
                strip.Append(GameCard(game));
            }

            return strip;
        }

        private HtmlTag GameButton(string value, string text, bool selected)
        {
            var name = RowKey("lrgame", value);

            var button = new HtmlTag("button")
                .AddClass("lr-game lr-game--all")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("aria-pressed", selected ? "true" : "false")
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text(text);

            return button;
        }

        private HtmlTag GameCard(LiveResultsGame game)
        {
            var name = RowKey("lrgame", game.GameId);
            var slug = StateSlug(game.State);
            var selected = game.GameId == _input.SelectedGameId;

            var button = new HtmlTag("button")
                .AddClass("lr-game")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("aria-pressed", selected ? "true" : "false")
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)");

            if (!string.IsNullOrEmpty(game.TooltipText))
                button.Attr("title", game.TooltipText);

            var top = new HtmlTag("div").AddClass("lr-game-top " + slug);

            if (game.State == LiveResultsGameState.Live)
                top.Append(new HtmlTag("span").AddClass("lr-game-dot"));

            top.Append(new HtmlTag("span").Text(game.StateText ?? DefaultStateText(game.State)));

            if (game.MyPlayerCount > 0)
                top.Append(new HtmlTag("span")
                    .AddClass("lr-game-mine")
                    .Text(game.MyPlayerCount.ToString()));

            button.Append(top);

            // Only a final has a loser. Greying anyone out while the game is
            // still being played would be saying something that is not true
            // yet - whoever is behind may not stay there.
            var settled = game.State == LiveResultsGameState.Final;
            var awayAhead = Ahead(game.AwayScoreText, game.HomeScoreText);

            button.Append(Side(game.AwayTeamCode, game.AwayScoreText,
                settled && !awayAhead));
            button.Append(Side(game.HomeTeamCode, game.HomeScoreText,
                settled && awayAhead));

            return button;
        }

        private static string DefaultStateText(LiveResultsGameState state)
        {
            switch (state)
            {
                case LiveResultsGameState.Live: return "Live";
                case LiveResultsGameState.Final: return "Final";
                default: return "";
            }
        }

        /// <summary>
        /// Scores are strings so the caller can send whatever it has, which
        /// means a comparison has to cope with anything that is not a number.
        /// Unparseable means nobody is ahead, so nobody greys out.
        /// </summary>
        private static bool Ahead(string a, string b)
        {
            int x, y;
            if (!int.TryParse((a ?? "").Trim(), out x)) return false;
            if (!int.TryParse((b ?? "").Trim(), out y)) return false;
            return x > y;
        }

        private static HtmlTag Side(string code, string score, bool lost)
        {
            var side = new HtmlTag("div").AddClass("lr-game-side");
            if (lost) side.AddClass("lr-game-side--lost");

            side.Append(new HtmlTag("span").AddClass("lr-game-tm").Text(code ?? ""));
            side.Append(new HtmlTag("span").AddClass("lr-game-sc").Text(score ?? ""));

            return side;
        }

        private static HtmlTag Switch(string name, string text, bool on)
        {
            var label = new HtmlTag("label").AddClass("ms-switch").Attr("for", name);

            var box = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("id", name)
                .Attr("name", name)
                .Attr("value", "1");

            if (on) box.Attr("checked", "checked");

            label.Append(box);
            label.Append(new HtmlTag("span").AddClass("ms-track"));
            label.Append(new HtmlTag("span").Text(text));

            return label;
        }
    }
}