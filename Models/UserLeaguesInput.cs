using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class UserLeagueRow
    {
        public string ProviderLeagueId { get; set; }
        public string UserLeagueId { get; set; }
        public string Title { get; set; }
        public string MyTeamTitle { get; set; }
        public bool IsImported { get; set; }
        public bool IsTracked { get; set; }
        public bool NotAtProvider { get; set; }
        public string EditUrl { get; set; }
    }

    public class UserLeagueConnectField
    {
        public string FieldName { get; set; }
        public string Placeholder { get; set; }
        public bool IsPassword { get; set; }
    }

    public class UserLeaguesTab
    {
        public string ProviderName { get; set; }
        public bool IsConnected { get; set; }
        public bool IsCustom { get; set; }
        public bool SupportsBulkImport { get; set; } = true;
        public List<UserLeagueRow> Leagues { get; set; } = new List<UserLeagueRow>();

        public string NotConnectedText { get; set; } = "Not connected";
        public string ConnectLead { get; set; }

        public List<UserLeagueConnectField> ConnectFields { get; set; }
            = new List<UserLeagueConnectField>();
        public string ConnectLinkUrl { get; set; }
        public string ConnectLinkText { get; set; }
        public string ConnectHelpHtml { get; set; }

        public string ErrorMessage { get; set; }
        public bool NeedsReauthorization { get; set; }

        public bool ShowManualEntry { get; set; }
        public string ManualEntryHeading { get; set; } = "Import using a league ID";
        public string ManualEntryHelpHtml { get; set; }
        public string ManualEntryPlaceholder { get; set; } = "League ID";
    }

    public class UserLeagueImportResultRow
    {
        public string Title { get; set; }
        public string ProviderLeagueId { get; set; }
        public bool Imported { get; set; }
        public bool Skipped { get; set; }
        public string Message { get; set; }
        public int MissingPlayerCount { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }

    public class UserLeaguesInput
    {
        public string Id { get; set; }
        public string Heading { get; set; } = "Your Leagues";

        public List<UserLeaguesTab> Tabs { get; set; } = new List<UserLeaguesTab>();
        public string SelectedTab { get; set; }

        public List<UserLeagueImportResultRow> ImportResults { get; set; }
        public string Message { get; set; }

        public bool ShowCreateCustom { get; set; } = true;
        public string CreateCustomHeading { get; set; } = "Create a custom league";
        public string CreateCustomLead { get; set; }
            = "Use this if you don't play on a connected provider. It creates a "
              + "league with default settings that you can then edit.";
        public string CreateCustomButtonText { get; set; } = "Create league";

        public string ImportButtonText { get; set; } = "Import selected";
        public string SelectAllText { get; set; } = "Select all";
        public string ClearAllText { get; set; } = "Clear all";
        public string DisconnectText { get; set; } = "Disconnect";
        public string ConnectText { get; set; } = "Connect";
    }
}