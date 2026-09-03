using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class MonsterSettingsResult
    {
        public string SelectedDateRangeId { get; set; }
        public string SelectedProjectionSourceId { get; set; }
        public string SelectedValueTypeId { get; set; }
        public bool RestOfSeason { get; set; }
        public bool ReplacementPlayers { get; set; }
        public bool AssumeGoodHealth { get; set; }

        public List<string> PuntCategoryIds { get; set; } = new List<string>();
        public Dictionary<string, string> PuntWeights { get; set; } = new Dictionary<string, string>();

        public bool ShowRotoStandings { get; set; }
        public bool ShowH2HStandings { get; set; }
        public string SelectedLineupPriorityId { get; set; }
        public string SelectedBenchHandlingId { get; set; }
        public bool UseAdvancedStandings { get; set; }
        public bool ApplyGameLimits { get; set; }

        public string SelectedStatsDisplayFormatId { get; set; }
        public string SelectedValueConsistencyId { get; set; }
        public bool ShowMonsterBar { get; set; }
        public string SelectedPlayerFilterId { get; set; }
        public string SelectedTeamId { get; set; }
        public string SelectedHomeAwayId { get; set; }

        public List<string> PositionIds { get; set; } = new List<string>();
        public bool AllPositionsSelected { get; set; }

        public bool ShowTierColumn { get; set; }
        public bool ColorByTier { get; set; }

        public bool ValuesExpanded { get; set; }
        public bool StandingsExpanded { get; set; }
        public bool TableExpanded { get; set; }
        public bool ColumnsPressed { get; set; }

        public bool ColumnsOpen { get; set; }

        public DisplayColumnsResult Columns { get; set; }

        public CustomValuesResult CustomValues { get; set; }
    }
}