using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class CustomValuesResult
    {
        public bool AddPressed { get; set; }

        public bool UseDefaultsPressed { get; set; }

        public bool DefaultOrderPressed { get; set; }

        public int? MoveUpIndex { get; set; }

        public int? MoveDownIndex { get; set; }

        public int? RemoveIndex { get; set; }

        public string SelectedOptionId { get; set; }

        public CustomValueType SelectedType { get; set; }

        public List<CustomValueColumn> SelectedColumns { get; set; }
            = new List<CustomValueColumn>();
    }
}