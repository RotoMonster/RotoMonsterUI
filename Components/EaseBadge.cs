using HtmlTags;

namespace RotoMonsterUI
{
    public class EaseBadge
    {
        private readonly double _ease;
        private string _label;
        private string _title;
        private bool _away;
        private bool _quality;
        private bool _outline;

        public EaseBadge(double ease)
        {
            _ease = ease;
        }

        public EaseBadge WithLabel(string label)
        {
            _label = label;
            return this;
        }

        public EaseBadge WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public EaseBadge WithAway()
        {
            _away = true;
            return this;
        }

        public EaseBadge WithQuality()
        {
            _quality = true;
            return this;
        }

        public EaseBadge WithOutline()
        {
            _outline = true;
            return this;
        }

        public static string ColorFor(double ease)
        {
            var value = (float)ease;
            return value >= 0
                ? ColorHelper.GetGreenColorCode(value, 0, 1, true)
                : ColorHelper.GetRedColorCode(-value, 0, 1, true);
        }

        public static string StyleFor(double ease)
        {
            var color = ColorFor(ease);
            return string.IsNullOrEmpty(color) ? null : "--ease-color:#" + color + ";";
        }

        public string Render()
        {
            if (string.IsNullOrEmpty(_label)) return "";

            var badge = new HtmlTag("span").AddClass("ease-badge");

            if (_outline) badge.AddClass("ease-badge--outline");
            if (_quality) badge.AddClass("ease-badge--quality");

            var style = StyleFor(_ease);
            if (!string.IsNullOrEmpty(style)) badge.Attr("style", style);

            if (!string.IsNullOrEmpty(_title)) badge.Attr("title", _title);

            if (_away)
                badge.Append(new HtmlTag("span").AddClass("ease-badge-at").Text("@"));

            badge.Append(new HtmlTag("span").Text(_label));

            return badge.ToString();
        }
    }
}