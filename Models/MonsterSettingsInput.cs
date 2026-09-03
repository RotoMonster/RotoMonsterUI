using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class MonsterSettingsInput
    {
        public string Id { get; set; }

        public bool ShowValuesPanel { get; set; } = true;
        public bool ShowStandingsPanel { get; set; } = true;
        public bool ShowTablePanel { get; set; } = true;

        public string ValuesPanelHeading { get; set; } = "How players are valued";
        public string StandingsPanelHeading { get; set; } = "How standings are worked out";
        public string TablePanelHeading { get; set; } = "What the table shows";

        public string SetOnceText { get; set; } = "Set once";
        public string AnyTimeText { get; set; } = "Change any time";

        public bool ValuesExpanded { get; set; }
        public bool StandingsExpanded { get; set; }
        public bool TableExpanded { get; set; }

        public List<MonsterOption> DateRanges { get; set; }
        public string SelectedDateRangeId { get; set; }
        public List<MonsterOption> ProjectionSources { get; set; }
        public string SelectedProjectionSourceId { get; set; }
        public List<MonsterOption> ValueTypes { get; set; }
        public string SelectedValueTypeId { get; set; }
        public bool ShowRestOfSeason { get; set; }
        public bool RestOfSeason { get; set; }

        public bool ShowAdjustments { get; set; } = true;
        public bool ReplacementPlayers { get; set; }
        public bool AssumeGoodHealth { get; set; }

        public string ReplacementPlayersText { get; set; } = "Use Replacement Players";
        public string AssumeGoodHealthText { get; set; } = "Assume Good Health";

        public List<MonsterPuntCategory> PuntCategories { get; set; }
        public string PuntHelpText { get; set; }
            = "Pick the categories you're giving up. Weight is optional, 1 is normal.";

        public bool ShowStandingsFormat { get; set; } = true;
        public bool ShowRotoStandings { get; set; }
        public bool ShowH2HStandings { get; set; }

        public List<MonsterOption> LineupPriorities { get; set; }
        public string SelectedLineupPriorityId { get; set; }
        public List<MonsterOption> BenchHandling { get; set; }
        public string SelectedBenchHandlingId { get; set; }

        public bool ShowStandingsOptions { get; set; } = true;
        public bool UseAdvancedStandings { get; set; }
        public bool ApplyGameLimits { get; set; }

        public List<MonsterOption> StatsDisplayFormats { get; set; }
        public string SelectedStatsDisplayFormatId { get; set; }
        public List<MonsterOption> ValueConsistencies { get; set; }
        public string SelectedValueConsistencyId { get; set; }
        public bool ShowMonsterBarToggle { get; set; }
        public bool ShowMonsterBar { get; set; }

        public List<MonsterOption> PlayerFilters { get; set; }
        public string SelectedPlayerFilterId { get; set; }
        public List<MonsterOption> Teams { get; set; }
        public string SelectedTeamId { get; set; }
        public List<MonsterOption> HomeAwayOptions { get; set; }
        public string SelectedHomeAwayId { get; set; }

        public List<MonsterPosition> Positions { get; set; }
        public bool AllPositionsSelected { get; set; } = true;

        public bool ShowTiersRow { get; set; }
        public bool ShowTierColumn { get; set; }
        public bool ColorByTier { get; set; }
        public string TiersLabel { get; set; } = "Tiers";
        public string ShowTierColumnText { get; set; } = "Show tier column";
        public string ColorByTierText { get; set; } = "Color rows by tier";

        public bool ShowColumnsRow { get; set; } = true;
        public string ColumnsButtonText { get; set; } = "Choose columns";
        public string ColumnsUrl { get; set; }
        public bool ColumnsPostsBack { get; set; }
        public string ColumnsSummary { get; set; }


        public DisplayColumnsInput ColumnsInput { get; set; }


        public CustomValuesInput CustomValuesInput { get; set; }


        public string ColumnsHtml { get; set; }

        public bool ColumnsOpen { get; set; }
        public string ColumnsCloseText { get; set; } = "Done";

        public string ProjectionsLabel { get; set; } = "Projections";
        public string AdjustmentsLabel { get; set; } = "Adjustments";
        public string PuntingLabel { get; set; } = "Punting";
        public string FormatLabel { get; set; } = "Format";
        public string LineupsLabel { get; set; } = "Lineups";
        public string OptionsLabel { get; set; } = "Options";
        public string StatsFormatLabel { get; set; } = "Stats format";
        public string PlayersShownLabel { get; set; } = "Players shown";
        public string PositionsLabel { get; set; } = "Positions";
        public string ColumnsLabel { get; set; } = "Columns";
    }
}