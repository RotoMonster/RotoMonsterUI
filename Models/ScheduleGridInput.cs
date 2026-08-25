using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum ScheduleGridColorType
    {
        MaxWeeks,
        Ease,
        QualityGames
    }

    public enum ScheduleGridSortBy
    {
        Team,
        Games,
        MaxWeeks,
        Ease,
        QualityGames
    }

    public class ScheduleGridDay
    {
        public DateTime Date { get; set; }
        public string Opponent { get; set; }
        public string EaseColor { get; set; }
        public bool IsQualityGame { get; set; }
        public bool IsAwayGame { get; set; }
    }

    public class ScheduleGridPeriodCell
    {
        public int Games { get; set; }

        /// <summary>Quality games in this period. If left at 0, the grid falls back to counting Days where IsQualityGame is true.</summary>
        public int QualityGames { get; set; }
        public bool IsMaxWeek { get; set; }
        public string EaseColor { get; set; }
        public double Ease { get; set; }
        public List<ScheduleGridDay> Days { get; set; } = new List<ScheduleGridDay>();
    }

    public class ScheduleGridPeriod
    {
        public int PeriodNumber { get; set; }
        public DateTime StartDate { get; set; }
        public int NumWeeks { get; set; } = 1;
    }

    public class ScheduleGridTeamSummary
    {
        public int Games { get; set; }
        public int MaxWeeks { get; set; }
        public int QualityGames { get; set; }
        public double AvgEase { get; set; }
        public string AvgEaseColor { get; set; }
    }

    public class ScheduleGridTeam
    {
        public string TeamCode { get; set; }
        public string TeamColor { get; set; }
        public Dictionary<int, ScheduleGridPeriodCell> Periods { get; set; } = new Dictionary<int, ScheduleGridPeriodCell>();
        public ScheduleGridTeamSummary Summary { get; set; } = new ScheduleGridTeamSummary();
    }

    public class ScheduleGridInput
    {
        public string Id { get; set; } = "schedule-grid";
        public List<ScheduleGridPeriod> Periods { get; set; } = new List<ScheduleGridPeriod>();
        public List<ScheduleGridTeam> Teams { get; set; } = new List<ScheduleGridTeam>();
        public DateTime? SelectedDate { get; set; }
        public int StartSelectedPeriod { get; set; }
        public int EndSelectedPeriod { get; set; }
        public ScheduleGridColorType ColorType { get; set; } = ScheduleGridColorType.MaxWeeks;
        public ScheduleGridSortBy SortBy { get; set; } = ScheduleGridSortBy.Team;
        /// <summary>Sport supports quality games at all. Gates the toggle, the sort option and the coloring option.</summary>
        public bool UseQualityGames { get; set; } = false;

        /// <summary>User toggle state. Controls the Quality Games summary row and the small count beside each cell's total.</summary>
        public bool ShowQualityGames { get; set; } = true;
        public bool ShowEasePositionFilter { get; set; } = false;
        public List<(string Text, string Value)> EasePositionOptions { get; set; } = new List<(string, string)>();
        public string EasePositionFilterValue { get; set; }
        /// <summary>Single expanded period. Kept for callers that only ever expand one; ExpandedPeriodNumbers is the general form and the two are unioned.</summary>
        public int? ExpandedPeriodNumber { get; set; }

        /// <summary>Every period currently expanded. Any number of periods can be open at once.</summary>
        public List<int> ExpandedPeriodNumbers { get; set; } = new List<int>();
    }
}