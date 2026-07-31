using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class CustomValues
    {
        private readonly CustomValuesInput _input;

        public CustomValues(CustomValuesInput input)
        {
            _input = input;
        }

        private string Key(string prefix)
        {
            return prefix + "_" + _input.Id;
        }

        private string RowKey(string prefix, int index)
        {
            return prefix + "_" + _input.Id + "_" + index;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("custom-values");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", "custom-values-" + _input.Id);
            wrap.Append(RenderCreateRow());

            if (!string.IsNullOrEmpty(_input.Message))
                wrap.Append(new HtmlTag("div")
                    .AddClass("custom-values-message")
                    .Text(_input.Message));

            wrap.Append(RenderList());

            var footer = RenderFooter();
            if (footer != null) wrap.Append(footer);

            return wrap.ToString();
        }

        private HtmlTag RenderCreateRow()
        {
            var row = new HtmlTag("div").AddClass("custom-values-create");

            AddField(row, "Value", RenderOptionPicker());
            AddField(row, "Type", RenderTypePicker());
            AddField(row, "Columns", RenderColumnPicker());

            var add = new HtmlTag("div").AddClass("custom-values-add");
            add.AppendHtml(new Button(_input.AddButtonText)
                .WithStyle(ButtonStyle.Primary)
                .WithName(Key("cvadd"))
                .WithPostBack()
                .Render());

            row.Append(new HtmlTag("span").AddClass("custom-values-label"));
            row.Append(add);

            return row;
        }

        private static void AddField(HtmlTag row, string label, HtmlTag control)
        {
            row.Append(new HtmlTag("span").AddClass("custom-values-label").Text(label));
            row.Append(control);
        }

        private HtmlTag RenderOptionPicker()
        {
            var dropdown = new Dropdown("Select a value")
                .WithName(Key("cvvalue"));

            foreach (var option in _input.Options ?? new List<CustomValueOption>())
            {
                if (option == null) continue;
                dropdown.AddItem(option.Name, option.OptionId);
            }

            if (!string.IsNullOrEmpty(_input.SelectedOptionId))
                dropdown.WithSelectedValue(_input.SelectedOptionId);

            var wrap = new HtmlTag("div").AddClass("custom-values-control");
            wrap.AppendHtml(dropdown.Render());

            var selected = SelectedOption();

            if (selected != null && !string.IsNullOrEmpty(selected.Description))
            {
                var icon = new HtmlTag("span")
                    .AddClass("custom-values-info")
                    .AppendHtml(new Icon(new IconInput { Type = IconType.Info, Size = 16 }).Render());

                wrap.AppendHtml(new CustomTooltip(icon.ToString(), selected.Description).Render());
            }

            return wrap;
        }

        private HtmlTag RenderTypePicker()
        {
            var group = new RadioGroup(Key("cvtype")).WithSegmented();
            var allowsTotal = SelectedAllowsTotal();

            group.AddOption("Per Game", "per",
                !allowsTotal || _input.SelectedType == CustomValueType.PerGame);

            if (allowsTotal)
                group.AddOption("Total Games", "total",
                    _input.SelectedType == CustomValueType.TotalGames);

            return new HtmlTag("div")
                .AddClass("custom-values-control")
                .AppendHtml(group.Render());
        }

        private HtmlTag RenderColumnPicker()
        {
            var badges = new HtmlTag("div")
                .AddClass("modern-filter-badges custom-values-control");

            badges.Append(ColumnBadge(CustomValueColumn.Rank, "rank", "Rank"));
            badges.Append(ColumnBadge(CustomValueColumn.Games, "games", "Games"));
            badges.Append(ColumnBadge(CustomValueColumn.MinutesPerGame, "mg", "m/g"));

            return badges;
        }

        private HtmlTag ColumnBadge(CustomValueColumn column, string slug, string text)
        {
            var selected = _input.SelectedColumns != null
                           && _input.SelectedColumns.Contains(column);

            var id = Key("cvcol") + "_" + slug;

            var label = new HtmlTag("label").AddClass("badge-checkbox").Attr("for", id);

            var check = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("id", id)
                .Attr("name", id)
                .Attr("value", "1");

            if (selected) check.Attr("checked", "checked");

            label.Append(check);
            label.Append(new HtmlTag("span")
                .AddClass("badge-label modern-filter-badge")
                .Text(text));

            return label;
        }

        private CustomValueOption SelectedOption()
        {
            var option = FindOption(_input.SelectedOptionId);
            if (option != null) return option;

            return (_input.Options ?? new List<CustomValueOption>()).FirstOrDefault();
        }

        private bool SelectedAllowsTotal()
        {
            var option = SelectedOption();
            return option == null || option.AllowsTotalGames;
        }

        private CustomValueOption FindOption(string optionId)
        {
            if (string.IsNullOrEmpty(optionId)) return null;
            return (_input.Options ?? new List<CustomValueOption>())
                .FirstOrDefault(o => o != null && o.OptionId == optionId);
        }

        private HtmlTag RenderList()
        {
            var list = new HtmlTag("div").AddClass("custom-values-list");
            var values = _input.Values ?? new List<CustomValueEntry>();

            if (!values.Any())
            {
                list.Append(new HtmlTag("div")
                    .AddClass("custom-values-empty")
                    .Text(_input.EmptyText));
                return list;
            }

            for (var i = 0; i < values.Count; i++)
            {
                var entry = values[i];
                if (entry == null) continue;

                var option = FindOption(entry.OptionId);

                var row = new HtmlTag("div").AddClass("custom-values-item");

                row.Append(new HtmlTag("span")
                    .AddClass("custom-values-item-name")
                    .Text(option != null ? option.Name : entry.OptionId));

                row.Append(new HtmlTag("span")
                    .AddClass("custom-values-item-type")
                    .Text(entry.Type == CustomValueType.TotalGames ? "Total games" : "Per game"));

                var cols = new HtmlTag("span").AddClass("custom-values-item-cols");
                foreach (var column in entry.Columns ?? new List<CustomValueColumn>())
                {
                    cols.Append(new HtmlTag("span")
                        .AddClass("custom-values-pill")
                        .Text(ColumnText(column)));
                }
                row.Append(cols);

                var actions = new HtmlTag("span").AddClass("custom-values-item-actions");
                actions.Append(RowButton("cvup", i, IconType.ArrowUp, "Move up", i == 0));
                actions.Append(RowButton("cvdown", i, IconType.ArrowDown, "Move down",
                    i == values.Count - 1));
                actions.Append(RowButton("cvremove", i, IconType.Trash, "Remove", false));
                row.Append(actions);

                list.Append(row);
            }

            return list;
        }

        private HtmlTag RowButton(string prefix, int index, IconType icon, string title, bool disabled)
        {
            var name = RowKey(prefix, index);

            var button = new HtmlTag("button")
                .AddClass("custom-values-icon-btn")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("title", title);

            if (disabled) button.Attr("disabled", "disabled");
            else button.Attr("onclick", "__doPostBack('" + name + "','',this.form)");

            button.AppendHtml(new Icon(new IconInput { Type = icon, Size = 15 }).Render());

            return button;
        }

        private static string ColumnText(CustomValueColumn column)
        {
            switch (column)
            {
                case CustomValueColumn.Games:
                    return "Games";
                case CustomValueColumn.MinutesPerGame:
                    return "m/g";
                default:
                    return "Rank";
            }
        }

        private HtmlTag RenderFooter()
        {
            if (!_input.ShowUseDefaults && !_input.ShowDefaultOrder) return null;

            var footer = new HtmlTag("div").AddClass("custom-values-footer");

            if (_input.ShowUseDefaults)
                footer.AppendHtml(new Button("Use defaults")
                    .WithStyle(ButtonStyle.Secondary)
                    .WithName(Key("cvdefaults"))
                    .WithPostBack()
                    .Render());

            if (_input.ShowDefaultOrder)
                footer.AppendHtml(new Button("Default order")
                    .WithStyle(ButtonStyle.Secondary)
                    .WithName(Key("cvorder"))
                    .WithPostBack()
                    .Render());

            return footer;
        }
    }
}