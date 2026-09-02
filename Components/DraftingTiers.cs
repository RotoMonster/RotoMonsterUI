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

            // One class turns it all off, rather than every tier and name
            // needing to know.
            if (!_input.ColorByTier) wrap.AddClass("tier-colors-off");

            var id = string.IsNullOrEmpty(_input.Id) ? "draftingTiers" : _input.Id;
            wrap.Attr("id", "drafting-tiers-" + id);

            if (!string.IsNullOrEmpty(_input.IntroHtml))
                wrap.Append(new HtmlTag("div").AddClass("dt-intro").AppendHtml(_input.IntroHtml));

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

        // ---- toolbar -------------------------------------------------------

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

                seg.Append(PositionButton(_input.AllPositionsText, "all", true));

                foreach (var pos in positions)
                {
                    if (string.IsNullOrEmpty(pos)) continue;
                    seg.Append(PositionButton(pos, pos, false));
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
                        .Text(tier.TierLabel ?? (i + 1).ToString()));
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

        // ---- tiers ---------------------------------------------------------

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

            // The colour comes from the shared palette, so a tier looks the
            // same here as it does on the draft board.
            if (tier.TierNumber > 0)
                box.AddClass(TierBadge.TierClass(tier.TierNumber));

            var head = new HtmlTag("div").AddClass("dt-tier-head");

            // One badge on the header rather than one per row, matching the
            // draft board's colours without repeating it a dozen times.
            if (tier.TierNumber > 0)
                head.AppendHtml(new TierBadge(tier.TierNumber).Render());

            head.Append(new HtmlTag("span")
                .AddClass("dt-tier-n")
                .Text(tier.TierLabel ?? ("Tier " + (index + 1))));

            // Filled by the script, since the count changes with the filter.
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

        private static HtmlTag RenderPlayer(DraftingTiersPlayer player, int tierNumber)
        {
            var row = new HtmlTag("div").AddClass("dt-player");

            var display = player.DisplayPlayerInput;

            // The filter and the search both read off the row, so neither has
            // to know anything about how the player is rendered.
            var positions = player.FilterPositions ?? new List<string>();
            row.Attr("data-dt-positions", string.Join(",", positions));
            row.Attr("data-dt-name", (display != null ? display.PlayerName : "") ?? "");

            // Filled by the script - the number is the player's place within
            // whatever the filter is currently showing, not a fixed rank.
            //
            // No tier badge per row here. Every player in a tier has the same
            // one, so it would be the same badge repeated down the group, and
            // the grouping already says which tier you are in. The badge earns
            // its place on the draft board, where the rows are mixed.
            row.Append(new HtmlTag("span").AddClass("dt-player-n"));

            var name = new HtmlTag("div").AddClass("dt-player-name");

            // The name carries the tier too, so a player still reads as theirs
            // once the heading has scrolled off.
            if (tierNumber > 0) name.AddClass("tier-name");

            if (display != null) name.AppendHtml(new DisplayPlayer(display).Render());
            row.Append(name);

            var note = new HtmlTag("div").AddClass("dt-player-note");
            if (!string.IsNullOrEmpty(player.NoteHtml)) note.AppendHtml(player.NoteHtml);
            row.Append(note);

            return row;
        }

        // ---- behaviour -----------------------------------------------------

        /// <summary>
        /// All client side. The page is static, so filtering by position or
        /// name needs no round trip - everything is already rendered.
        /// </summary>
        private string Script(string id)
        {
            var scope = "#drafting-tiers-" + id;

            return @"
(function () {
    var root = document.querySelector('" + scope + @"');
    if (!root) return;

    var search = document.getElementById('" + id + @"-search');
    var empty = root.querySelector('.dt-empty');
    var pos = 'all';

    function matches(row) {
        if (pos !== 'all') {
            var list = (row.getAttribute('data-dt-positions') || '').split(',');
            if (list.indexOf(pos) === -1) return false;
        }

        if (search) {
            var q = (search.value || '').toLowerCase().trim();
            if (q) {
                var name = (row.getAttribute('data-dt-name') || '').toLowerCase();
                if (name.indexOf(q) === -1) return false;
            }
        }

        return true;
    }

    function apply() {
        var anyShown = false;

        root.querySelectorAll('.dt-tier').forEach(function (tier) {
            var shown = 0;

            tier.querySelectorAll('.dt-player').forEach(function (row) {
                var ok = matches(row);
                row.hidden = !ok;
                if (ok) {
                    shown++;
                    var n = row.querySelector('.dt-player-n');
                    if (n) n.textContent = shown;
                }
            });

            // A tier with nothing left in it goes away entirely rather than
            // sitting there as an empty header.
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

            // With only a few tiers there is nothing to scroll, so the click
            // looks like it did nothing. Removing the class first is what lets
            // the same tier flash twice.
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