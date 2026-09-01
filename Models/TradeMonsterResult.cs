using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class TradeMonsterResult
    {
        public TradeMonsterTask SelectedTask { get; set; }
        public bool TaskChanged { get; set; }

        public List<string> MyTeamPlayerIds { get; set; } = new List<string>();
        public List<string> OtherTeamPlayerIds { get; set; } = new List<string>();
        public List<string> FreeAgentPlayerIds { get; set; } = new List<string>();

        public string SelectedMyTeamValue { get; set; }
        public string SelectedOtherTeamValue { get; set; }
        public string SelectedFreeAgentCountValue { get; set; }

        public bool GoPressed { get; set; }
        public bool ClearPressed { get; set; }
    }
}