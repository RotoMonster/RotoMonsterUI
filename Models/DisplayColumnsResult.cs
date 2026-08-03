using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class DisplayColumnsResult
    {
        public List<string> SelectedColumnIds { get; set; } = new List<string>();
        public bool ApplyPressed { get; set; }
        public bool ResetPressed { get; set; }
    }
}