using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class MonsterSettings
    {
        private readonly MonsterSettingsInput _input;

        public MonsterSettings(MonsterSettingsInput input)
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
            var wrap = new HtmlTag("div").AddClass("monster-settings");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", "monster-settings-" + _input.Id);

            if (_input.ShowValuesPanel)
            {
                var body = ValuesBody();
                if (body != null)
                    wrap.Append(Panel("values", _input.ValuesPanelHeading,
                        _input.SetOnceText, body, _input.ValuesExpanded));
            }

            if (_input.ShowStandingsPanel)
            {
                var body = StandingsBody();
                if (body != null)
                    wrap.Append(Panel("standings", _input.StandingsPanelHeading,
                        _input.SetOnceText, body, _input.StandingsExpanded));
            }

            if (_input.ShowTablePanel)
            {
                var body = TableBody();
                if (body != null)
                    wrap.Append(Panel("table", _input.TablePanelHeading,
                        _input.AnyTimeText, body, _input.TableExpanded));
            }

            if (_input.Positions != null && _input.Positions.Count > 0)
                wrap.Append(new HtmlTag("script").AppendHtml(Script()));

            return wrap.ToString();
        }

        // ---- panel bodies -------------------------------------------------

        private HtmlTag ValuesBody()
        {
            var body = new HtmlTag("div");
            var any = false;

            var projections = new List<HtmlTag>();

            if (Has(_input.DateRanges))
                projections.Add(Select(Key("msdate"), _input.DateRanges, _input.SelectedDateRangeId));
            if (Has(_input.ProjectionSources))
                projections.Add(Select(Key("msproj"), _input.ProjectionSources, _input.SelectedProjectionSourceId));
            if (Has(_input.ValueTypes))
                projections.Add(Select(Key("msvaluetype"), _input.ValueTypes, _input.SelectedValueTypeId));
            if (_input.ShowRestOfSeason)
                projections.Add(Switch(Key("msros"), "Rest of season", _input.RestOfSeason));

            if (projections.Count > 0)
            {
                body.Append(Row(_input.ProjectionsLabel, projections));
                any = true;
            }

            if (_input.ShowAdjustments)
            {
                body.Append(Row(_input.AdjustmentsLabel, new List<HtmlTag>
                {
                    Switch(Key("msreplacement"), "Compare to replacement players", _input.ReplacementPlayers),
                    Switch(Key("mshealth"), "Assume good health", _input.AssumeGoodHealth)
                }));
                any = true;
            }

            if (Has(_input.PuntCategories))
            {
                var punt = new HtmlTag("div").AddClass("ms-stack");

                if (!string.IsNullOrEmpty(_input.PuntHelpText))
                    punt.Append(new HtmlTag("span").AddClass("ms-hint").Text(_input.PuntHelpText));

                var grid = new HtmlTag("div").AddClass("ms-punt-grid");

                foreach (var cat in _input.PuntCategories)
                {
                    if (cat == null) continue;
                    grid.Append(PuntCategory(cat));
                }

                punt.Append(grid);
                body.Append(Row(_input.PuntingLabel, new List<HtmlTag> { punt }));
                any = true;
            }

            return any ? body : null;
        }

        private HtmlTag StandingsBody()
        {
            var body = new HtmlTag("div");
            var any = false;

            if (_input.ShowStandingsFormat)
            {
                body.Append(Row(_input.FormatLabel, new List<HtmlTag>
                {
                    Switch(Key("msroto"), "Roto", _input.ShowRotoStandings),
                    Switch(Key("msh2h"), "H2H", _input.ShowH2HStandings)
                }));
                any = true;
            }

            var lineups = new List<HtmlTag>();

            if (Has(_input.LineupPriorities))
                lineups.Add(Select(Key("mslineup"), _input.LineupPriorities, _input.SelectedLineupPriorityId));
            if (Has(_input.BenchHandling))
                lineups.Add(Select(Key("msbench"), _input.BenchHandling, _input.SelectedBenchHandlingId));

            if (lineups.Count > 0)
            {
                body.Append(Row(_input.LineupsLabel, lineups));
                any = true;
            }

            if (_input.ShowStandingsOptions)
            {
                body.Append(Row(_input.OptionsLabel, new List<HtmlTag>
                {
                    Switch(Key("msadvanced"), "Advanced standings", _input.UseAdvancedStandings),
                    Switch(Key("msgamelimits"), "Apply game limits", _input.ApplyGameLimits)
                }));
                any = true;
            }

            return any ? body : null;
        }

        private HtmlTag TableBody()
        {
            var body = new HtmlTag("div");
            var any = false;

            var stats = new List<HtmlTag>();

            if (Has(_input.StatsDisplayFormats))
                stats.Add(Select(Key("msstatsformat"), _input.StatsDisplayFormats, _input.SelectedStatsDisplayFormatId));
            if (Has(_input.ValueConsistencies))
                stats.Add(Select(Key("msvaluec"), _input.ValueConsistencies, _input.SelectedValueConsistencyId));
            if (_input.ShowMonsterBarToggle)
                stats.Add(Switch(Key("msmonsterbar"), "Show Monster Bar", _input.ShowMonsterBar));

            if (stats.Count > 0)
            {
                body.Append(Row(_input.StatsFormatLabel, stats));
                any = true;
            }

            var shown = new List<HtmlTag>();

            if (Has(_input.PlayerFilters))
                shown.Add(Select(Key("msfilter"), _input.PlayerFilters, _input.SelectedPlayerFilterId));
            if (Has(_input.Teams))
                shown.Add(Select(Key("msteam"), _input.Teams, _input.SelectedTeamId));
            if (Has(_input.HomeAwayOptions))
                shown.Add(Select(Key("mshomeaway"), _input.HomeAwayOptions, _input.SelectedHomeAwayId));

            if (shown.Count > 0)
            {
                body.Append(Row(_input.PlayersShownLabel, shown));
                any = true;
            }

            if (Has(_input.Positions))
            {
                body.Append(Row(_input.PositionsLabel, new List<HtmlTag> { PositionFilter() }));
                any = true;
            }

            if (_input.ShowColumnsRow)
            {
                var columns = new HtmlTag("div").AddClass("ms-controls");

                if (!string.IsNullOrEmpty(_input.ColumnsUrl) && !_input.ColumnsPostsBack)
                {
                    columns.Append(new HtmlTag("a")
                        .AddClass("ms-btn")
                        .Attr("href", _input.ColumnsUrl)
                        .Text(_input.ColumnsButtonText));
                }
                else
                {
                    var name = Key("mscolumns");

                    var button = new HtmlTag("button")
                        .AddClass("ms-btn")
                        .Attr("type", "button")
                        .Attr("name", name)
                        .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                        .Text(_input.ColumnsOpen
                            ? _input.ColumnsCloseText
                            : _input.ColumnsButtonText);

                    if (_input.ColumnsOpen) button.AddClass("ms-btn--on");

                    columns.Append(button);
                }

                if (!string.IsNullOrEmpty(_input.ColumnsSummary))
                    columns.Append(new HtmlTag("span").AddClass("ms-hint").Text(_input.ColumnsSummary));

                body.Append(Row(_input.ColumnsLabel, new List<HtmlTag> { columns }));

                // The pickers sit in their own full-width row under the button
                // rather than inside it, so a wide picker is not squeezed into
                // the controls column.
                if (_input.ColumnsOpen)
                {
                    var picker = new HtmlTag("div").AddClass("ms-columns-picker");
                    var anyPicker = false;

                    if (_input.ColumnsInput != null)
                    {
                        picker.AppendHtml(new DisplayColumns(_input.ColumnsInput).Render());
                        anyPicker = true;
                    }

                    if (_input.CustomValuesInput != null)
                    {
                        picker.Append(new HtmlTag("div")
                            .AddClass("ms-columns-values")
                            .AppendHtml(new CustomValues(_input.CustomValuesInput).Render()));
                        anyPicker = true;
                    }

                    if (!string.IsNullOrEmpty(_input.ColumnsHtml))
                    {
                        picker.AppendHtml(_input.ColumnsHtml);
                        anyPicker = true;
                    }

                    if (anyPicker)
                        body.Append(new HtmlTag("div")
                            .AddClass("ms-row ms-row--full").Append(picker));
                }

                any = true;
            }

            return any ? body : null;
        }

        // ---- pieces --------------------------------------------------------

        private HtmlTag PuntCategory(MonsterPuntCategory cat)
        {
            var cell = new HtmlTag("span").AddClass("ms-punt");

            var boxId = SubKey("mspunt", cat.CategoryId);

            var label = new HtmlTag("label").AddClass("ms-punt-cat").Attr("for", boxId);

            var box = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("id", boxId)
                .Attr("name", boxId)
                .Attr("value", "1");

            if (cat.IsSelected) box.Attr("checked", "checked");

            label.Append(box);
            label.Append(new HtmlTag("span").AddClass("ms-punt-abbr").Text(cat.Abbreviation));

            cell.Append(label);

            cell.Append(new HtmlTag("input")
                .AddClass("ms-input ms-punt-weight")
                .Attr("type", "text")
                .Attr("name", SubKey("mspuntw", cat.CategoryId))
                .Attr("placeholder", "1")
                .Attr("aria-label", "Weight for " + cat.Abbreviation)
                .Attr("value", cat.Weight ?? ""));

            return cell;
        }

        private HtmlTag PositionFilter()
        {
            var wrap = new HtmlTag("div").AddClass("ms-positions");

            var all = new HtmlTag("button")
                .AddClass("ms-pos ms-pos--all")
                .Attr("type", "button")
                .Attr("data-ms-all", "1")
                .Text("All");

            if (_input.AllPositionsSelected) all.AddClass("ms-pos--on");
            wrap.Append(all);

            foreach (var pos in _input.Positions)
            {
                if (pos == null) continue;

                var boxId = SubKey("mspos", pos.PositionId);

                var label = new HtmlTag("label").AddClass("ms-pos").Attr("for", boxId);

                if (!string.IsNullOrEmpty(pos.ColorCSS))
                    label.Attr("style", "--ms-pos-color:" + pos.ColorCSS + ";");

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

        private HtmlTag Panel(string name, string heading, string when, HtmlTag body, bool expanded)
        {
            var panel = new HtmlTag("div").AddClass("ms-panel");

            var toggleName = SubKey("mstoggle", name);

            var head = new HtmlTag("button")
                .AddClass("ms-panel-head")
                .Attr("type", "button")
                .Attr("name", toggleName)
                .Attr("aria-expanded", expanded ? "true" : "false")
                .Attr("onclick", "__doPostBack('" + toggleName + "','',this.form)");

            head.Append(new HtmlTag("span").AddClass("ms-caret").AppendHtml("&#9662;"));
            head.Append(new HtmlTag("span").AddClass("ms-panel-title").Text(heading));

            if (!string.IsNullOrEmpty(when))
                head.Append(new HtmlTag("span").AddClass("ms-panel-when").Text(when));

            panel.Append(head);

            var state = new HtmlTag("input")
                .Attr("type", "checkbox")
                .AddClass("ms-state")
                .Attr("name", SubKey("msopen", name))
                .Attr("value", "1")
                .Attr("hidden", "hidden");

            if (expanded) state.Attr("checked", "checked");
            panel.Append(state);

            var wrapper = new HtmlTag("div").AddClass("ms-panel-body");
            if (!expanded) wrapper.Attr("hidden", "hidden");
            wrapper.Append(body);

            panel.Append(wrapper);

            return panel;
        }

        private static bool Has<T>(List<T> list)
        {
            return list != null && list.Count > 0;
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

            foreach (var option in options)
            {
                if (option == null) continue;
                dropdown.AddItem(option.Text, option.Value);
            }

            if (!string.IsNullOrEmpty(selected))
                dropdown.WithSelectedValue(selected);

            return new HtmlTag("div").AddClass("ms-select-wrap").AppendHtml(dropdown.Render());
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

        private string Script()
        {
            var scope = "#monster-settings-" + _input.Id;

            return @"
(function () {
    var root = document.querySelector('" + scope + @"');
    if (!root) return;

    var wrap = root.querySelector('.ms-positions');
    if (!wrap) return;

    var all = wrap.querySelector('[data-ms-all]');
    var boxes = Array.prototype.slice.call(wrap.querySelectorAll('input[type=checkbox]'));

    function sync() {
        var any = boxes.some(function (b) { return b.checked; });
        if (all) all.classList.toggle('ms-pos--on', !any);
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