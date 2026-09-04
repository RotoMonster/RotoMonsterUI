using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum ScheduleAnalyzerSortBy
    {
        GamesThenEase,
        Ease,
        Games,
        Team
    }

    public enum ScheduleAnalyzerEaseDisplay
    {
        Badge,
        Dot,
        Background,
        Outline,
        Text,
        None
    }

    public class ScheduleAnalyzerRange
    {
        public string Key { get; set; }
        public string Text { get; set; }
    }

    public class ScheduleAnalyzerDayColumn
    {
        public DateTime Date { get; set; }
        public string Label { get; set; }
        public int GameCount { get; set; }
    }

    public class ScheduleAnalyzerDay
    {
        public DateTime Date { get; set; }
        public string Opponent { get; set; }
        public bool IsAwayGame { get; set; }
        public bool IsQualityGame { get; set; }
        public double Ease { get; set; }
        public Dictionary<string, double> EaseByPosition { get; set; }
    }

    public class ScheduleAnalyzerPlayer
    {
        public string Name { get; set; }
        public string Html { get; set; }
    }

    public class ScheduleAnalyzerTeam
    {
        public string TeamCode { get; set; }
        public string TeamColor { get; set; }
        public int Games { get; set; }
        public int HomeGames { get; set; }
        public int AwayGames { get; set; }
        public int QualityGames { get; set; }
        public int BackToBacks { get; set; }
        public double Ease { get; set; }
        public Dictionary<string, double> EaseByPosition { get; set; }
        public List<double> CategoryEase { get; set; } = new List<double>();
        public List<ScheduleAnalyzerDay> Days { get; set; } = new List<ScheduleAnalyzerDay>();
        public List<ScheduleAnalyzerPlayer> MyPlayers { get; set; } = new List<ScheduleAnalyzerPlayer>();
        public List<ScheduleAnalyzerPlayer> AvailablePlayers { get; set; } = new List<ScheduleAnalyzerPlayer>();
    }

    public class ScheduleAnalyzerInput
    {
        public string Id { get; set; }

        public GameSport Sport { get; set; } = GameSport.Basketball;

        public List<ScheduleAnalyzerTeam> Teams { get; set; } = new List<ScheduleAnalyzerTeam>();

        public List<ScheduleAnalyzerDayColumn> DayColumns { get; set; } = new List<ScheduleAnalyzerDayColumn>();

        public List<string> CategoryLabels { get; set; } = new List<string>();

        public List<ScheduleAnalyzerRange> Ranges { get; set; } = new List<ScheduleAnalyzerRange>();
        public string SelectedRangeKey { get; set; }
        public string RangeText { get; set; }
        public string RangeCountText { get; set; }

        public string CalendarHtml { get; set; }
        public string CustomRangeText { get; set; } = "Custom...";

        public string SettingsHtml { get; set; }

        public ScheduleAnalyzerSortBy SortBy { get; set; } = ScheduleAnalyzerSortBy.GamesThenEase;
        public ScheduleAnalyzerEaseDisplay EaseDisplay { get; set; } = ScheduleAnalyzerEaseDisplay.Badge;

        public bool ShowColumnToggles { get; set; } = true;
        public string ColumnsLabel { get; set; } = "Columns";
        public string QualityToggleText { get; set; } = "Quality games and back-to-backs";
        public string CategoryToggleText { get; set; } = "Ease by category";
        public string DayToggleText { get; set; } = "Daily opponents";

        public bool ShowQualityColumns { get; set; } = true;
        public bool ShowCategoryColumns { get; set; } = true;
        public bool ShowDayColumns { get; set; } = true;
        public bool ShowRosterRows { get; set; } = true;
        public bool ColorNumbers { get; set; } = true;

        public string EasePositionFilterValue { get; set; }
        public List<ScheduleAnalyzerTeamOption> TeamOptions { get; set; } = new List<ScheduleAnalyzerTeamOption>();
        public string SelectedTeamValue { get; set; }
        public string AnalyzeButtonText { get; set; } = "Analyze";
        public bool ShowAnalyzeButton { get; set; } = true;

        public List<string> ExpandedTeamCodes { get; set; } = new List<string>();

        public string TeamHeaderText { get; set; } = "Team";
        public string GamesHeaderText { get; set; } = "Games";
        public string HomeHeaderText { get; set; } = "Home";
        public string AwayHeaderText { get; set; } = "Away";
        public string QualityHeaderText { get; set; } = "QG";
        public string BackToBackHeaderText { get; set; } = "B2B";
        public string EaseHeaderText { get; set; } = "Ease";
        public string ScheduleGroupText { get; set; } = "Schedule";
        public string OverallGroupText { get; set; } = "Overall";
        public string CategoryGroupText { get; set; } = "Ease by category";

        public string QualityTooltip { get; set; } = "Quality games, against a bottom-ten defense";
        public string BackToBackTooltip { get; set; } = "Back-to-backs, second nights of two games in two days";

        public string MyPlayersText { get; set; } = "Your players";
        public string AvailablePlayersText { get; set; } = "Best available";
        public string NoMyPlayersText { get; set; } = "No one on your roster plays for {0}.";
        public string NoAvailablePlayersText { get; set; } = "Nothing available worth a pickup.";

        public string EmptyText { get; set; } = "No teams to show.";
    }

    public class ScheduleAnalyzerTeamOption
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}