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

            if (_input.ExtensionDetect != null)
                wrap.AppendHtml(new ExtensionDetect(_input.ExtensionDetect).Render());

            if (_input.ShowDraftPicksField)
                wrap.Append(RenderDraftPicksField());

            wrap.Append(RenderConnect());
            wrap.Append(RenderDuringDraft());

            if (!string.IsNullOrEmpty(_input.Message))
                wrap.Append(new HtmlTag("div").AddClass("dm-message").Text(_input.Message));

            wrap.AppendHtml(new MonsterSettings(ToSettingsInput()).Render());

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

            return wrap.ToString();
        }

        private HtmlTag RenderDraftPicksField()
        {
            var fieldId = string.IsNullOrEmpty(_input.DraftPicksFieldId)
                ? "espnDraftPicks"
                : _input.DraftPicksFieldId;

            var wrap = new HtmlTag("span").AddClass("dm-picks-field");

            wrap.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("id", fieldId)
                .Attr("name", fieldId));

            if (_input.DraftPicksAutoPostBack)
                wrap.Append(new HtmlTag("script").AppendHtml(PicksScript(fieldId)));

            return wrap;
        }

        private static string PicksScript(string fieldId)
        {
            return @"
(function () {
    if (window.rmDraftPicksBound) return;
    window.rmDraftPicksBound = true;

    document.addEventListener('rm-draft-imported', function () {
        var field = document.getElementById('" + fieldId + @"');
        if (!field || !field.value) return;

        if (typeof __doPostBack === 'function') {
            __doPostBack('" + fieldId + @"', '');
            return;
        }

        var form = field.form;
        if (form) form.submit();
    });
})();
";
        }

        private HtmlTag RenderConnect()
        {
            var block = new HtmlTag("div")
                .AddClass("dm-connect")
                .Attr("id", Key("dmconnectblock"));

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
                .AddClass("ms-input dm-input--pick")
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

        private HtmlTag RenderDuringDraft()
        {
            var strip = new HtmlTag("div")
                .AddClass("dm-during")
                .Attr("id", Key("dmduring"));

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

        private string SettingsId()
        {
            return _input.Id + "settings";
        }

        private MonsterSettingsInput ToSettingsInput()
        {
            var settings = new MonsterSettingsInput
            {
                Id = SettingsId(),

                ValuesPanelHeading = "How players are valued",
                StandingsPanelHeading = "Standings",
                TablePanelHeading = "What the table shows",
                SetOnceText = "Set once before you draft",
                AnyTimeText = "Change any time",

                ValuesExpanded = _input.ValuesExpanded,
                StandingsExpanded = _input.StandingsSettingsExpanded,
                TableExpanded = _input.TableSettingsExpanded,

                SelectedProjectionSourceId = _input.SelectedProjectionSourceId,
                SelectedValueTypeId = _input.SelectedValueTypeId,
                ReplacementPlayers = _input.ReplacementPlayers,
                AssumeGoodHealth = _input.AssumeGoodHealth,
                PuntHelpText = _input.PuntHelpText,

                ShowRotoStandings = _input.ShowRotoStandings,
                ShowH2HStandings = _input.ShowH2HStandings,
                UseAdvancedStandings = _input.UseAdvancedStandings,
                ApplyGameLimits = _input.ApplyGameLimits,

                SelectedStatsDisplayFormatId = _input.SelectedStatsDisplayFormatId,
                SelectedValueConsistencyId = _input.SelectedValueConsistencyId,
                SelectedPlayerFilterId = _input.SelectedPlayerFilterId,
                SelectedTeamId = _input.SelectedTeamId,
                AllPositionsSelected = _input.AllPositionsSelected,

                ColumnsButtonText = _input.ColumnsButtonText,
                ColumnsUrl = _input.ColumnsUrl,
                ColumnsPostsBack = _input.ColumnsPostsBack,
                ColumnsInput = _input.ColumnsInput,
                CustomValuesInput = _input.CustomValuesInput,
                ColumnsHtml = _input.ColumnsHtml,
                ColumnsOpen = _input.ColumnsOpen,
                ShowTiersRow = _input.ShowTiersRow,
                ShowTierColumn = _input.ShowTierColumn,
                ColorByTier = _input.ColorByTier,
                ColumnsSummary = _input.ColumnsSummary
            };

            settings.ProjectionSources = _input.ProjectionSources;
            settings.ValueTypes = _input.ValueTypes;
            settings.StatsDisplayFormats = _input.StatsDisplayFormats;
            settings.ValueConsistencies = _input.ValueConsistencies;
            settings.PlayerFilters = _input.PlayerFilters;
            settings.Teams = _input.Teams;
            settings.PuntCategories = _input.PuntCategories;
            settings.Positions = _input.Positions;

            return settings;
        }

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

        private HtmlTag Panel(string name, string heading, string when, HtmlTag body, bool expanded)
        {
            var panel = new HtmlTag("div").AddClass("ms-panel");

            var baseId = SubKey("dmpanel", name);
            var contentId = baseId + "-content";
            var toggleId = baseId + "-toggle";

            var head = new HtmlTag("button")
                .AddClass("ms-panel-head")
                .Attr("type", "button")
                .Attr("id", baseId)
                .Attr("data-toggle", "collapse")
                .Attr("data-target", "#" + contentId)
                .Attr("aria-controls", contentId)
                .Attr("aria-expanded", expanded ? "true" : "false");

            head.Append(new HtmlTag("span").AddClass("ms-caret").AppendHtml("&#9662;"));
            head.Append(new HtmlTag("span").AddClass("ms-panel-title").Text(heading));

            if (!string.IsNullOrEmpty(when))
                head.Append(new HtmlTag("span").AddClass("ms-panel-when").Text(when));

            panel.Append(head);

            panel.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("id", toggleId)
                .Attr("name", toggleId)
                .Attr("value", expanded ? "1" : "0"));

            var wrapper = new HtmlTag("div")
                .Attr("id", contentId)
                .AddClass(expanded ? "ms-panel-body collapse show" : "ms-panel-body collapse");

            wrapper.Append(body);
            panel.Append(wrapper);

            return panel;
        }

        private static HtmlTag Row(string label, List<HtmlTag> controls)
        {
            var row = new HtmlTag("div").AddClass("ms-row");

            row.Append(new HtmlTag("span").AddClass("ms-label").Text(label));

            var wrap = new HtmlTag("div").AddClass("ms-controls");
            foreach (var control in controls)
            {
                if (control != null) wrap.Append(control);
            }

            row.Append(wrap);
            return row;
        }

        private static HtmlTag Select(string name, List<MonsterOption> options, string selected)
        {
            var dropdown = new Dropdown("Select").WithName(name);

            foreach (var option in options ?? new List<MonsterOption>())
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