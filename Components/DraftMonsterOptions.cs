using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class DraftMonsterOptions
    {
        private readonly DraftMonsterOptionsInput _input;

        public DraftMonsterOptions(DraftMonsterOptionsInput input)
        {
            _input = input;
        }

        private string Key(string prefix)
        {
            return prefix + "_" + _input.Id;
        }

        private string SubKey(string prefix, string name)
        {
            return prefix + "_" + _input.Id + "_" + name;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("draft-monster");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", "draft-monster-" + _input.Id);

            wrap.Append(RenderConnect());
            wrap.Append(RenderDuringDraft());

            if (!string.IsNullOrEmpty(_input.Message))
                wrap.Append(new HtmlTag("div").AddClass("dm-message").Text(_input.Message));

            wrap.Append(RenderValuesPanel());
            wrap.Append(RenderStandingsSettingsPanel());
            wrap.Append(RenderTablePanel());

            if (!string.IsNullOrEmpty(_input.StandingsHtml))
                wrap.Append(RenderOutput("standings", _input.StandingsHeading,
                    _input.StandingsHtml, _input.StandingsExpanded, _input.StandingsCompact, true));

            if (!string.IsNullOrEmpty(_input.TeamAnalysisHtml))
                wrap.Append(RenderOutput("analysis", _input.TeamAnalysisHeading,
                    _input.TeamAnalysisHtml, _input.TeamAnalysisExpanded, _input.TeamAnalysisCompact, true));

            if (!string.IsNullOrEmpty(_input.ResultsHtml))
                wrap.Append(new HtmlTag("div")
                    .AddClass("dm-results")
                    .AppendHtml(_input.ResultsHtml));

            wrap.Append(new HtmlTag("script").AppendHtml(Script()));

            return wrap.ToString();
        }

        // ---- connect -----------------------------------------------------

        private HtmlTag RenderConnect()
        {
            var block = new HtmlTag("div").AddClass("dm-connect");
            if (_input.IsConnected) block.AddClass("dm-connect--live");

            if (_input.IsConnected)
            {
                var row = new HtmlTag("div").AddClass("dm-connect-row");

                if (!string.IsNullOrEmpty(_input.ConnectedStatusHtml))
                    row.Append(new HtmlTag("span")
                        .AddClass("dm-connect-status")
                        .AppendHtml(_input.ConnectedStatusHtml));

                row.AppendHtml(new Button("Change")
                    .WithStyle(ButtonStyle.Secondary)
                    .WithName(Key("dmchangepick"))
                    .WithPostBack()
                    .Render());

                block.Append(row);
                block.Append(HiddenPick());
                return block;
            }

            block.Append(new HtmlTag("h2").AddClass("dm-connect-heading").Text(_input.ConnectHeading));

            if (!string.IsNullOrEmpty(_input.ConnectLead))
                block.Append(new HtmlTag("p").AddClass("dm-lead").Text(_input.ConnectLead));

            var controls = new HtmlTag("div").AddClass("dm-connect-row");

            controls.Append(new HtmlTag("input")
                .AddClass("dm-input dm-input--pick")
                .Attr("type", "text")
                .Attr("id", Key("dmpick"))
                .Attr("name", Key("dmpick"))
                .Attr("placeholder", "Pick #")
                .Attr("aria-label", "Your pick number")
                .Attr("value", _input.PickNumber ?? ""));

            controls.AppendHtml(new Button(_input.ConnectButtonText)
                .WithStyle(ButtonStyle.Primary)
                .WithName(Key("dmconnect"))
                .WithPostBack()
                .Render());

            controls.Append(Switch(Key("dmrev3"), "3rd round reversal", _input.ThirdRoundReversal));
            controls.Append(Switch(Key("dmrev2"), "2nd round+ high to low", _input.SecondRoundHighToLow));
            controls.Append(Switch(Key("dmrev5"), "5th round reversal", _input.FifthRoundReversal));

            block.Append(controls);
            return block;
        }

        private HtmlTag HiddenPick()
        {
            return new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("name", Key("dmpick"))
                .Attr("value", _input.PickNumber ?? "");
        }

        // ---- during the draft --------------------------------------------

        private HtmlTag RenderDuringDraft()
        {
            var strip = new HtmlTag("div").AddClass("dm-during");

            strip.Append(new HtmlTag("span").AddClass("dm-strip-label").Text("During the draft"));

            strip.Append(Switch(Key("dmhidedrafted"), "Hide drafted players", _input.HideDraftedPlayers));
            strip.Append(Switch(Key("dmhighlight"), "Highlight drafted since import", _input.HighlightDraftedSinceImport));
            strip.Append(Switch(Key("dmtargets"), "Include targets in analysis", _input.IncludeTargetsInAnalysis));
            strip.Append(Switch(Key("dmstatfilters"), "Show stat filters", _input.ShowStatFilters));

            strip.AppendHtml(new Button("Refresh")
                .WithStyle(ButtonStyle.Secondary)
                .WithName(Key("dmrefresh"))
                .WithPostBack()
                .Render());

            return strip;
        }

        // ---- settings panels ---------------------------------------------

        private HtmlTag RenderValuesPanel()
        {
            var body = new HtmlTag("div");

            body.Append(Row("Projections", new List<HtmlTag>
            {
                Select(Key("dmproj"), _input.ProjectionSources, _input.SelectedProjectionSourceId),
                Select(Key("dmvaluetype"), _input.ValueTypes, _input.SelectedValueTypeId)
            }));

            body.Append(Row("Adjustments", new List<HtmlTag>
            {
                Switch(Key("dmreplacement"), "Compare to replacement players", _input.ReplacementPlayers),
                Switch(Key("dmhealth"), "Assume good health", _input.AssumeGoodHealth)
            }));

            var punt = new HtmlTag("div").AddClass("dm-stack");

            if (!string.IsNullOrEmpty(_input.PuntHelpText))
                punt.Append(new HtmlTag("span").AddClass("dm-hint").Text(_input.PuntHelpText));

            var grid = new HtmlTag("div").AddClass("dm-punt-grid");

            foreach (var cat in _input.PuntCategories ?? new List<DraftMonsterPuntCategory>())
            {
                if (cat == null) continue;
                grid.Append(PuntCategory(cat));
            }

            punt.Append(grid);
            body.Append(Row("Punting", new List<HtmlTag> { punt }));

            return Panel("values", "How players are valued", "Set once before you draft",
                body, _input.ValuesExpanded);
        }

        private HtmlTag PuntCategory(DraftMonsterPuntCategory cat)
        {
            var cell = new HtmlTag("span").AddClass("dm-punt");

            var boxId = SubKey("dmpunt", cat.CategoryId);

            var label = new HtmlTag("label").AddClass("dm-punt-cat").Attr("for", boxId);

            var box = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("id", boxId)
                .Attr("name", boxId)
                .Attr("value", "1");

            if (cat.IsSelected) box.Attr("checked", "checked");

            label.Append(box);
            label.Append(new HtmlTag("span").AddClass("dm-punt-abbr").Text(cat.Abbreviation));

            cell.Append(label);

            cell.Append(new HtmlTag("input")
                .AddClass("dm-input dm-punt-weight")
                .Attr("type", "text")
                .Attr("name", SubKey("dmpuntw", cat.CategoryId))
                .Attr("placeholder", "1")
                .Attr("aria-label", "Weight for " + cat.Abbreviation)
                .Attr("value", cat.Weight ?? ""));

            return cell;
        }

        private HtmlTag RenderStandingsSettingsPanel()
        {
            var body = new HtmlTag("div");

            body.Append(Row("Format", new List<HtmlTag>
            {
                Switch(Key("dmroto"), "Roto", _input.ShowRotoStandings),
                Switch(Key("dmh2h"), "H2H", _input.ShowH2HStandings)
            }));

            body.Append(Row("Options", new List<HtmlTag>
            {
                Switch(Key("dmadvanced"), "Advanced standings", _input.UseAdvancedStandings),
                Switch(Key("dmgamelimits"), "Apply game limits", _input.ApplyGameLimits)
            }));

            return Panel("standingset", "Standings", "Set once before you draft",
                body, _input.StandingsSettingsExpanded);
        }

        private HtmlTag RenderTablePanel()
        {
            var body = new HtmlTag("div");

            body.Append(Row("Stats format", new List<HtmlTag>
            {
                Select(Key("dmstatsformat"), _input.StatsDisplayFormats, _input.SelectedStatsDisplayFormatId),
                Select(Key("dmvaluec"), _input.ValueConsistencies, _input.SelectedValueConsistencyId)
            }));

            body.Append(Row("Players shown", new List<HtmlTag>
            {
                Select(Key("dmfilter"), _input.PlayerFilters, _input.SelectedPlayerFilterId),
                Select(Key("dmteam"), _input.Teams, _input.SelectedTeamId),
                Select(Key("dmhomeaway"), _input.HomeAwayOptions, _input.SelectedHomeAwayId)
            }));

            body.Append(Row("Positions", new List<HtmlTag> { PositionFilter() }));

            var columns = new HtmlTag("div").AddClass("dm-controls");

            if (!string.IsNullOrEmpty(_input.ColumnsUrl))
                columns.Append(new HtmlTag("a")
                    .AddClass("dm-btn")
                    .Attr("href", _input.ColumnsUrl)
                    .Text(_input.ColumnsButtonText));

            if (!string.IsNullOrEmpty(_input.ColumnsSummary))
                columns.Append(new HtmlTag("span").AddClass("dm-hint").Text(_input.ColumnsSummary));

            body.Append(Row("Columns", new List<HtmlTag> { columns }));

            return Panel("table", "What the table shows", "Change any time",
                body, _input.TableSettingsExpanded);
        }

        private HtmlTag PositionFilter()
        {
            var wrap = new HtmlTag("div").AddClass("dm-positions");

            var all = new HtmlTag("button")
                .AddClass("dm-pos dm-pos--all")
                .Attr("type", "button")
                .Attr("data-dm-all", "1")
                .Text("All");

            if (_input.AllPositionsSelected) all.AddClass("dm-pos--on");
            wrap.Append(all);

            foreach (var pos in _input.Positions ?? new List<DraftMonsterPosition>())
            {
                if (pos == null) continue;

                var boxId = SubKey("dmpos", pos.PositionId);

                var label = new HtmlTag("label").AddClass("dm-pos").Attr("for", boxId);

                if (!string.IsNullOrEmpty(pos.ColorCSS))
                    label.Attr("style", "--dm-pos-color:" + pos.ColorCSS + ";");

                var box = new HtmlTag("input")
                    .Attr("type", "checkbox")
                    .Attr("id", boxId)
                    .Attr("name", boxId)
                    .Attr("value", "1");

                if (pos.IsSelected) box.Attr("checked", "checked");

                label.Append(box);
                label.Append(new HtmlTag("span").Text(pos.Abbreviation));

                wrap.Append(label);
            }

            return wrap;
        }

        // ---- output sections ---------------------------------------------

        private HtmlTag RenderOutput(string name, string heading, string html,
            bool expanded, bool compact, bool showCompactToggle)
        {
            var body = new HtmlTag("div").AddClass("dm-output-body");

            if (showCompactToggle)
            {
                var bar = new HtmlTag("div").AddClass("dm-output-bar");
                bar.Append(Switch(SubKey("dmcompact", name),
                    compact ? _input.CompactText : _input.FullText, compact));
                body.Append(bar);
            }

            body.AppendHtml(html);

            return Panel(name, heading, null, body, expanded);
        }

        // ---- building blocks ---------------------------------------------

        private HtmlTag Panel(string name, string heading, string when, HtmlTag body, bool expanded)
        {
            var panel = new HtmlTag("div").AddClass("dm-panel");

            var toggleName = SubKey("dmtoggle", name);

            var head = new HtmlTag("button")
                .AddClass("dm-panel-head")
                .Attr("type", "button")
                .Attr("name", toggleName)
                .Attr("aria-expanded", expanded ? "true" : "false")
                .Attr("onclick", "__doPostBack('" + toggleName + "','',this.form)");

            head.Append(new HtmlTag("span").AddClass("dm-caret").AppendHtml("&#9662;"));
            head.Append(new HtmlTag("span").AddClass("dm-panel-title").Text(heading));

            if (!string.IsNullOrEmpty(when))
                head.Append(new HtmlTag("span").AddClass("dm-panel-when").Text(when));

            panel.Append(head);

            panel.Append(new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("name", SubKey("dmopen", name))
                .Attr("class", "dm-state")
                .Attr("value", "1")
                .Attr("checked", expanded ? "checked" : null)
                .Attr("hidden", "hidden"));

            var wrapper = new HtmlTag("div").AddClass("dm-panel-body");
            if (!expanded) wrapper.Attr("hidden", "hidden");
            wrapper.Append(body);

            panel.Append(wrapper);

            return panel;
        }

        private static HtmlTag Row(string label, List<HtmlTag> controls)
        {
            var row = new HtmlTag("div").AddClass("dm-row");

            row.Append(new HtmlTag("span").AddClass("dm-label").Text(label));

            var wrap = new HtmlTag("div").AddClass("dm-controls");
            foreach (var control in controls)
            {
                if (control != null) wrap.Append(control);
            }

            row.Append(wrap);
            return row;
        }

        private static HtmlTag Select(string name, List<DraftMonsterOption> options, string selected)
        {
            var dropdown = new Dropdown("Select").WithName(name);

            foreach (var option in options ?? new List<DraftMonsterOption>())
            {
                if (option == null) continue;
                dropdown.AddItem(option.Text, option.Value);
            }

            if (!string.IsNullOrEmpty(selected))
                dropdown.WithSelectedValue(selected);

            return new HtmlTag("div")
                .AddClass("dm-select-wrap")
                .AppendHtml(dropdown.Render());
        }

        private static HtmlTag Switch(string name, string text, bool on)
        {
            var label = new HtmlTag("label").AddClass("dm-switch").Attr("for", name);

            var box = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("id", name)
                .Attr("name", name)
                .Attr("value", "1");

            if (on) box.Attr("checked", "checked");

            label.Append(box);
            label.Append(new HtmlTag("span").AddClass("dm-track"));
            label.Append(new HtmlTag("span").Text(text));

            return label;
        }

        private string Script()
        {
            var scope = "#draft-monster-" + _input.Id;

            return @"
(function () {
    var root = document.querySelector('" + scope + @"');
    if (!root) return;

    var wrap = root.querySelector('.dm-positions');
    if (!wrap) return;

    var all = wrap.querySelector('[data-dm-all]');
    var boxes = Array.prototype.slice.call(wrap.querySelectorAll('input[type=checkbox]'));

    function sync() {
        var any = boxes.some(function (b) { return b.checked; });
        if (all) all.classList.toggle('dm-pos--on', !any);
    }

    if (all) {
        all.addEventListener('click', function () {
            boxes.forEach(function (b) { b.checked = false; });
            sync();
        });
    }

    boxes.forEach(function (b) { b.addEventListener('change', sync); });
    sync();
})();
";
        }
    }
}