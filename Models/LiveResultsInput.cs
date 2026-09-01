using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum LiveResultsGameState
    {
        NotStarted,
        Live,
        Final
    }

    public class LiveResultsGame
    {
        public string GameId { get; set; }
        public string AwayTeamCode { get; set; }
        public string HomeTeamCode { get; set; }
        public string AwayScoreText { get; set; }
        public string HomeScoreText { get; set; }
        public LiveResultsGameState State { get; set; }
        public string StateText { get; set; }
        public int MyPlayerCount { get; set; }
        public string TooltipText { get; set; }
    }

    public class LiveResultsOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class LiveResultsInput
    {
        public string Id { get; set; }

        public string DayText { get; set; }
        public string CountText { get; set; }
        public bool ShowPreviousDay { get; set; } = true;
        public bool ShowNextDay { get; set; } = true;
        public string RefreshButtonText { get; set; } = "Refresh";

        public bool ShowMyPlayersToggle { get; set; } = true;
        public string MyPlayersToggleText { get; set; } = "Only my players";
        public bool MyPlayersOnly { get; set; }

        public List<LiveResultsGame> Games { get; set; } = new List<LiveResultsGame>();
        public string SelectedGameId { get; set; }
        public string AllGamesText { get; set; } = "All";

        public string SettingsHtml { get; set; }
        public string ContentHtml { get; set; }

        public string Message { get; set; }
    }
}