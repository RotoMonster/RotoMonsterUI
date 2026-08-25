using HtmlTags;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class RadioGroup
    {
        private string _name;
        private List<(string label, string value, bool selected, string color)> _options;
        private string _id;
        private bool _autoPostBack = false;
        private bool _segmented = true;

        public RadioGroup(string name)
        {
            _name = name;
            _options = new List<(string, string, bool, string)>();
        }

        public RadioGroup WithId(string id)
        {
            _id = id;
            return this;
        }

        public RadioGroup WithPostBack()
        {
            _autoPostBack = true;
            return this;
        }

        public RadioGroup WithSegmented()
        {
            _segmented = true;
            return this;
        }

        public RadioGroup WithBubbles()
        {
            _segmented = false;
            return this;
        }

        public string Id => _id ?? _name ?? "";

        public RadioGroup AddOption(string label, string value, bool isSelected = false, string color = null)
        {
            _options.Add((label, value, isSelected, color));
            return this;
        }

        public string Render()
        {
            var wrapper = new HtmlTag("div").AddClass(_segmented ? "bm-segmented" : "bm-bubbles");

            if (!string.IsNullOrEmpty(_id))
                wrapper.Attr("id", _id);

            foreach (var option in _options)
            {
                var inputId = $"{_name}-{option.value}";

                var input = new HtmlTag("input")
                    .Attr("type", "radio")
                    .Attr("name", _name)
                    .Attr("id", inputId)
                    .Attr("value", option.value);

                if (option.selected)
                    input.Attr("checked", "checked");

                if (_autoPostBack)
                {
                    input.Attr("onchange", $"__doPostBack('{_name}','',this.form)");
                    input.Attr("language", "javascript");
                }

                var label = new HtmlTag("label")
                    .Attr("for", inputId)
                    .Text(option.label);

                if (!string.IsNullOrEmpty(option.color))
                    label.Attr("style", $"--segment-color:{option.color};");

                wrapper.Append(input);
                wrapper.Append(label);
            }

            return wrapper.ToString();
        }
    }
}