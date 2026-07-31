using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum CustomValueType
    {
        PerGame,
        TotalGames
    }

    public enum CustomValueColumn
    {
        Rank,
        Games,
        MinutesPerGame
    }

    public class CustomValuesInput
    {
        public string Id { get; set; }

        public List<CustomValueOption> Options { get; set; } = new List<CustomValueOption>();

        public List<CustomValueEntry> Values { get; set; } = new List<CustomValueEntry>();

        public List<CustomValueEntry> DefaultValues { get; set; } = new List<CustomValueEntry>();

        public string SelectedOptionId { get; set; }

        public CustomValueType SelectedType { get; set; } = CustomValueType.PerGame;

        public List<CustomValueColumn> SelectedColumns { get; set; }
            = new List<CustomValueColumn> { CustomValueColumn.Rank };

        public string Message { get; set; }

        public bool ShowUseDefaults { get; set; } = true;

        public bool ShowDefaultOrder { get; set; } = true;

        public string AddButtonText { get; set; } = "Add value";

        public string EmptyText { get; set; }
            = "No values yet. Pick one above and press Add value.";
    }

    public class CustomValueOption
    {
        public string OptionId { get; set; }

        public string Name { get; set; }

        public bool AllowsTotalGames { get; set; } = true;

        public int DisplayOrder { get; set; }
    }

    public class CustomValueEntry
    {
        public string OptionId { get; set; }

        public CustomValueType Type { get; set; }

        public List<CustomValueColumn> Columns { get; set; } = new List<CustomValueColumn>();
    }
}