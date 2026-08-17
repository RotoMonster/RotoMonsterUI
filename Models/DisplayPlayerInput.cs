using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class DisplayPlayerInput
    {
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public string TeamCode { get; set; }
        public string TeamColor { get; set; }

        public bool DisableLink { get; set; }

        public List<DisplayPosition> Positions { get; set; } = new List<DisplayPosition>();

        // Alternate names a player can be searched by 
        public List<string> Aliases { get; set; } = new List<string>();
    }
}