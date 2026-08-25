using HtmlTags;

namespace RotoMonsterUI
{
    public class Checkbox
    {
        private string _label;
        private string _name;
        private string _id;
        private bool _checked;
        private bool _autoPostBack;

        public Checkbox WithLabel(string label)
        {
            _label = label;
            return this;
        }

        public Checkbox WithName(string name)
        {
            _name = name;
            return this;
        }

        public Checkbox WithId(string id)
        {
            _id = id;
            return this;
        }

        public Checkbox WithChecked(bool isChecked)
        {
            _checked = isChecked;
            return this;
        }

        public Checkbox WithPostBack()
        {
            _autoPostBack = true;
            return this;
        }

        public string Id => _id ?? _name ?? "";

        public string Render()
        {
            var wrapper = new HtmlTag("div").AddClass("bm-checkbox");

            var inputId = _id ?? _name ?? "";

            if (!string.IsNullOrEmpty(inputId))
                wrapper.Attr("id", $"{inputId}-wrapper");

            var input = new HtmlTag("input")
                .Attr("type", "checkbox")
                .Attr("name", _name)
                .Attr("id", inputId)
                .Attr("value", "1");

            if (_checked)
                input.Attr("checked", "checked");

            if (_autoPostBack)
            {
                input.Attr("onclick", $"__doPostBack('{_name}','',this.form)");
                input.Attr("language", "javascript");
            }

            var label = new HtmlTag("label")
                .Attr("for", inputId)
                .Text(_label);

            wrapper.Append(input);
            wrapper.Append(label);

            return wrapper.ToString();
        }
    }
}