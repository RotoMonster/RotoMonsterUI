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

        /// <summary>
        /// A small block of buttons, one per default value that is not already
        /// in the list, e.g. "Add Bonus Per Game".
        ///
        /// Deliberately NOT part of Render - Ken's mockup has these sitting
        /// above the component next to the Edit Display and Value Columns
        /// button rather than inside it, so the caller places them.
        ///
        /// Returns an empty string when every default is already added, so it
        /// can be written out unconditionally.
        /// </summary>
        public string RenderDefaults()
        {
            if (_input == null) return "";

            var defaults = _input.DefaultValues ?? new List<CustomValueEntry>();
            if (!defaults.Any()) return "";

            var wrap = new HtmlTag("div").AddClass("custom-values-defaults");
            var any = false;

            for (var i = 0; i < defaults.Count; i++)
            {
                var entry = defaults[i];
                if (entry == null) continue;
                if (AlreadyAdded(entry)) continue;

                var option = FindOption(entry.OptionId);
                var name = option != null ? option.Name : entry.OptionId;

                var typeText = entry.Type == CustomValueType.TotalGames ? "Total Games" : "Per Game";
                var text = (_input.AddDefaultPrefix + " " + name + " " + typeText).Trim();
                var buttonName = RowKey("cvadddefault", i);

                var button = new HtmlTag("button")
                    .AddClass("custom-values-default-btn")
                    .Attr("type", "button")
                    .Attr("name", buttonName)
                    .Attr("onclick", "__doPostBack('" + buttonName + "','',this.form)")
                    .Text(text);

                wrap.Append(button);
                any = true;
            }

            return any ? wrap.ToString() : "";
        }

        /// <summary>
        /// A default counts as added when the same value is present with the
        /// same type. Columns are deliberately not compared - the user is free
        /// to change those after adding, and re-offering the button because
        /// they unticked Games would be wrong.
        /// </summary>
        private bool AlreadyAdded(CustomValueEntry entry)
        {
            return (_input.Values ?? new List<CustomValueEntry>())
                .Any(v => v != null
                          && v.OptionId == entry.OptionId
                          && v.Type == entry.Type);
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

                if (_input.ShowMainValue && entry.IsMainValue)
                    row.AddClass("custom-values-item-main");

                row.Append(new HtmlTag("span")
                    .AddClass("custom-values-item-name")
                    .Text(option != null ? option.Name : entry.OptionId));

                row.Append(new HtmlTag("span")
                    .AddClass("custom-values-item-type")
                    .Text(TypeText(entry.Type)));

                var cols = new HtmlTag("span").AddClass("custom-values-item-cols");
                foreach (var column in entry.Columns ?? new List<CustomValueColumn>())
                {
                    cols.Append(new HtmlTag("span")
                        .AddClass("custom-values-pill")
                        .Text(ColumnText(column)));
                }
                row.Append(cols);

                var actions = new HtmlTag("span").AddClass("custom-values-item-actions");

                if (_input.ShowMainValue)
                    actions.Append(MainButton(i, entry.IsMainValue));

                actions.Append(RowButton("cvup", i, IconType.ArrowUp, "Move up", i == 0));
                actions.Append(RowButton("cvdown", i, IconType.ArrowDown, "Move down",
                    i == values.Count - 1));
                actions.Append(RowButton("cvremove", i, IconType.Trash, "Remove", false));
                row.Append(actions);

                list.Append(row);
            }

            return list;
        }

        /// <summary>
        /// The main value control. Always clickable, including on the row that
        /// is already the main value - pressing it again is how the caller gets
        /// told to clear it, and a disabled button would give no way back.
        /// </summary>
        private HtmlTag MainButton(int index, bool isMain)
        {
            var name = RowKey("cvmain", index);

            var button = new HtmlTag("button")
                .AddClass("custom-values-icon-btn custom-values-main-btn")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("title", isMain ? "Main value" : "Set as main value")
                .Attr("onclick", "__doPostBack('" + name + "','',this.form)");

            if (isMain) button.AddClass("custom-values-main-on");

            button.AppendHtml(new Icon(new IconInput
            {
                Type = IconType.MainValue,
                Size = 15
            }).Render());

            return button;
        }

        private HtmlTag RowButton(string prefix, int index, IconType icon, string title, bool disabled)
        {
            var name = RowKey(prefix, index);

            var button = new HtmlTag("button")
                .AddClass("custom-values-icon-btn")
                .Attr("type", "button")
                .Attr("name", name)
                .Attr("aria-label", title);

            if (disabled) button.Attr("disabled", "disabled");
            else button.Attr("onclick", "__doPostBack('" + name + "','',this.form)");

            button.AppendHtml(new Icon(new IconInput { Type = icon, Size = 15 }).Render());

            // A disabled button gets no tooltip - it cannot be hovered in a way
            // that reads as interactive, and explaining a control you cannot use
            // is noise.
            if (disabled) return button;

            return CustomTooltip.Wrap(button, title, TooltipPlacement.Above);
        }

        private static string TypeText(CustomValueType type)
        {
            return type == CustomValueType.TotalGames ? "Total games" : "Per game";
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