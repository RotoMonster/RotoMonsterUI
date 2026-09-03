using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class DraftingTiers
    {
        private readonly DraftingTiersInput _input;

        public DraftingTiers(DraftingTiersInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("drafting-tiers");

            if (_input == null) return wrap.ToString();

            if (!_input.ColorByTier) wrap.AddClass("tier-colors-off");

            var id = string.IsNullOrEmpty(_input.Id) ? "draftingTiers" : _input.Id;
            wrap.Attr("id", "drafting-tiers-" + id);
            wrap.Attr("data-dt-position-default", _input.SelectedPosition ?? "");

            if (!string.IsNullOrEmpty(_input.IntroHtml))
                wrap.Append(new HtmlTag("div").AddClass("dt-intro").AppendHtml(_input.IntroHtml));

            foreach (var summary in _input.PositionSummaries ?? new List<DraftingTiersSummary>())
            {
                if (summary == null || string.IsNullOrEmpty(summary.Html)) continue;

                wrap.Append(new HtmlTag("div")
                    .AddClass("dt-summary")
                    .Attr("data-dt-summary", summary.Position ?? "")
                    .AppendHtml(summary.Html));
            }

            var toolbar = RenderToolbar(id);
            if (toolbar != null) wrap.Append(toolbar);

            wrap.Append(RenderTiers(id));

            wrap.Append(new HtmlTag("div")
                .AddClass("dt-empty")
                .Attr("hidden", "hidden")
                .Text(_input.EmptyText));

            wrap.Append(new HtmlTag("script").AppendHtml(Script(id)));

            return wrap.ToString();
        }

        private HtmlTag RenderToolbar(string id)
        {
            var positions = _input.Positions ?? new List<string>();
            var hasPositions = positions.Any();

            if (!hasPositions && !_input.ShowSearch
                && !_input.ShowJumpToTier && !_input.ShowColorToggle) return null;

            var bar = new HtmlTag("div").AddClass("dt-toolbar");

            if (hasPositions)
            {
                var seg = new HtmlTag("div").AddClass("dt-positions");

                var selected = _input.SelectedPosition ?? "";

                seg.Append(PositionButton(_input.AllPositionsText, "", selected == ""));

                foreach (var pos in positions)
                {
                    if (string.IsNullOrEmpty(pos)) continue;
                    seg.Append(PositionButton(pos, pos, pos == selected));
                }

                bar.Append(seg);
            }

            if (_input.ShowColorToggle)
            {
                var toggleId = id + "-color";

                var label = new HtmlTag("label").AddClass("dt-switch").Attr("for", toggleId);

                var box = new HtmlTag("input")
                    .Attr("type", "checkbox")
                    .Attr("id", toggleId)
                    .Attr("data-dt-color", "1");

                if (_input.ColorByTier) box.Attr("checked", "checked");

                label.Append(box);
                label.Append(new HtmlTag("span").AddClass("dt-track"));
                label.Append(new HtmlTag("span").Text(_input.ColorToggleText));

                bar.Append(label);
            }

            if (_input.ShowSearch)
                bar.Append(new HtmlTag("input")
                    .AddClass("dt-search")
                    .Attr("type", "search")
                    .Attr("id", id + "-search")
                    .Attr("placeholder", _input.SearchPlaceholder)
                    .Attr("aria-label", _input.SearchPlaceholder));

            if (_input.ShowJumpToTier && (_input.Tiers ?? new List<DraftingTier>()).Any())
            {
                var jump = new HtmlTag("div").AddClass("dt-jump");

                if (!string.IsNullOrEmpty(_input.JumpLabel))
                    jump.Append(new HtmlTag("span").AddClass("dt-jump-label").Text(_input.JumpLabel));

                for (var i = 0; i < _input.Tiers.Count; i++)
                {
                    var tier = _input.Tiers[i];
                    if (tier == null) continue;

                    jump.Append(new HtmlTag("button")
                        .AddClass("dt-jump-btn")
                        .Attr("type", "button")
                        .Attr("data-dt-jump", i.ToString())
                        .Attr("data-dt-position", tier.Position ?? "")
                        .Text(TierHeading(tier, i)));
                }

                bar.Append(jump);
            }

            return bar;
        }

        private static HtmlTag PositionButton(string text, string value, bool selected)
        {
            return new HtmlTag("button")
                .AddClass("dt-pos")
                .Attr("type", "button")
                .Attr("data-dt-pos", value)
                .Attr("aria-pressed", selected ? "true" : "false")
                .Text(text);
        }

        private HtmlTag RenderTiers(string id)
        {
            var list = new HtmlTag("div").AddClass("dt-tiers");

            var tiers = _input.Tiers ?? new List<DraftingTier>();

            for (var i = 0; i < tiers.Count; i++)
            {
                var tier = tiers[i];
                if (tier == null) continue;

                list.Append(RenderTier(id, tier, i));
            }

            return list;
        }

        private HtmlTag RenderTier(string id, DraftingTier tier, int index)
        {
            var box = new HtmlTag("div")
                .AddClass("dt-tier")
                .Attr("id", id + "-tier-" + index)
                .Attr("data-dt-tier", index.ToString());

            if (tier.TierNumber > 0)
                box.AddClass(TierBadge.TierClass(tier.TierNumber));

            box.Attr("data-dt-position", tier.Position ?? "");

            var head = new HtmlTag("div").AddClass("dt-tier-head");

            if (tier.TierNumber > 0)
                head.AppendHtml(new TierBadge(tier.TierNumber).Render());

            head.Append(new HtmlTag("span")
                .AddClass("dt-tier-n")
                .Text(TierHeading(tier, index)));

            head.Append(new HtmlTag("span").AddClass("dt-tier-count"));

            if (!string.IsNullOrEmpty(tier.NoteText))
                head.Append(new HtmlTag("span").AddClass("dt-tier-note").Text(tier.NoteText));

            box.Append(head);

            foreach (var player in tier.Players ?? new List<DraftingTiersPlayer>())
            {
                if (player == null) continue;
                box.Append(RenderPlayer(player, tier.TierNumber));
            }

            return box;
        }

        private static string TierHeading(DraftingTier tier, int index)
        {
            if (!string.IsNullOrEmpty(tier.TierLabel)) return tier.TierLabel;

            var number = tier.TierNumber > 0 ? tier.TierNumber : index + 1;

            return string.IsNullOrEmpty(tier.Position)
                ? "Tier " + number
                : tier.Position + " " + number;
        }

        private static HtmlTag RenderPlayer(DraftingTiersPlayer player, int tierNumber)
        {
            var row = new HtmlTag("div").AddClass("dt-player");

            var display = player.DisplayPlayerInput;

            row.Attr("data-dt-name", (display != null ? display.PlayerName : "") ?? "");

            row.Append(new HtmlTag("span").AddClass("dt-player-n"));

            var name = new HtmlTag("div").AddClass("dt-player-name");

            if (tierNumber > 0) name.AddClass("tier-name");

            if (display != null) name.AppendHtml(new DisplayPlayer(display).Render());
            row.Append(name);

            var note = new HtmlTag("div").AddClass("dt-player-note");
            if (!string.IsNullOrEmpty(player.NoteHtml)) note.AppendHtml(player.NoteHtml);
            row.Append(note);

            return row;
        }

        private string Script(string id)
        {
            var scope = "#drafting-tiers-" + id;

            return @"
(function () {
    var root = document.querySelector('" + scope + @"');
    if (!root) return;

    var search = document.getElementById('" + id + @"-search');
    var empty = root.querySelector('.dt-empty');
    var pos = root.getAttribute('data-dt-position-default') || '';

    function nameMatches(row) {
        if (!search) return true;

        var q = (search.value || '').toLowerCase().trim();
        if (!q) return true;

        var name = (row.getAttribute('data-dt-name') || '').toLowerCase();
        return name.indexOf(q) !== -1;
    }

    function apply() {
        var anyShown = false;

        root.querySelectorAll('.dt-summary').forEach(function (summary) {
            summary.hidden = pos !== (summary.getAttribute('data-dt-summary') || '');
        });

        root.querySelectorAll('.dt-tier').forEach(function (tier) {
            if (pos !== (tier.getAttribute('data-dt-position') || '')) {
                tier.hidden = true;
                return;
            }

            var shown = 0;

            tier.querySelectorAll('.dt-player').forEach(function (row) {
                var ok = nameMatches(row);
                row.hidden = !ok;
                if (ok) {
                    shown++;
                    var n = row.querySelector('.dt-player-n');
                    if (n) n.textContent = shown;
                }
            });

            tier.hidden = shown === 0;
            if (shown) anyShown = true;

            var count = tier.querySelector('.dt-tier-count');
            if (count) {
                count.textContent = shown + ' ' +
                    (shown === 1 ? '" + _input.PlayerWord + @"' : '" + _input.PlayersWord + @"');
            }
        });

        root.querySelectorAll('.dt-jump-btn').forEach(function (btn) {
            var tier = root.querySelector('[data-dt-tier=""' + btn.getAttribute('data-dt-jump') + '""]');
            btn.hidden = !tier || tier.hidden;
        });

        if (empty) empty.hidden = anyShown;
    }

    var colorBox = root.querySelector('[data-dt-color]');
    if (colorBox) {
        colorBox.addEventListener('change', function () {
            root.classList.toggle('tier-colors-off', !colorBox.checked);
        });
    }

    root.querySelectorAll('.dt-pos').forEach(function (btn) {
        btn.addEventListener('click', function () {
            pos = btn.getAttribute('data-dt-pos');

            root.querySelectorAll('.dt-pos').forEach(function (b) {
                b.setAttribute('aria-pressed', b === btn ? 'true' : 'false');
            });

            apply();
        });
    });

    if (search) search.addEventListener('input', apply);

    root.querySelectorAll('.dt-jump-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var tier = root.querySelector('[data-dt-tier=""' + btn.getAttribute('data-dt-jump') + '""]');
            if (!tier) return;

            tier.scrollIntoView({ behavior: 'smooth', block: 'start' });

            tier.classList.remove('dt-tier--flash');
            void tier.offsetWidth;
            tier.classList.add('dt-tier--flash');

            setTimeout(function () {
                tier.classList.remove('dt-tier--flash');
            }, 1000);
        });
    });

    apply();
})();
";
        }
    }
}