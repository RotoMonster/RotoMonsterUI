using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class UserLeaguesResult
    {
        public string SelectedTab { get; set; }
        public string TabChangedTo { get; set; }

        public string ConnectProvider { get; set; }
        public Dictionary<string, string> ConnectValues { get; set; }
            = new Dictionary<string, string>();
        public string DisconnectProvider { get; set; }

        public bool ImportPressed { get; set; }
        public string ImportProvider { get; set; }
        public List<string> SelectedLeagueIds { get; set; } = new List<string>();

        public string ImportLeagueProvider { get; set; }
        public string ImportLeagueId { get; set; }

        public string ManualEntryProvider { get; set; }
        public string ManualEntryLeagueId { get; set; }

        public string ToggleTrackUserLeagueId { get; set; }
        public string RemoveUserLeagueId { get; set; }

        public bool CreateCustomPressed { get; set; }
    }
}