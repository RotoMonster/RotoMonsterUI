using HtmlTags;
using System;

namespace RotoMonsterUI
{
    /// <summary>
    /// Where a tooltip sits relative to whatever opened it. Auto picks above
    /// or below on available space, which is the old behaviour and stays the
    /// default. Right is worth setting on small icons, since the cursor
    /// usually covers what is directly below them.
    /// </summary>
    public enum TooltipPlacement
    {
        Auto,
        Above,
        Below,
        Left,
        Right
    }

    public class CustomTooltip
    {
        private string _triggerHtml;
        private string _contentHtml;
        private string _id;
        private bool _centered;
        private int? _maxWidth;
        private bool _hoverTrigger;
        private TooltipPlacement _placement = TooltipPlacement.Auto;

        public CustomTooltip(string triggerHtml, string contentHtml)
        {
            _triggerHtml = triggerHtml;
            _contentHtml = contentHtml;
            _id = "bm-tip-" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public CustomTooltip WithCentered()
        {
            _centered = true;
            return this;
        }

        public CustomTooltip WithMaxWidth(int pixels)
        {
            _maxWidth = pixels;
            return this;
        }

        public CustomTooltip WithHoverTrigger()
        {
            _hoverTrigger = true;
            return this;
        }

        public CustomTooltip WithPlacement(TooltipPlacement placement)
        {
            _placement = placement;
            return this;
        }

        private static string PlacementValue(TooltipPlacement placement)
        {
            switch (placement)
            {
                case TooltipPlacement.Above: return "above";
                case TooltipPlacement.Below: return "below";
                case TooltipPlacement.Left: return "left";
                case TooltipPlacement.Right: return "right";
                default: return null;
            }
        }

        /// <summary>
        /// Wraps an element that already exists, so a site with a title
        /// attribute becomes a real tooltip without restructuring how it is
        /// built. Returns the wrapper, so it drops straight into an existing
        /// Append call.
        /// </summary>
        public static HtmlTag Wrap(HtmlTag trigger, string text,
            TooltipPlacement placement = TooltipPlacement.Auto)
        {
            if (trigger == null) return new HtmlTag("span");

            if (string.IsNullOrEmpty(text))
                return new HtmlTag("span").AppendHtml(trigger.ToString());

            var html = new CustomTooltip(trigger.ToString(), text)
                .WithHoverTrigger()
                .WithPlacement(placement)
                .Render();

            // A plain span wrapper rather than NoTag, since the tooltip's own
            // markup already has a wrap span and an extra inline span changes
            // nothing about layout.
            return new HtmlTag("span").AddClass("bm-tooltip-host").AppendHtml(html);
        }

        public string Render()
        {
            var wrapper = new HtmlTag("span")
                .AddClass("bm-tooltip-trigger")
                .Attr("data-bm-tooltip", _id)
                .AppendHtml(_triggerHtml);

            if (_hoverTrigger)
                wrapper.AddClass("bm-tooltip-trigger--hover");

            var placement = PlacementValue(_placement);
            if (placement != null)
                wrapper.Attr("data-bm-placement", placement);

            var content = new HtmlTag("div")
                .AddClass("bm-tooltip-content")
                .Attr("id", _id)
                .Attr("role", "tooltip");

            if (_centered)
                content.AddClass("bm-tooltip-content--centered");

            if (_maxWidth.HasValue)
                content.Attr("style", $"max-width:{_maxWidth}px;");

            content.AppendHtml(_contentHtml);

            var outer = new HtmlTag("span").AddClass("bm-tooltip-wrap");
            outer.Append(wrapper);
            outer.Append(content);

            return outer.ToString();
        }
    }
}