using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class NbaStartingLineupsInput
    {
        public string Id { get; set; } = "nbaLineups";

        public string DayText { get; set; }
        public string CountText { get; set; }
        public bool ShowPreviousDay { get; set; } = true;
        public bool ShowNextDay { get; set; } = true;
        public string RefreshButtonText { get; set; } = "Refresh";

        public bool ShowMyPlayersToggle { get; set; } = true;
        public string MyPlayersToggleText { get; set; } = "Only my players";
        public bool MyPlayersOnly { get; set; }

        public bool ShowBenchToggle { get; set; } = true;
        public string BenchToggleText { get; set; } = "Show bench";
        public bool ShowBench { get; set; }

        public bool ShowMinutesToggle { get; set; } = true;
        public string MinutesToggleText { get; set; } = "Projected minutes";
        public bool ShowProjectedMinutes { get; set; } = true;

        public List<NbaLineupCardInput> Games { get; set; } = new List<NbaLineupCardInput>();

        public int Columns { get; set; } = 2;

        public string EmptyText { get; set; } = "No games today.";
        public string Message { get; set; }
    }
}