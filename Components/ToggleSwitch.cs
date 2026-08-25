using HtmlTags;

namespace RotoMonsterUI
{
    public class ToggleSwitch
    {
        private string _label;
        private string _name;
        private bool _checked;
        private string _id;
        private bool _postBack;
        private bool _onDarkSurface;

        public string Id => _id ?? _name ?? "";

        public ToggleSwitch WithLabel(string label)
        {
            _label = label;
            return this;
        }

        public ToggleSwitch WithName(string name)
        {
            _name = name;
            return this;
        }

        public ToggleSwitch WithId(string id)
        {
            _id = id;
            return this;
        }

        public ToggleSwitch WithChecked(bool isChecked)
        {
            _checked = isChecked;
            return this;
        }

        public ToggleSwitch WithPostBack()
        {
            _postBack = true;
            return this;
        }
        public ToggleSwitch OnDarkSurface()
        {
            _onDarkSurface = true;
            return this;
        }

        public string Render()
        {
            var wrapper = new HtmlTag("label").AddClass("modern-filter-toggle");

            if (_onDarkSurface)
                wrapper.AddClass("modern-filter-toggle--dark-surface");

            var input = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("name", _name ?? "")
                .Attr("id", _id ?? _name ?? "")
                .Attr("value", "1");

            if (_checked)
                input.Attr("checked", "checked");

            if (_postBack && !string.IsNullOrEmpty(_name))
            {
                input.Attr("onchange", $"__doPostBack('{_name}','',this.form)");
                input.Attr("language", "javascript");
            }

            var switchDiv = new HtmlTag("div").AddClass("modern-filter-toggle-switch");

            var labelSpan = new HtmlTag("span")
                .AddClass("modern-filter-toggle-label")
                .AppendHtml(_label ?? "");

            wrapper.Append(input);
            wrapper.Append(switchDiv);
            wrapper.Append(labelSpan);

            return wrapper.ToString();
        }
    }
}