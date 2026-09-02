using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum TradeMonsterTask
    {
        CheckTrade,
        FindTrade,
        CheckAddDrop,
        FindFreeAgent
    }

    public enum TradeMonsterBoard
    {
        MyTeam,
        OtherTeam,
        FreeAgents
    }

    public class TradeMonsterStep
    {
        public string Text { get; set; }
        public bool IsDone { get; set; }
        public bool IsOptional { get; set; }
    }

    public class TradeMonsterPlayer
    {
        public string PlayerId { get; set; }
        public DisplayPlayerInput DisplayPlayerInput { get; set; }
        public string ValueText { get; set; }
        public string GamesText { get; set; }
        public string MonsterBarHtml { get; set; }
        public InjuryBadgeInput InjuryBadge { get; set; }
        public bool IsSelected { get; set; }
    }

    public class TradeMonsterBoardInput
    {
        public TradeMonsterBoard Board { get; set; }
        public string Title { get; set; }
        public string ActionText { get; set; }
        public string SelectedActionText { get; set; }
        public string ActionColorCSS { get; set; }
        public List<TradeMonsterPlayer> Players { get; set; } = new List<TradeMonsterPlayer>();
        public List<MonsterOption> TeamOptions { get; set; }
        public string SelectedTeamValue { get; set; }
        public string TeamPlaceholder { get; set; } = "Any team";
        public string EmptyText { get; set; }
        public string CountText { get; set; }
        public int MaxSelectable { get; set; }
    }

    public class TradeMonsterTaskOption
    {
        public TradeMonsterTask Task { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class TradeMonsterInput
    {
        public string Id { get; set; }

        public List<TradeMonsterTaskOption> Tasks { get; set; } = new List<TradeMonsterTaskOption>();
        public TradeMonsterTask SelectedTask { get; set; } = TradeMonsterTask.CheckTrade;

        public string StepsHeading { get; set; }
        public List<TradeMonsterStep> Steps { get; set; } = new List<TradeMonsterStep>();
        public string GoButtonText { get; set; } = "Analyze";
        public bool GoEnabled { get; set; }

        public bool ShowSelectionSummary { get; set; } = true;
        public string LeavingLabel { get; set; } = "Leaving your team";
        public string JoiningLabel { get; set; } = "Joining your team";
        public string NothingSelectedText { get; set; } = "nobody yet";
        public string ClearButtonText { get; set; } = "Clear";

        public List<TradeMonsterBoardInput> Boards { get; set; } = new List<TradeMonsterBoardInput>();

        public string SettingsHtml { get; set; }
        public string ResultsHtml { get; set; }

        public string Message { get; set; }
    }
}