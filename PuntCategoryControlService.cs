using System.Collections.Generic;
using System.Globalization;

namespace RotoMonsterUI
{
    public class PuntCategoryControlResult
    {
        public List<int> SelectedIds { get; set; } = new List<int>();

        /// <summary>Weight per category id. Blank prompts come back as 1.0.</summary>
        public Dictionary<int, decimal> Weights { get; set; } = new Dictionary<int, decimal>();
    }

    public class PuntCategoryControlService
    {
        public PuntCategoryControlResult Process(string controlId, List<int> categoryIds, Dictionary<string, string> formValues)
        {
            var result = new PuntCategoryControlResult();

            foreach (var id in categoryIds)
            {
                if (formValues.ContainsKey($"cat_{id}"))
                    result.SelectedIds.Add(id);

                var weight = 1.0m;
                string raw;

                if (formValues.TryGetValue(PuntCategoryControl.WeightName(controlId, id), out raw)
                    && !string.IsNullOrWhiteSpace(raw))
                {
                    decimal parsed;
                    if (decimal.TryParse(raw.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
                        weight = decimal.Round(parsed, 2);
                }

                result.Weights[id] = weight;
            }

            return result;
        }
    }
}