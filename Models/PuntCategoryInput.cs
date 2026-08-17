using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class PuntCategoryInput
    {
        public string Id { get; set; }
        public List<ScoringCategory> Categories { get; set; } = new List<ScoringCategory>();
        public List<int> SelectedIds { get; set; } = new List<int>();
        public Dictionary<int, decimal?> Weights { get; set; } = new Dictionary<int, decimal?>();
    }
}