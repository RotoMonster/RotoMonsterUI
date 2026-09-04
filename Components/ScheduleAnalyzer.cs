using System;
using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class ScheduleAnalyzer
    {
        private readonly ScheduleAnalyzerInput _input;

        public ScheduleAnalyzer(ScheduleAnalyzerInput input)
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

        private string SelectedEasePosition
        {
            get
            {
                var pos = _input.EasePositionFilterValue;
                if (string.IsNullOrEmpty(pos) || pos.Equals("all", StringComparison.OrdinalIgnoreCase)) return null;
                return pos;
            }
        }

        private double EaseFor(ScheduleAnalyzerTeam team)
        {
            var pos = SelectedEasePosition;
            if (pos != null && team.EaseByPosition != null && team.EaseByPosition.TryGetValue(pos, out var posEase))
                return posEase;
            return team.Ease;
        }

        private double EaseFor(ScheduleAnalyzerDay day)
        {
            if (day == null) return 0;
            var pos = SelectedEasePosition;
            if (pos != null && day.EaseByPosition != null && day.EaseByPosition.TryGetValue(pos, out var posEase))
                return posEase;
            return day.Ease;
        }

        private bool ShowQuality => _input.ShowQualityColumns;
        private bool ShowCategories => _input.ShowCategoryColumns && _input.CategoryLabels.Count > 0;
        private bool ShowDays => _input.ShowDayColumns && _input.DayColumns.Count > 0;

        private bool HasQuality => true;
        private bool HasCategories => _input.CategoryLabels.Count > 0;
        private bool HasDays => _input.DayColumns.Count > 0;

        private string RosterRowId(string teamCode)
        {
            return "sa-roster-" + _input.Id + "-" + teamCode;
        }

        private bool IsExpanded(string teamCode)
        {
            return _input.ExpandedTeamCodes != null
                && _input.ExpandedTeamCodes.Any(c => string.Equals(c, teamCode, StringComparison.OrdinalIgnoreCase));
        }

        private int ColumnCount()
        {
            var count = 2;
            count += 2;
            if (ShowQuality) count += 2;
            count += 1;
            if (ShowCategories) count += _input.CategoryLabels.Count;
            if (HasDays) count += _input.DayColumns.Count;
            return count;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("schedule-analyzer");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", "schedule-analyzer-" + _input.Id);
            wrap.AddClass("sa-ease-" + EaseDisplaySlug());

            if (!_input.ColorNumbers) wrap.AddClass("sa-colors-off");

            if (!ShowQuality) wrap.AddClass("sa-hide-qg");
            if (!ShowCategories) wrap.AddClass("sa-hide-cats");
            if (!ShowDays) wrap.AddClass("sa-hide-days");

            wrap.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("id", Key("sarange"))
                .Attr("name", Key("sarange"))
                .Attr("value", _input.SelectedRangeKey ?? ""));

            wrap.Append(RenderRangeBar());

            if (_input.ShowColumnToggles) wrap.Append(RenderColumnToggles());

            if (!string.IsNullOrEmpty(_input.SettingsHtml))
                wrap.AppendHtml(_input.SettingsHtml);

            if (_input.Teams == null || _input.Teams.Count == 0)
            {
                wrap.Append(new HtmlTag("div").AddClass("sa-empty").Text(_input.EmptyText));
                return wrap.ToString();
            }

            wrap.Append(RenderTable());
            wrap.Append(new HtmlTag("script").AppendHtml(ColumnScript()));

            return wrap.ToString();
        }

        private string EaseDisplaySlug()
        {
            switch (_input.EaseDisplay)
            {
                case ScheduleAnalyzerEaseDisplay.Dot: return "dot";
                case ScheduleAnalyzerEaseDisplay.Background: return "bg";
                case ScheduleAnalyzerEaseDisplay.Outline: return "outline";
                case ScheduleAnalyzerEaseDisplay.Text: return "text";
                case ScheduleAnalyzerEaseDisplay.None: return "none";
                default: return "badge";
            }
        }

        private HtmlTag RenderRangeBar()
        {
            var bar = new HtmlTag("div").AddClass("sa-daybar");

            if (_input.Ranges != null && _input.Ranges.Count > 0)
            {
                var presets = new HtmlTag("div").AddClass("sa-presets");

                foreach (var range in _input.Ranges)
                {
                    if (range == null || string.IsNullOrEmpty(range.Key)) continue;
                    presets.Append(RangeButton(range));
                }

                bar.Append(presets);
            }

            if (!string.IsNullOrEmpty(_input.CalendarHtml))
            {
                var calendar = new HtmlTag("div").AddClass("sa-calwrap");
                calendar.AppendHtml(_input.CalendarHtml);
                bar.Append(calendar);
            }

            if (!string.IsNullOrEmpty(_input.RangeText))
                bar.Append(new HtmlTag("span").AddClass("sa-day").Text(_input.RangeText));

            if (!string.IsNullOrEmpty(_input.RangeCountText))
                bar.Append(new HtmlTag("span").AddClass("sa-count").Text(_input.RangeCountText));

            bar.Append(new HtmlTag("span").AddClass("sa-spacer"));

            if (_input.TeamOptions != null && _input.TeamOptions.Count > 0)
            {
                var dropdown = new Dropdown("Team")
                    .WithName(Key("sateam"))
                    .WithSelectedValue(_input.SelectedTeamValue);

                foreach (var option in _input.TeamOptions)
                    dropdown.AddItem(option.Text, option.Value);

                bar.AppendHtml(dropdown.Render());
            }

            if (_input.ShowAnalyzeButton)
                bar.AppendHtml(new Button(_input.AnalyzeButtonText)
                    .WithStyle(ButtonStyle.Primary)
                    .WithName(Key("saanalyze"))
                    .WithPostBack()
                    .Render());

            return bar;
        }

        private HtmlTag RangeButton(ScheduleAnalyzerRange range)
        {
            var name = RowKey("sarangepick", range.Key);
            var selected = string.Equals(range.Key, _input.SelectedRangeKey, StringComparison.OrdinalIgnoreCase);

            return new HtmlTag("button")
                .AddClass("sa-preset")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("aria-pressed", selected ? "true" : "false")
                .Attr("data-sa-range", range.Key)
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                .Text(range.Text ?? range.Key);
        }

        private HtmlTag RenderTable()
        {
            var wrapper = new HtmlTag("div").AddClass("sa-table-wrapper");
            var table = new HtmlTag("table").AddClass("sa-table");

            var thead = new HtmlTag("thead");
            thead.Append(RenderGroupRow());
            thead.Append(RenderColumnRow());
            table.Append(thead);

            var tbody = new HtmlTag("tbody");

            foreach (var team in SortTeams(_input.Teams))
            {
                tbody.Append(RenderTeamRow(team));

                if (!_input.ShowRosterRows) continue;

                var rosterRow = RenderRosterRow(team);
                rosterRow.Attr("id", RosterRowId(team.TeamCode));
                if (!IsExpanded(team.TeamCode)) rosterRow.Attr("hidden", "hidden");
                tbody.Append(rosterRow);
            }

            table.Append(tbody);
            wrapper.Append(table);

            return wrapper;
        }

        private List<ScheduleAnalyzerTeam> SortTeams(List<ScheduleAnalyzerTeam> teams)
        {
            switch (_input.SortBy)
            {
                case ScheduleAnalyzerSortBy.Ease:
                    return teams.OrderByDescending(EaseFor).ToList();
                case ScheduleAnalyzerSortBy.Games:
                    return teams.OrderByDescending(t => t.Games).ThenBy(t => t.TeamCode).ToList();
                case ScheduleAnalyzerSortBy.Team:
                    return teams.OrderBy(t => t.TeamCode).ToList();
                case ScheduleAnalyzerSortBy.GamesThenEase:
                default:
                    return teams.OrderByDescending(t => t.Games).ThenByDescending(EaseFor).ToList();
            }
        }

        private HtmlTag RenderGroupRow()
        {
            var row = new HtmlTag("tr").AddClass("sa-groups");

            row.Append(new HtmlTag("th").AddClass("sa-team-col"));

            row.Append(new HtmlTag("th")
                .AddClass("sa-sep")
                .Attr("colspan", "3")
                .Text(_input.ScheduleGroupText));

            if (HasQuality)
                row.Append(new HtmlTag("th")
                    .AddClass("sa-sep-none")
                    .Attr("data-sa-col", "qg")
                    .Attr("colspan", "2"));

            row.Append(new HtmlTag("th").AddClass("sa-sep").Text(_input.OverallGroupText));

            if (HasCategories)
                row.Append(new HtmlTag("th")
                    .AddClass("sa-sep")
                    .Attr("data-sa-col", "cats")
                    .Attr("colspan", _input.CategoryLabels.Count.ToString())
                    .Text(_input.CategoryGroupText));

            if (HasDays)
                row.Append(new HtmlTag("th")
                    .AddClass("sa-sep")
                    .Attr("data-sa-col", "days")
                    .Attr("colspan", _input.DayColumns.Count.ToString())
                    .Text(_input.RangeText ?? ""));

            return row;
        }

        private HtmlTag RenderColumnRow()
        {
            var row = new HtmlTag("tr").AddClass("sa-cols");

            row.Append(new HtmlTag("th").AddClass("sa-team-col").Text(_input.TeamHeaderText));

            row.Append(new HtmlTag("th").AddClass("sa-sep sa-num").Text(_input.GamesHeaderText));
            row.Append(new HtmlTag("th").AddClass("sa-num").Text(_input.HomeHeaderText));
            row.Append(new HtmlTag("th").AddClass("sa-num").Text(_input.AwayHeaderText));

            if (HasQuality)
            {
                row.Append(HeaderWithTooltip(_input.QualityHeaderText, _input.QualityTooltip)
                    .Attr("data-sa-col", "qg"));
                row.Append(HeaderWithTooltip(_input.BackToBackHeaderText, _input.BackToBackTooltip)
                    .Attr("data-sa-col", "qg"));
            }

            row.Append(new HtmlTag("th").AddClass("sa-sep sa-num").Text(_input.EaseHeaderText));

            if (HasCategories)
            {
                for (var i = 0; i < _input.CategoryLabels.Count; i++)
                {
                    var th = new HtmlTag("th").AddClass("sa-num")
                        .Attr("data-sa-col", "cats")
                        .Text(_input.CategoryLabels[i]);
                    if (i == 0) th.AddClass("sa-sep");
                    row.Append(th);
                }
            }

            if (HasDays)
            {
                for (var i = 0; i < _input.DayColumns.Count; i++)
                {
                    var column = _input.DayColumns[i];
                    var th = new HtmlTag("th").AddClass("sa-mid").Attr("data-sa-col", "days");
                    if (i == 0) th.AddClass("sa-sep");

                    th.Append(new HtmlTag("span").Text(column.Label ?? column.Date.ToString("ddd")));

                    if (column.GameCount > 0)
                        th.Append(new HtmlTag("span")
                            .AddClass("sa-day-count")
                            .Text(column.GameCount.ToString()));

                    row.Append(th);
                }
            }

            return row;
        }

        private static HtmlTag HeaderWithTooltip(string text, string tooltip)
        {
            var th = new HtmlTag("th").AddClass("sa-num");

            if (string.IsNullOrEmpty(tooltip))
            {
                th.Text(text);
                return th;
            }

            th.AppendHtml(new CustomTooltip(
                new HtmlTag("span").AddClass("sa-abbr").Text(text).ToString(),
                tooltip).Render());

            return th;
        }

        private HtmlTag RenderTeamRow(ScheduleAnalyzerTeam team)
        {
            var row = new HtmlTag("tr").AddClass("sa-row");

            row.Append(RenderTeamCell(team));

            var games = new HtmlTag("td").AddClass("sa-sep sa-num sa-games");
            games.Append(new HtmlTag("b").Text(team.Games.ToString()));

            if (team.BackToBacks > 0)
                games.AppendHtml(new CustomTooltip(
                    "<span class='sa-b2b-dot'></span>",
                    _input.BackToBackTooltip).Render());

            row.Append(games);

            row.Append(new HtmlTag("td").AddClass("sa-num sa-plain").Text(team.HomeGames.ToString()));
            row.Append(new HtmlTag("td").AddClass("sa-num sa-plain").Text(team.AwayGames.ToString()));

            if (HasQuality)
            {
                row.Append(CountCell(team.QualityGames).Attr("data-sa-col", "qg"));
                row.Append(CountCell(team.BackToBacks).Attr("data-sa-col", "qg"));
            }

            row.Append(EaseCell(EaseFor(team), true));

            if (HasCategories)
            {
                for (var i = 0; i < _input.CategoryLabels.Count; i++)
                {
                    var value = i < team.CategoryEase.Count ? team.CategoryEase[i] : 0;
                    var cell = EaseCell(value, false).Attr("data-sa-col", "cats");
                    if (i == 0) cell.AddClass("sa-sep");
                    row.Append(cell);
                }
            }

            if (HasDays)
            {
                for (var i = 0; i < _input.DayColumns.Count; i++)
                {
                    var cell = RenderDayCell(team, _input.DayColumns[i])
                        .Attr("data-sa-col", "days");
                    if (i == 0) cell.AddClass("sa-sep");
                    row.Append(cell);
                }
            }

            return row;
        }

        private HtmlTag RenderTeamCell(ScheduleAnalyzerTeam team)
        {
            var cell = new HtmlTag("td").AddClass("sa-team-col");

            if (_input.ShowRosterRows)
            {
                var expanded = IsExpanded(team.TeamCode);
                var rowId = RosterRowId(team.TeamCode);

                var button = new HtmlTag("button")
                    .AddClass("sa-rowtog")
                    .Attr("type", "button")
                    .Attr("aria-expanded", expanded ? "true" : "false")
                    .Attr("aria-controls", rowId)
                    .Attr("aria-label", "Show players")
                    .Attr("onclick", "rmScheduleAnalyzerToggle(this,'" + rowId + "')")
                    .Text(expanded ? "\u25BE" : "\u25B8");

                cell.Append(button);
            }

            var code = new HtmlTag("span").AddClass("sa-team-code").Text(team.TeamCode);

            var color = TeamColor(team);
            if (!string.IsNullOrEmpty(color))
                code.Attr("style", "--team-color:" + color + ";");

            cell.Append(code);

            if (team.MyPlayers != null && team.MyPlayers.Count > 0)
                cell.Append(new HtmlTag("span")
                    .AddClass("sa-mine-count")
                    .Attr("title", _input.MyPlayersText)
                    .Text(team.MyPlayers.Count.ToString()));

            return cell;
        }

        private string TeamColor(ScheduleAnalyzerTeam team)
        {
            if (!string.IsNullOrEmpty(team.TeamColor))
            {
                var color = team.TeamColor;
                if (color.StartsWith("var(") || color.StartsWith("#")) return color;
                return "#" + color;
            }

            return _input.Sport == GameSport.Basketball
                ? TeamColorHelper.GetNbaTeamColorVar(team.TeamCode)
                : TeamColorHelper.GetTeamColorVar(team.TeamCode);
        }

        private static HtmlTag CountCell(int value)
        {
            var cell = new HtmlTag("td").AddClass("sa-num");

            if (value <= 0)
            {
                cell.AddClass("sa-zero").Text("\u2013");
                return cell;
            }

            cell.Text(value.ToString());
            return cell;
        }

        private HtmlTag EaseCell(double value, bool isOverall)
        {
            var cell = new HtmlTag("td").AddClass("sa-num");

            if (!_input.ColorNumbers)
            {
                cell.AddClass("sa-plain").Text(value.ToString("0.00"));
                return cell;
            }

            if (Math.Abs(value) < 0.005)
            {
                cell.AddClass("sa-zero").Text(value.ToString("0.00"));
                return cell;
            }

            if (isOverall)
            {
                cell.AppendHtml(new EaseBadge(value).WithLabel(value.ToString("0.00")).Render());
                return cell;
            }

            cell.AddClass("sa-cat").Attr("style", EaseBadge.StyleFor(value)).Text(value.ToString("0.00"));
            return cell;
        }

        private HtmlTag RenderDayCell(ScheduleAnalyzerTeam team, ScheduleAnalyzerDayColumn column)
        {
            var cell = new HtmlTag("td").AddClass("sa-mid sa-opp");

            var day = team.Days == null
                ? null
                : team.Days.FirstOrDefault(d => d.Date.Date == column.Date.Date);

            if (day == null || string.IsNullOrEmpty(day.Opponent))
            {
                cell.AddClass("sa-off").Text("\u2013");
                return cell;
            }

            var ease = EaseFor(day);

            if (_input.ColorNumbers)
            {
                cell.AddClass("sa-has");
                cell.Attr("style", EaseBadge.StyleFor(ease));
            }

            var badge = new EaseBadge(ease)
                .WithLabel(day.Opponent)
                .WithTitle((day.IsAwayGame ? "at " : "vs ") + day.Opponent + " \u2014 ease " + ease.ToString("0.00"));

            if (day.IsAwayGame) badge.WithAway();
            if (day.IsQualityGame) badge.WithQuality();

            cell.Append(new HtmlTag("span").AddClass("sa-dot"));
            cell.AppendHtml(badge.Render());

            return cell;
        }

        private HtmlTag RenderRosterRow(ScheduleAnalyzerTeam team)
        {
            var row = new HtmlTag("tr").AddClass("sa-roster");

            var cell = new HtmlTag("td").Attr("colspan", ColumnCount().ToString());
            var grid = new HtmlTag("div").AddClass("sa-rgrid");

            grid.Append(PlayerColumn(
                _input.MyPlayersText,
                team.MyPlayers,
                string.Format(_input.NoMyPlayersText, team.TeamCode)));

            grid.Append(PlayerColumn(
                _input.AvailablePlayersText,
                team.AvailablePlayers,
                _input.NoAvailablePlayersText));

            cell.Append(grid);
            row.Append(cell);

            return row;
        }

        private static HtmlTag PlayerColumn(string heading, List<ScheduleAnalyzerPlayer> players, string emptyText)
        {
            var column = new HtmlTag("div").AddClass("sa-rcol");
            column.Append(new HtmlTag("h4").Text(heading));

            var list = new HtmlTag("div").AddClass("sa-plist");

            if (players == null || players.Count == 0)
            {
                list.Append(new HtmlTag("span").AddClass("sa-hint").Text(emptyText));
                column.Append(list);
                return column;
            }

            foreach (var player in players)
            {
                if (player == null) continue;

                var pill = new HtmlTag("span").AddClass("sa-pill");

                if (!string.IsNullOrEmpty(player.Html))
                    pill.AppendHtml(player.Html);
                else
                    pill.Text(player.Name ?? "");

                list.Append(pill);
            }

            column.Append(list);
            return column;
        }

        private HtmlTag RenderColumnToggles()
        {
            var row = new HtmlTag("div").AddClass("sa-coltoggles");

            row.Append(new HtmlTag("span").AddClass("sa-coltoggles-label").Text(_input.ColumnsLabel));

            if (HasQuality)
                row.Append(ColumnSwitch("saqg", "qg", _input.QualityToggleText, ShowQuality));

            if (HasCategories)
                row.Append(ColumnSwitch("sacats", "cats", _input.CategoryToggleText, ShowCategories));

            if (HasDays)
                row.Append(ColumnSwitch("sadays", "days", _input.DayToggleText, ShowDays));

            return row;
        }

        private HtmlTag ColumnSwitch(string prefix, string col, string text, bool on)
        {
            var name = Key(prefix);
            var label = new HtmlTag("label").AddClass("ms-switch").Attr("for", name);

            var box = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("id", name)
                .Attr("name", name)
                .Attr("value", "1")
                .Attr("data-sa-toggle", col);

            if (on) box.Attr("checked", "checked");

            label.Append(box);
            label.Append(new HtmlTag("span").AddClass("ms-track"));
            label.Append(new HtmlTag("span").Text(text));

            return label;
        }

        private string ColumnScript()
        {
            var scope = "#schedule-analyzer-" + _input.Id;

            return @"
(function () {
    var root = document.querySelector('" + scope + @"');
    if (!root) return;

    root.querySelectorAll('[data-sa-toggle]').forEach(function (box) {
        box.addEventListener('change', function () {
            root.classList.toggle('sa-hide-' + box.getAttribute('data-sa-toggle'),
                !box.checked);
        });
    });

})();
";
        }
    }
}