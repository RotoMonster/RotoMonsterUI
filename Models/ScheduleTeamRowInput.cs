using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class ScheduleTeamRowInput
    {
        public string Id { get; set; } = "schedule-team-row";
        public string TeamCode { get; set; }

        public List<ScheduleGridPeriod> Periods { get; set; } = new List<ScheduleGridPeriod>();

        // Key = PeriodNumber
        public Dictionary<int, ScheduleGridPeriodCell> PeriodCells { get; set; } = new Dictionary<int, ScheduleGridPeriodCell>();

        public ScheduleGridColorType ColorType { get; set; } = ScheduleGridColorType.MaxWeeks;

        public bool ShowQualityGames { get; set; } = false;
        public int? CurrentPeriodNumber { get; set; }

        public int? ExpandedPeriodNumber { get; set; }
    }
}