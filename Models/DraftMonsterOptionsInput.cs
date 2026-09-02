using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class DraftMonsterOptionsInput
    {
        public string Id { get; set; }
        public string Heading { get; set; } = "Draft Monster";
        public string Description { get; set; }

        public List<MonsterOption> ProjectionSources { get; set; } = new List<MonsterOption>();
        public string SelectedProjectionSourceId { get; set; }
        public List<MonsterOption> ValueTypes { get; set; } = new List<MonsterOption>();
        public string SelectedValueTypeId { get; set; }
        public bool ReplacementPlayers { get; set; }
        public bool AssumeGoodHealth { get; set; }
        public List<MonsterPuntCategory> PuntCategories { get; set; } = new List<MonsterPuntCategory>();
        public string PuntHelpText { get; set; }
            = "Pick the categories you're giving up. Weight is optional, 1 is normal.";

        public bool ShowRotoStandings { get; set; }
        public bool ShowH2HStandings { get; set; }
        public bool UseAdvancedStandings { get; set; }
        public bool ApplyGameLimits { get; set; }

        public List<MonsterOption> StatsDisplayFormats { get; set; } = new List<MonsterOption>();
        public string SelectedStatsDisplayFormatId { get; set; }
        public List<MonsterOption> ValueConsistencies { get; set; } = new List<MonsterOption>();
        public string SelectedValueConsistencyId { get; set; }
        public List<MonsterOption> PlayerFilters { get; set; } = new List<MonsterOption>();
        public string SelectedPlayerFilterId { get; set; }
        public List<MonsterOption> Teams { get; set; } = new List<MonsterOption>();
        public string SelectedTeamId { get; set; }
        public List<MonsterPosition> Positions { get; set; } = new List<MonsterPosition>();
        public bool AllPositionsSelected { get; set; } = true;
        public string ColumnsButtonText { get; set; } = "Choose columns";
        public string ColumnsUrl { get; set; }

        /// <summary>
        /// Renders Choose columns as a button that posts back instead of a
        /// link, so the picker can open in place. Comes back as ColumnsPressed.
        /// </summary>
        public bool ColumnsPostsBack { get; set; }

        /// <summary>
        /// The column picker and the custom value builder. Pass the inputs and
        /// they are rendered under the Choose columns button, and read back for
        /// you into the result.
        /// </summary>
        public DisplayColumnsInput ColumnsInput { get; set; }

        public CustomValuesInput CustomValuesInput { get; set; }

        /// <summary>Raw html, for a picker with no component yet.</summary>
        public string ColumnsHtml { get; set; }

        public bool ColumnsOpen { get; set; }
        public string ColumnsSummary { get; set; }

        public string PickNumber { get; set; }
        public bool ThirdRoundReversal { get; set; }
        public bool SecondRoundHighToLow { get; set; }
        public bool FifthRoundReversal { get; set; }
        public bool IsConnected { get; set; }
        public string ConnectedStatusHtml { get; set; }
        public string ConnectButtonText { get; set; } = "Connect";
        public string ConnectHeading { get; set; } = "Connect your draft";
        public string ConnectLead { get; set; }
            = "Picks come in from your draft room automatically. Enter the slot you're "
              + "drafting from so Draft Monster knows when you're up.";

        public bool HideDraftedPlayers { get; set; } = true;
        public bool HighlightDraftedSinceImport { get; set; }
        public bool IncludeTargetsInAnalysis { get; set; }
        public bool ShowStatFilters { get; set; }

        /// <summary>
        /// Pass this and the extension notice renders above the connect panel,
        /// so someone without the extension is told before they try to use it.
        /// Leave it null and nothing shows.
        /// </summary>
        public ExtensionDetectInput ExtensionDetect { get; set; }

        /// <summary>
        /// Emits the hidden field the extension gates on. It only injects its
        /// Import from ESPN button on a page that has this, so rendering it
        /// here is what makes the button appear.
        /// </summary>
        public bool ShowDraftPicksField { get; set; }

        public string DraftPicksFieldId { get; set; } = "espnDraftPicks";

        /// <summary>
        /// Posts back on its own when the extension delivers picks, rather than
        /// the user having to press something. The extension writes the field
        /// then fires rm-draft-imported, so this listens for that.
        /// </summary>
        public bool DraftPicksAutoPostBack { get; set; } = true;

        public string StandingsHtml { get; set; }
        public string TeamAnalysisHtml { get; set; }
        public string ResultsHtml { get; set; }

        public string StandingsHeading { get; set; } = "Standings";
        public string TeamAnalysisHeading { get; set; } = "Team analysis";

        public bool StandingsExpanded { get; set; } = true;
        public bool TeamAnalysisExpanded { get; set; } = true;
        public bool StandingsCompact { get; set; } = true;
        public bool TeamAnalysisCompact { get; set; } = true;
        public string CompactText { get; set; } = "Just my team";
        public string FullText { get; set; } = "All teams";

        public bool ValuesExpanded { get; set; }
        public bool StandingsSettingsExpanded { get; set; }
        public bool TableSettingsExpanded { get; set; }

        public string Message { get; set; }
    }
}