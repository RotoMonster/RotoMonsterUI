using System.Collections.Generic;
using System.Globalization;
using HtmlTags;

namespace RotoMonsterUI
{
    public class PuntCategoryControl
    {
        private readonly PuntCategoryInput _input;

        public PuntCategoryControl(PuntCategoryInput input)
        {
            _input = input;
        }

        public static string WeightPrefix(string controlId)
        {
            return $"{controlId}_weight_";
        }

        public static string WeightName(string controlId, int categoryId)
        {
            return WeightPrefix(controlId) + categoryId;
        }

        public string Render()
        {
            var wrapper = new HtmlTag("div")
                .AddClass("modern-filter-badges punt-category-control")
                .Attr("id", _input.Id);

            foreach (var category in _input.Categories)
            {
                var isSelected = _input.SelectedIds.Contains(category.Id);
                var checkboxId = $"{_input.Id}_cat_{category.Id}";

                var cell = new HtmlTag("div").AddClass("punt-category-cell");

                var label = new HtmlTag("label")
                    .AddClass("badge-checkbox")
                    .Attr("for", checkboxId);

                var checkbox = new HtmlTag("input")
                    .Attr("type", "checkbox")
                    .Attr("id", checkboxId)
                    .Attr("name", $"cat_{category.Id}")
                    .Attr("value", category.Id.ToString());

                if (isSelected)
                    checkbox.Attr("checked", "checked");

                var badgeLabel = new HtmlTag("span")
                    .AddClass("badge-label modern-filter-badge")
                    .Attr("data-cat", category.Abbreviation)
                    .Attr("style", $"--cat-color:{category.ColorCSS};color:{category.ColorCSS};")
                    .Text(category.Abbreviation);

                label.Append(checkbox);
                label.Append(badgeLabel);
                cell.Append(label);

                var weightName = WeightName(_input.Id, category.Id);

                var weight = new HtmlTag("input")
                    .AddClass("punt-category-weight")
                    .Attr("type", "number")
                    .Attr("id", weightName)
                    .Attr("name", weightName)
                    .Attr("step", "0.01")
                    .Attr("min", "0")
                    .Attr("placeholder", "1.00")
                    .Attr("title", $"Weight for {category.Abbreviation}");

                decimal? existing;
                if (_input.Weights != null
                    && _input.Weights.TryGetValue(category.Id, out existing)
                    && existing.HasValue)
                {
                    weight.Attr("value", existing.Value.ToString("0.00", CultureInfo.InvariantCulture));
                }

                cell.Append(weight);
                wrapper.Append(cell);
            }

            return wrapper.ToString();
        }
    }
}