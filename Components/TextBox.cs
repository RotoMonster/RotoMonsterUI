using HtmlTags;

namespace RotoMonsterUI
{
    public enum TextBoxType
    {
        Text,
        Number
    }

    public class TextBox
    {
        private string _name;
        private string _id;
        private string _value;
        private string _placeholder;
        private bool _postBack;
        private TextBoxType _type = TextBoxType.Text;
        private double? _minValue;
        private double? _maxValue;
        private int? _maxLength;
        private int? _displayLength;
        private string _errorMessage;

        public string Id => _id ?? _name ?? "";

        public TextBox WithName(string name)
        {
            _name = name;
            return this;
        }

        public TextBox WithId(string id)
        {
            _id = id;
            return this;
        }

        public TextBox WithValue(string value)
        {
            _value = value;
            return this;
        }

        public TextBox WithPlaceholder(string placeholder)
        {
            _placeholder = placeholder;
            return this;
        }

        public TextBox WithPostBack()
        {
            _postBack = true;
            return this;
        }

        public TextBox WithType(TextBoxType type)
        {
            _type = type;
            return this;
        }

        public TextBox WithRange(double min, double max)
        {
            _minValue = min;
            _maxValue = max;
            return this;
        }

        public TextBox WithMaxLength(int maxLength)
        {
            _maxLength = maxLength;
            return this;
        }

        public TextBox WithDisplayLength(int displayLength)
        {
            _displayLength = displayLength;
            return this;
        }

        public TextBox WithError(string errorMessage)
        {
            _errorMessage = errorMessage;
            return this;
        }

        public string Render()
        {
            var wrapper = new HtmlTag("div").AddClass("bm-textbox-wrapper");

            var input = new HtmlTag("input")
                .Attr("type", _type == TextBoxType.Number ? "number" : "text")
                .AddClass("bm-textbox")
                .Attr("name", _name ?? "")
                .Attr("id", _id ?? _name ?? "");

            if (!string.IsNullOrEmpty(_placeholder))
                input.Attr("placeholder", _placeholder);

            if (!string.IsNullOrEmpty(_value))
                input.Attr("value", _value);

            if (_type == TextBoxType.Number)
            {
                if (_minValue.HasValue)
                    input.Attr("min", _minValue.Value.ToString());
                if (_maxValue.HasValue)
                    input.Attr("max", _maxValue.Value.ToString());
            }
            else if (_maxLength.HasValue)
            {
                input.Attr("maxlength", _maxLength.Value.ToString());
            }

            if (_displayLength.HasValue)
                input.Attr("size", _displayLength.Value.ToString());

            if (_postBack && !string.IsNullOrEmpty(_name))
            {
                input.Attr("onchange", $"__doPostBack('{_name}','',this.form)");
                input.Attr("language", "javascript");
            }

            if (!string.IsNullOrEmpty(_errorMessage))
                input.AddClass("bm-textbox--error");

            wrapper.Append(input);

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                var error = new HtmlTag("div")
                    .AddClass("bm-textbox-error")
                    .Text(_errorMessage);
                wrapper.Append(error);
            }

            return wrapper.ToString();
        }
    }
}