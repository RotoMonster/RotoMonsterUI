using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class NbaStartingLineups
    {
        private readonly NbaStartingLineupsInput _input;

        public NbaStartingLineups(NbaStartingLineupsInput input)
        {
            _input = input;
        }

        private string Key(string prefix)
        {
            return prefix + "_" + _input.Id;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("nba-lineups");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", "nba-lineups-" + _input.Id);

            if (!_input.ShowBench) wrap.AddClass("nl-hide-bench");
            if (!_input.ShowProjectedMinutes) wrap.AddClass("nl-hide-mins");
            if (_input.MyPlayersOnly) wrap.AddClass("nl-only-mine");

            wrap.Append(RenderBar());

            if (!string.IsNullOrEmpty(_input.Message))
                wrap.Append(new HtmlTag("div").AddClass("nl-message").Text(_input.Message));

            wrap.Append(RenderGames());
            wrap.Append(new HtmlTag("script").AppendHtml(Script()));

            return wrap.ToString();
        }

        private HtmlTag RenderBar()
        {
            var bar = new HtmlTag("div").AddClass("nl-bar");

            if (_input.ShowPreviousDay)
                bar.Append(NavButton(Key("nlprev"), "\u2039", "Previous day"));

            if (_input.ShowNextDay)
                bar.Append(NavButton(Key("nlnext"), "\u203a", "Next day"));

            if (!string.IsNullOrEmpty(_input.DayText))
                bar.Append(new HtmlTag("span").AddClass("nl-day").Text(_input.DayText));

            if (!string.IsNullOrEmpty(_input.CountText))
                bar.Append(new HtmlTag("span").AddClass("nl-count").Text(_input.CountText));

            bar.Append(new HtmlTag("span").AddClass("nl-spacer"));

            if (_input.ShowMinutesToggle)
                bar.Append(Switch(Key("nlminutes"), _input.MinutesToggleText,
                    _input.ShowProjectedMinutes, "mins"));

            if (_input.ShowBenchToggle)
                bar.Append(Switch(Key("nlbench"), _input.BenchToggleText,
                    _input.ShowBench, "bench"));

            if (_input.ShowMyPlayersToggle)
                bar.Append(Switch(Key("nlmine"), _input.MyPlayersToggleText,
                    _input.MyPlayersOnly, "mine"));

            bar.AppendHtml(new Button(_input.RefreshButtonText)
                .WithStyle(ButtonStyle.Primary)
                .WithName(Key("nlrefresh"))
                .WithPostBack()
                .Render());

            return bar;
        }

        private static HtmlTag NavButton(string name, string glyph, string title)
        {
            return new HtmlTag("button")
                .AddClass("nl-nav")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("title", title)
                .Attr("aria-label", title)
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text(glyph);
        }

        private HtmlTag RenderGames()
        {
            var games = _input.Games ?? new List<NbaLineupCardInput>();

            if (games.Count == 0)
                return new HtmlTag("div").AddClass("nl-empty").Text(_input.EmptyText);

            var grid = new HtmlTag("div").AddClass("nl-grid");

            var columns = _input.Columns > 0 ? _input.Columns : 2;
            grid.Attr("style", "--nl-columns:" + columns + ";");

            foreach (var game in games)
            {
                if (game == null) continue;

                game.ShowBench = true;
                game.ShowProjectedMinutes = true;

                grid.Append(new HtmlTag("div")
                    .AddClass("nl-game")
                    .AppendHtml(new NbaLineupCard(game).Render()));
            }

            return grid;
        }

        private static HtmlTag Switch(string name, string text, bool on, string instant = null)
        {
            var label = new HtmlTag("label").AddClass("ms-switch").Attr("for", name);

            var box = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("id", name)
                .Attr("name", name)
                .Attr("value", "1");

            if (instant != null) box.Attr("data-nl-instant", instant);
            if (on) box.Attr("checked", "checked");

            label.Append(box);
            label.Append(new HtmlTag("span").AddClass("ms-track"));
            label.Append(new HtmlTag("span").Text(text));

            return label;
        }
        private string Script()
        {
            return @"
(function () {
    var root = document.querySelector('#nba-lineups-" + _input.Id + @"');
    if (!root) return;

    root.querySelectorAll('[data-nl-instant]').forEach(function (box) {
        box.addEventListener('change', function () {
            var what = box.getAttribute('data-nl-instant');

            if (what === 'mine') {
                root.classList.toggle('nl-only-mine', box.checked);
                return;
            }

            root.classList.toggle('nl-hide-' + what, !box.checked);
        });
    });
})();
";
        }

    }
}