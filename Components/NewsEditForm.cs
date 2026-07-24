using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{

    public class NewsEditForm
    {
        private readonly NewsEditFormInput _input;

        public NewsEditForm(NewsEditFormInput input)
        {
            _input = input;
        }

        private string Key(string baseName)
        {
            return (_input.KeyPrefix ?? "") + baseName + "_" + _input.KeyId;
        }

        public string Render()
        {
            return RenderTag().ToString();
        }

        public HtmlTag RenderTag()
        {
            var form = new HtmlTag("div").AddClass("news-card-edit-form");

            if (!string.IsNullOrEmpty(_input.LeadingHtml))
                form.AppendHtml(_input.LeadingHtml);

            // Status
            var statusDropdown = new Dropdown("Status").WithName(Key("status")).WithoutPostBack();
            foreach (var opt in _input.StatusTypeOptions)
                statusDropdown.AddItem(opt, opt);
            statusDropdown.WithSelectedValue(_input.StatusTypeText);
            form.AppendHtml("<div class='news-card-field-row'><label>Status</label>" + statusDropdown.Render() + "</div>");

            // Tag + Set
            var tagDropdown = new Dropdown("Tag").WithName(Key("tag")).WithoutPostBack();
            foreach (var opt in _input.StatusTypeTagOptions)
                tagDropdown.AddItem(opt, opt);
            tagDropdown.WithSelectedValue(_input.StatusTypeTag);

            var tagRow = new HtmlTag("div").AddClass("news-card-field-row");
            tagRow.AppendHtml("<label>Tag</label>" + tagDropdown.Render());
            tagRow.AppendHtml(new Button("Set").WithStyle(ButtonStyle.Secondary).WithName(Key("settag")).Render());
            form.Append(tagRow);

            // Title box, labelled with the selected tag
            var titleBox = new TextBox()
                .WithName(Key("newstitle"))
                .WithValue(_input.NewsTitle)
                .Render();
            var titleLabel = string.IsNullOrEmpty(_input.StatusTypeTag) ? "Title" : _input.StatusTypeTag;
            form.AppendHtml("<div class='news-card-field-row'><label>" + titleLabel + "</label>" + titleBox + "</div>");

            // Source
            var sourceBox = new TextBox()
                .WithName(Key("source"))
                .WithValue(_input.SourceURL)
                .Render();
            form.AppendHtml("<div class='news-card-field-row'><label>Source</label>" + sourceBox + "</div>");

            // L / M / H / Monster
            var levelGroup = new RadioGroup(Key("level"));
            levelGroup.AddOption("L", "Low", _input.NewsLevel == NewsLevel.Low);
            levelGroup.AddOption("M", "Medium", _input.NewsLevel == NewsLevel.Medium);
            levelGroup.AddOption("H", "High", _input.NewsLevel == NewsLevel.High);
            levelGroup.AddOption("Monster", "Monster", _input.NewsLevel == NewsLevel.Monster);
            form.AppendHtml(levelGroup.Render());

            // Action buttons + Unofficial
            var buttonRow = new HtmlTag("div").AddClass("news-card-field-row");
            foreach (var button in _input.Buttons)
            {
                if (button == null || string.IsNullOrEmpty(button.Name)) continue;
                buttonRow.AppendHtml(new Button(button.Text).WithStyle(button.Style).WithName(button.Name).Render());
            }
            buttonRow.AppendHtml(new Checkbox()
                .WithLabel("Unofficial")
                .WithName(Key("unofficial"))
                .WithChecked(_input.IsUnofficial)
                .Render());
            form.Append(buttonRow);

            // Details
            form.AppendHtml(new TextArea(new TextAreaInput
            {
                Id = Key("newsdetails"),
                Name = Key("newsdetails"),
                Placeholder = "More details",
                InitialValue = _input.NewsDetails ?? ""
            }).Render());

            // Tag checkboxes
            var appliedIds = new HashSet<int>(_input.NewsTags != null
                ? _input.NewsTags.Select(t => t.Id)
                : System.Array.Empty<int>());

            var checkGrid = new HtmlTag("div").AddClass("news-card-tag-checkbox-grid");
            foreach (var opt in _input.AvailableNewsTags)
            {
                checkGrid.AppendHtml(new Checkbox()
                    .WithLabel(opt.Name)
                    .WithName(Key("newstag") + "_" + opt.Id)
                    .WithChecked(appliedIds.Contains(opt.Id))
                    .Render());
            }
            form.Append(checkGrid);

            return form;
        }
    }
}