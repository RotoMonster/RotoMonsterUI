using System.Collections.Generic;
using System.Globalization;

namespace RotoMonsterUI
{
    public class PuntCategoryResult
    {
        public List<int> SelectedIds { get; set; } = new List<int>();

        /// <summary>Weight per category id. A blank prompt comes back as 1.0.</summary>
        public Dictionary<int, decimal> Weights { get; set; } = new Dictionary<int, decimal>();
    }

    public class PuntCategoryService
    {
        public PuntCategoryResult Process(string controlId, Dictionary<string, string> formValues)
        {
            var result = new PuntCategoryResult();

            foreach (var kvp in formValues)
            {
                if (kvp.Key.StartsWith("cat_") && int.TryParse(kvp.Value, out int id))
                    result.SelectedIds.Add(id);
            }

            var prefix = PuntCategoryControl.WeightPrefix(controlId);

            foreach (var kvp in formValues)
            {
                if (!kvp.Key.StartsWith(prefix)) continue;

                if (!int.TryParse(kvp.Key.Substring(prefix.Length), out int weightId)) continue;

                var weight = 1.0m;

                if (!string.IsNullOrWhiteSpace(kvp.Value)
                    && decimal.TryParse(kvp.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsed))
                {
                    weight = decimal.Round(parsed, 2);
                }

                result.Weights[weightId] = weight;
            }

            return result;
        }
    }
}