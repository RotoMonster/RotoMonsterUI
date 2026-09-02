using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class DraftMonsterOptionsResult
    {
        public string SelectedProjectionSourceId { get; set; }
        public string SelectedValueTypeId { get; set; }
        public bool ReplacementPlayers { get; set; }
        public bool AssumeGoodHealth { get; set; }
        public List<string> PuntCategoryIds { get; set; } = new List<string>();
        public Dictionary<string, string> PuntWeights { get; set; } = new Dictionary<string, string>();

        public bool ShowRotoStandings { get; set; }
        public bool ShowH2HStandings { get; set; }
        public bool UseAdvancedStandings { get; set; }
        public bool ApplyGameLimits { get; set; }

        public string SelectedStatsDisplayFormatId { get; set; }
        public string SelectedValueConsistencyId { get; set; }
        public string SelectedPlayerFilterId { get; set; }
        public string SelectedTeamId { get; set; }
        public string SelectedHomeAwayId { get; set; }
        public List<string> PositionIds { get; set; } = new List<string>();
        public bool AllPositionsSelected { get; set; }

        public string PickNumber { get; set; }
        public bool ThirdRoundReversal { get; set; }
        public bool SecondRoundHighToLow { get; set; }
        public bool FifthRoundReversal { get; set; }
        public bool ConnectPressed { get; set; }
        public bool ChangePickPressed { get; set; }

        public bool HideDraftedPlayers { get; set; }
        public bool HighlightDraftedSinceImport { get; set; }
        public bool IncludeTargetsInAnalysis { get; set; }
        public bool ShowStatFilters { get; set; }
        public bool RefreshPressed { get; set; }

        public bool StandingsExpanded { get; set; }
        public bool TeamAnalysisExpanded { get; set; }
        public bool ValuesExpanded { get; set; }
        public bool StandingsSettingsExpanded { get; set; }
        public bool TableSettingsExpanded { get; set; }

        public bool StandingsCompact { get; set; }
        public bool TeamAnalysisCompact { get; set; }

        public string ToggledSection { get; set; }
        public bool ColumnsPressed { get; set; }
    }
}