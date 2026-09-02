using HtmlTags;

namespace RotoMonsterUI
{
    public class IconButton
    {
        private string _text;
        private string _icon;
        private string _url;
        private ButtonStyle _style;
        private string _name;
        private bool _centered;
        private bool _iconOnly;
        private TooltipPlacement _tooltipPlacement = TooltipPlacement.Auto;
        private string _onClick;

        public string Id { get; private set; }
        public IconInput IconInput { get; set; }

        public static string FontAwesome(string classes)
        {
            return $"<i class=\"{classes}\"></i>";
        }

        public IconButton(string text, string icon)
        {
            _text = text;
            _icon = icon;
            _style = ButtonStyle.Secondary;
        }

        public IconButton(string text, IconType iconType)
        {
            _text = text;
            _style = ButtonStyle.Secondary;
            IconInput = new IconInput { Type = iconType, Size = 18 };
            _icon = new Icon(IconInput).Render();
        }

        public IconButton WithUrl(string url)
        {
            _url = url;
            return this;
        }

        public IconButton WithStyle(ButtonStyle style)
        {
            _style = style;
            return this;
        }

        public IconButton WithId(string id)
        {
            Id = id;
            return this;
        }

        public IconButton WithTitle(string title)
        {
            _text = title;
            return this;
        }

        public IconButton WithName(string name)
        {
            _name = name;
            return this;
        }

        public IconButton WithIconSize(int size)
        {
            IconInput.Size = size;
            return this;
        }

        public IconButton WithCentered()
        {
            _centered = true;
            return this;
        }

        /// <summary>
        /// Where the icon-only tooltip sits. Right is worth setting on a small
        /// icon, since the cursor covers whatever is directly below it.
        /// Ignored when the button is not icon-only, because it has no tooltip.
        /// </summary>
        public IconButton WithTooltipPlacement(TooltipPlacement placement)
        {
            _tooltipPlacement = placement;
            return this;
        }

        public IconButton WithIconOnly()
        {
            _iconOnly = true;
            return this;
        }

        public IconButton WithOnClick(string js)
        {
            _onClick = js;
            return this;
        }

        public string Render()
        {
            if (IconInput != null)
                _icon = new Icon(IconInput).Render();

            var tag = string.IsNullOrEmpty(_url)
                ? new HtmlTag("button")
                : new HtmlTag("a").Attr("href", _url);

            tag.AddClass("modern-filter-btn");

            switch (_style)
            {
                case ButtonStyle.Primary:
                    tag.AddClass("modern-filter-btn-primary");
                    break;
                case ButtonStyle.Secondary:
                    tag.AddClass("modern-filter-btn-secondary");
                    break;
                default:
                    tag.AddClass($"btn btn-{_style.ToString().ToLower()}");
                    break;
            }

            if (!string.IsNullOrEmpty(Id))
                tag.Attr("id", Id);

            if (!string.IsNullOrEmpty(_name))
                tag.Attr("name", _name);

            if (!string.IsNullOrEmpty(_onClick))
                tag.Attr("onclick", _onClick);

            if (_iconOnly)
            {
                tag.AddClass("modern-filter-btn-icon-only");
                tag.Attr("aria-label", _text);
                tag.AppendHtml(_icon);

                var tooltipped = new CustomTooltip(tag.ToString(), _text)
                    .WithHoverTrigger()
                    .WithPlacement(_tooltipPlacement)
                    .Render();

                if (_centered)
                    return $"<div style='text-align:center;'>{tooltipped}</div>";

                return tooltipped;
            }

            tag.AppendHtml($"<span style='margin-right:0.35rem;'>{_icon}</span>{_text}");

            if (_centered)
                return $"<div style='text-align:center;'>{tag}</div>";

            return tag.ToString();
        }
    }
}