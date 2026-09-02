using System;
using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class ScheduleGrid
    {
        private readonly ScheduleGridInput _input;
        private ScheduleGridPeriod _selectedPeriod;

        public ScheduleGrid(ScheduleGridInput input)
        {
            _input = input;
        }

        private static readonly string DownArrowSvg =
            "<svg xmlns='http://www.w3.org/2000/svg' width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><polyline points='6 9 12 15 18 9'/></svg>";

        private static readonly string UpArrowSvg =
            "<svg xmlns='http://www.w3.org/2000/svg' width='11' height='11' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><polyline points='18 15 12 9 6 15'/></svg>";

        private static readonly string CurrentPeriodSvg =
            "<svg xmlns='http://www.w3.org/2000/svg' width='14' height='14' viewBox='0 0 24 24' fill='#22c55e'><polygon points='5 3 19 12 5 21 5 3'/></svg>";

        private static readonly string ExpandChevronSvg =
            "<svg xmlns='http://www.w3.org/2000/svg' width='10' height='10' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><polyline points='9 6 15 12 9 18'/></svg>";

        private static readonly string CollapseChevronSvg =
            "<svg xmlns='http://www.w3.org/2000/svg' width='10' height='10' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><polyline points='6 9 12 15 18 9'/></svg>";
        private int TeamCellColSpan => 4;

        private bool ShowQuality => _input.UseQualityGames && _input.ShowQualityGames;

        /// <summary>The position the user picked, or null when it's All / not in use.</summary>
        private string SelectedEasePosition
        {
            get
            {
                var pos = _input.EasePositionFilterValue;
                if (string.IsNullOrEmpty(pos) || pos.Equals("all", StringComparison.OrdinalIgnoreCase)) return null;
                return pos;
            }
        }

        private static string EaseToColor(double ease)
        {
            var e = (float)ease;
            return e >= 0
                ? ColorHelper.GetGreenColorCode(e, 0, 1, true)
                : ColorHelper.GetRedColorCode(-e, 0, 1, true);
        }

        /// <summary>Ease for a cell under the current position, falling back to the flat Ease when there's no per-position value.</summary>
        private double EaseValueFor(ScheduleGridPeriodCell cell)
        {
            if (cell == null) return 0;
            var pos = SelectedEasePosition;
            if (pos != null && cell.EaseByPosition != null && cell.EaseByPosition.TryGetValue(pos, out var posEase))
                return posEase;
            return cell.Ease;
        }

        /// <summary>Cell color under the current position. Generated here rather than taken from EaseColor, so the caller only supplies numbers.</summary>
        private string EaseColorFor(ScheduleGridPeriodCell cell)
        {
            if (cell == null) return null;
            var pos = SelectedEasePosition;
            if (pos != null && cell.EaseByPosition != null && cell.EaseByPosition.TryGetValue(pos, out var posEase))
                return EaseToColor(posEase);
            return cell.EaseColor;
        }

        private bool HasPositionEase(ScheduleGridTeam team)
        {
            var pos = SelectedEasePosition;
            if (pos == null) return false;
            return team.Periods.Values.Any(c => c.EaseByPosition != null && c.EaseByPosition.ContainsKey(pos));
        }

        private double AvgEaseFor(ScheduleGridTeam team)
        {
            if (!HasPositionEase(team)) return team.Summary.AvgEase;

            var inRange = team.Periods
                .Where(kv => kv.Key >= _input.StartSelectedPeriod && kv.Key <= _input.EndSelectedPeriod)
                .Select(kv => EaseValueFor(kv.Value))
                .ToList();

            return inRange.Count > 0 ? inRange.Average() : team.Summary.AvgEase;
        }

        private string AvgEaseColorFor(ScheduleGridTeam team)
        {
            return HasPositionEase(team) ? EaseToColor(AvgEaseFor(team)) : team.Summary.AvgEaseColor;
        }

        private bool IsPeriodExpanded(int periodNumber)
        {
            if (_input.ExpandedPeriodNumber.HasValue && _input.ExpandedPeriodNumber.Value == periodNumber) return true;
            return _input.ExpandedPeriodNumbers != null && _input.ExpandedPeriodNumbers.Contains(periodNumber);
        }

        /// <summary>
        /// Falls back to the team palette when no color was supplied, and accepts a hex with or
        /// without the leading #. TeamColorHelper returns one, so requiring bare hex silently
        /// dropped the color for anyone passing its output straight through.
        /// </summary>
        private string TeamHeaderColor(ScheduleGridTeam team)
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

        private static int QualityCount(ScheduleGridPeriodCell cell)
        {
            if (cell == null) return 0;
            if (cell.QualityGames > 0) return cell.QualityGames;
            return cell.Days.Count(d => d.IsQualityGame);
        }

        private ScheduleGridPeriod GetPeriodForSelectedDate()
        {
            if (!_input.SelectedDate.HasValue) return null;
            var date = _input.SelectedDate.Value.Date;

            foreach (var period in _input.Periods.OrderBy(p => p.PeriodNumber))
            {
                var start = period.StartDate.Date;
                var end = start.AddDays(period.NumWeeks * 7 - 1);
                if (date >= start && date <= end)
                    return period;
            }
            return null;
        }

        public string Render()
        {
            _selectedPeriod = GetPeriodForSelectedDate();

            var wrapper = new HtmlTag("div").AddClass("schedule-grid-wrapper").Attr("id", _input.Id);

            wrapper.Append(RenderControls());

            wrapper.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("name", $"{_input.Id}-current-start")
                .Attr("value", _input.StartSelectedPeriod.ToString()));

            wrapper.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("name", $"{_input.Id}-current-end")
                .Attr("value", _input.EndSelectedPeriod.ToString()));

            var tableWrapper = new HtmlTag("div").AddClass("schedule-grid-table-wrapper");
            var table = new HtmlTag("table").AddClass("schedule-grid");
            var tbody = new HtmlTag("tbody");

            var teams = SortTeams(_input.Teams);

            tbody.Append(RenderSummaryRow("Games", t => t.Summary.Games.ToString(), null));
            tbody.Append(RenderSummaryRow("Max Weeks", t => t.Summary.MaxWeeks.ToString(), null));

            if (ShowQuality)
                tbody.Append(RenderSummaryRow("Quality Games", t => t.Summary.QualityGames.ToString(), null));

            tbody.Append(RenderSummaryRow("Avg Ease", t => AvgEaseFor(t).ToString("0.00"), t => AvgEaseColorFor(t)));

            tbody.Append(RenderTeamHeaderRow(teams));

            foreach (var period in _input.Periods.OrderBy(p => p.PeriodNumber))
            {
                tbody.Append(RenderPeriodRow(period, teams));
                if (IsPeriodExpanded(period.PeriodNumber))
                {
                    foreach (var dayRow in RenderExpandedDayRows(period, teams))
                        tbody.Append(dayRow);
                }
            }

            tbody.Append(RenderTeamHeaderRow(teams));
            tbody.Append(RenderSummaryRow("Games", t => t.Summary.Games.ToString(), null));
            tbody.Append(RenderSummaryRow("Max Weeks", t => t.Summary.MaxWeeks.ToString(), null));

            if (ShowQuality)
                tbody.Append(RenderSummaryRow("Quality Games", t => t.Summary.QualityGames.ToString(), null));

            tbody.Append(RenderSummaryRow("Avg Ease", t => AvgEaseFor(t).ToString("0.00"), t => AvgEaseColorFor(t)));

            table.Append(tbody);
            tableWrapper.Append(table);
            wrapper.Append(tableWrapper);

            return wrapper.ToString();
        }

        private List<ScheduleGridTeam> SortTeams(List<ScheduleGridTeam> teams)
        {
            switch (_input.SortBy)
            {
                case ScheduleGridSortBy.Games:
                    return teams.OrderByDescending(t => t.Summary.Games).ToList();
                case ScheduleGridSortBy.MaxWeeks:
                    return teams.OrderByDescending(t => t.Summary.MaxWeeks).ToList();
                case ScheduleGridSortBy.Ease:
                    return teams.OrderByDescending(t => AvgEaseFor(t)).ToList();
                case ScheduleGridSortBy.QualityGames:
                    return teams.OrderByDescending(t => t.Summary.QualityGames).ThenBy(t => t.TeamCode).ToList();
                case ScheduleGridSortBy.Team:
                default:
                    return teams.OrderBy(t => t.TeamCode).ToList();
            }
        }

        private HtmlTag RenderControls()
        {
            var controls = new HtmlTag("div").AddClass("schedule-grid-controls");

            var coloringGroup = new HtmlTag("div").AddClass("schedule-grid-control-group");
            coloringGroup.Append(new HtmlTag("span").AddClass("schedule-grid-control-label").Text("Coloring"));
            var coloringRadio = new RadioGroup($"{_input.Id}-colortype")
                .WithPostBack()
                .WithSegmented()
                .AddOption("Max Weeks Coloring", ScheduleGridColorType.MaxWeeks.ToString(), _input.ColorType == ScheduleGridColorType.MaxWeeks)
                .AddOption("Ease Coloring", ScheduleGridColorType.Ease.ToString(), _input.ColorType == ScheduleGridColorType.Ease);
            if (_input.UseQualityGames)
                coloringRadio.AddOption("Quality Games Coloring", ScheduleGridColorType.QualityGames.ToString(), _input.ColorType == ScheduleGridColorType.QualityGames);
            coloringGroup.AppendHtml(coloringRadio.Render());

            var sortGroup = new HtmlTag("div").AddClass("schedule-grid-control-group");
            sortGroup.Append(new HtmlTag("span").AddClass("schedule-grid-control-label").Text("Sort By"));
            var sortRadio = new RadioGroup($"{_input.Id}-sortby")
                .WithPostBack()
                .WithSegmented();
            foreach (ScheduleGridSortBy option in Enum.GetValues(typeof(ScheduleGridSortBy)))
            {
                if (option == ScheduleGridSortBy.QualityGames && !_input.UseQualityGames) continue;
                sortRadio.AddOption(SortByLabel(option), option.ToString(), _input.SortBy == option);
            }
            sortGroup.AppendHtml(sortRadio.Render());

            controls.Append(coloringGroup);
            controls.Append(sortGroup);

            if (_input.UseQualityGames)
            {
                var qualityGroup = new HtmlTag("div").AddClass("schedule-grid-control-group");
                var qualityToggle = new ToggleSwitch()
                    .WithLabel("Quality Games")
                    .WithName($"{_input.Id}-showquality")
                    .WithChecked(_input.ShowQualityGames)
                    .WithPostBack();
                qualityGroup.AppendHtml(qualityToggle.Render());

                // Unchecked checkboxes are not submitted, so this marker tells the service the toggle was on the page
                qualityGroup.Append(new HtmlTag("input")
                    .Attr("type", "hidden")
                    .Attr("name", $"{_input.Id}-quality-present")
                    .Attr("value", "1"));

                controls.Append(qualityGroup);
            }

            if (_input.ShowEasePositionFilter && _input.EasePositionOptions.Count > 0)
            {
                var easePosGroup = new HtmlTag("div").AddClass("schedule-grid-control-group");
                easePosGroup.Append(new HtmlTag("span").AddClass("schedule-grid-control-label").Text("Ease Position"));
                var dropdown = new Dropdown("Ease Position")
                    .WithName($"{_input.Id}-ease-position")
                    .WithSelectedValue(_input.EasePositionFilterValue);
                foreach (var option in _input.EasePositionOptions)
                    dropdown.AddItem(option.Text, option.Value);
                easePosGroup.AppendHtml(dropdown.Render());
                controls.Append(easePosGroup);
            }

            return controls;
        }

        private static string SortByLabel(ScheduleGridSortBy sortBy)
        {
            switch (sortBy)
            {
                case ScheduleGridSortBy.MaxWeeks: return "Max Weeks";
                case ScheduleGridSortBy.QualityGames: return "Quality Games";
                default: return sortBy.ToString();
            }
        }

        private HtmlTag RenderTeamHeaderRow(List<ScheduleGridTeam> teams)
        {
            var row = new HtmlTag("tr");
            row.Append(new HtmlTag("td").AddClass("schedule-grid-header-blank").Attr("colspan", TeamCellColSpan.ToString()));

            foreach (var team in teams)
            {
                var td = new HtmlTag("td").AddClass("schedule-grid-team-header").Text(team.TeamCode);

                var headerColor = TeamHeaderColor(team);
                if (!string.IsNullOrEmpty(headerColor))
                    td.Attr("style", $"background-color:{headerColor}; color:#fff;");
                row.Append(td);
            }

            return row;
        }

        private HtmlTag RenderSummaryRow(string label, Func<ScheduleGridTeam, string> valueSelector, Func<ScheduleGridTeam, string> colorSelector)
        {
            var teams = SortTeams(_input.Teams);
            var row = new HtmlTag("tr").AddClass("schedule-grid-summary-row");
            row.Append(new HtmlTag("td").AddClass("schedule-grid-summary-label").Attr("colspan", TeamCellColSpan.ToString()).Text(label));

            foreach (var team in teams)
            {
                var cell = new HtmlTag("td").AddClass("schedule-grid-summary-cell");
                if (colorSelector != null)
                {
                    var color = colorSelector(team);
                    if (!string.IsNullOrEmpty(color))
                    {
                        cell.Attr("style", $"background-color:#{color};");
                        cell.AddClass("schedule-grid-cell-colored");
                    }
                }
                cell.Text(valueSelector(team));
                row.Append(cell);
            }

            return row;
        }

        private HtmlTag RenderPeriodRow(ScheduleGridPeriod period, List<ScheduleGridTeam> teams)
        {
            var isCurrent = _selectedPeriod != null && _selectedPeriod.PeriodNumber == period.PeriodNumber;
            var isRangeStart = period.PeriodNumber == _input.StartSelectedPeriod;
            var isRangeEnd = period.PeriodNumber == _input.EndSelectedPeriod;
            var isExpanded = IsPeriodExpanded(period.PeriodNumber);

            var row = new HtmlTag("tr").AddClass("schedule-grid-period-row");
            if (isRangeStart) row.AddClass("schedule-grid-range-start-row");
            if (isRangeEnd) row.AddClass("schedule-grid-range-end-row");
            if (period.IsPlayoffPeriod) row.AddClass("schedule-grid-playoff-row");

            var indicatorCell = new HtmlTag("td").AddClass("schedule-grid-indicator-cell");
            if (isCurrent) indicatorCell.AppendHtml(CurrentPeriodSvg);
            row.Append(indicatorCell);

            var arrowsCell = new HtmlTag("td").AddClass("schedule-grid-arrows-cell");
            var downBtn = new HtmlTag("button").AddClass("schedule-grid-arrow-btn")
                .Attr("type", "submit")
                .Attr("name", $"{_input.Id}-set-start")
                .Attr("value", period.PeriodNumber.ToString())
                .Attr("aria-label", "Set as start period");
            downBtn.AppendHtml(DownArrowSvg);
            var upBtn = new HtmlTag("button").AddClass("schedule-grid-arrow-btn")
                .Attr("type", "submit")
                .Attr("name", $"{_input.Id}-set-end")
                .Attr("value", period.PeriodNumber.ToString())
                .Attr("aria-label", "Set as end period");
            upBtn.AppendHtml(UpArrowSvg);
            arrowsCell.Append(CustomTooltip.Wrap(downBtn, "Set as start period", TooltipPlacement.Right));
            arrowsCell.Append(CustomTooltip.Wrap(upBtn, "Set as end period", TooltipPlacement.Right));
            row.Append(arrowsCell);

            var isInRange = period.PeriodNumber >= _input.StartSelectedPeriod && period.PeriodNumber <= _input.EndSelectedPeriod;
            var periodCell = new HtmlTag("td").AddClass("schedule-grid-period-cell");
            if (isInRange) periodCell.AddClass("schedule-grid-period-selected");
            periodCell.Text(period.PeriodNumber.ToString());
            if (period.NumWeeks > 1)
                periodCell.Append(new HtmlTag("span").AddClass("schedule-grid-multiweek-badge").Text($"{period.NumWeeks}w"));
            if (period.IsPlayoffPeriod)
                periodCell.AppendHtml(new CustomTooltip(
                    "<span class='schedule-grid-playoff-badge'>P</span>",
                    "Playoff Period").Render());
            row.Append(periodCell);

            var dateCell = new HtmlTag("td").AddClass("schedule-grid-date-cell");
            dateCell.AppendHtml(new DisplayDate(new DisplayDateInput { Date = period.StartDate }).Render());

            var hasDays = teams.Any(t => t.Periods.TryGetValue(period.PeriodNumber, out var c) && c.Days.Count > 0);
            if (hasDays)
            {
                var expandBtn = new HtmlTag("button")
                    .AddClass("schedule-grid-expand-btn")
                    .Attr("type", "submit")
                    .Attr("name", $"{_input.Id}-expand")
                    .Attr("value", isExpanded ? $"collapse-{period.PeriodNumber}" : period.PeriodNumber.ToString())
                    .Attr("aria-label", isExpanded ? "Collapse this period" : "Expand this period");
                expandBtn.AppendHtml(isExpanded ? CollapseChevronSvg : ExpandChevronSvg);
                dateCell.Append(CustomTooltip.Wrap(
                    expandBtn,
                    isExpanded ? "Collapse this period" : "Expand this period",
                    TooltipPlacement.Right));
            }

            row.Append(dateCell);

            foreach (var team in teams)
            {
                team.Periods.TryGetValue(period.PeriodNumber, out var cellData);
                row.Append(RenderTeamCell(cellData));
            }

            return row;
        }

        private HtmlTag RenderTeamCell(ScheduleGridPeriodCell cellData)
        {
            var cell = new HtmlTag("td").AddClass("schedule-grid-team-cell");

            if (cellData == null)
                return cell;

            if (_input.ColorType == ScheduleGridColorType.MaxWeeks)
            {
                if (cellData.IsMaxWeek)
                    cell.AddClass("schedule-grid-maxweek");
            }
            else if (_input.ColorType == ScheduleGridColorType.Ease)
            {
                var easeColor = EaseColorFor(cellData);
                if (!string.IsNullOrEmpty(easeColor))
                {
                    cell.Attr("style", $"background-color:#{easeColor};");
                    cell.AddClass("schedule-grid-cell-colored");
                }
            }
            else if (_input.ColorType == ScheduleGridColorType.QualityGames)
            {
                var shade = Math.Min(QualityCount(cellData), 3);
                if (shade > 0)
                    cell.AddClass($"schedule-grid-quality-{shade}");
            }

            cell.Append(new HtmlTag("span").AddClass("schedule-grid-team-games").Text(cellData.Games.ToString()));

            if (ShowQuality)
            {
                var quality = QualityCount(cellData);
                if (quality > 0)
                    cell.Append(new HtmlTag("span").AddClass("schedule-grid-quality-count").Text(quality.ToString()));
            }

            return cell;
        }

        private List<HtmlTag> RenderExpandedDayRows(ScheduleGridPeriod period, List<ScheduleGridTeam> teams)
        {
            var rows = new List<HtmlTag>();

            var allDates = teams
                .Select(t => t.Periods.TryGetValue(period.PeriodNumber, out var c) ? c : null)
                .Where(c => c != null && c.Days.Count > 0)
                .SelectMany(c => c.Days)
                .Select(d => d.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            foreach (var date in allDates)
            {
                var isSelected = _input.SelectedDate.HasValue && date == _input.SelectedDate.Value.Date;

                var row = new HtmlTag("tr").AddClass("schedule-grid-expanded-row");
                if (isSelected) row.AddClass("schedule-grid-expanded-row--selected");

                var dateCell = new HtmlTag("td")
                    .AddClass("schedule-grid-expanded-date-cell")
                    .Attr("colspan", TeamCellColSpan.ToString());
                if (isSelected) dateCell.AddClass("schedule-grid-expanded-cell--selected");
                dateCell.AppendHtml(new DisplayDate(new DisplayDateInput { Date = date, Format = "ddd M/d" }).Render());
                row.Append(dateCell);

                foreach (var team in teams)
                {
                    var cell = new HtmlTag("td").AddClass("schedule-grid-expanded-cell");
                    if (isSelected) cell.AddClass("schedule-grid-expanded-cell--selected");
                    team.Periods.TryGetValue(period.PeriodNumber, out var cellData);

                    var day = cellData?.Days.FirstOrDefault(d => d.Date.Date == date);
                    if (day != null && !string.IsNullOrEmpty(day.Opponent))
                    {
                        var dayDiv = new HtmlTag("div").AddClass("schedule-grid-day");
                        if (day.IsQualityGame) dayDiv.AddClass("schedule-grid-day-quality");

                        var oppSpan = new HtmlTag("span").AddClass("schedule-grid-day-opponent");
                        if (day.IsAwayGame)
                            oppSpan.AppendHtml("<span class='schedule-grid-day-away'>@</span>");
                        oppSpan.AppendHtml(day.Opponent);
                        if (!string.IsNullOrEmpty(day.EaseColor))
                            oppSpan.Attr("style", $"background-color:#{day.EaseColor}; color:#000;");

                        // Real element rather than a ::after, so it can carry its own tooltip
                        if (day.IsQualityGame)
                            oppSpan.AppendHtml("<span class='schedule-grid-day-star' title='Quality Game'>&#9733;</span>");

                        dayDiv.Append(oppSpan);

                        cell.Append(dayDiv);
                    }

                    row.Append(cell);
                }

                rows.Add(row);
            }

            return rows;
        }
    }
}