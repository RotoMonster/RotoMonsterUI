using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class RotatingPanel
    {
        private readonly RotatingPanelInput _input;

        public RotatingPanel(RotatingPanelInput input)
        {
            _input = input;
        }

        private static string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return color;
            if (color.StartsWith("var(") || color.StartsWith("#")) return color;
            return "#" + color;
        }

        public string Render()
        {
            var slides = _input.Slides ?? new List<RotatingPanelSlide>();
            if (slides.Count == 0) return "";

            var wrap = new HtmlTag("div").AddClass("rotating-panel");
            if (!string.IsNullOrEmpty(_input.Id))
                wrap.Attr("id", _input.Id);

            wrap.Attr("style", "background:" + NormalizeColor(_input.AccentColorCSS) + ";");
            wrap.Attr("data-interval", _input.AutoAdvanceSeconds.ToString());

            if (!string.IsNullOrEmpty(_input.Heading) || _input.Icon.HasValue)
            {
                var header = new HtmlTag("div").AddClass("rotating-panel-header");

                if (_input.Icon.HasValue)
                {
                    header.AppendHtml(new Icon(new IconInput
                    {
                        Type = _input.Icon.Value,
                        Size = 20,
                        Color = "#fff"
                    }).Render());
                }

                if (!string.IsNullOrEmpty(_input.Heading))
                    header.Append(new HtmlTag("span")
                        .AddClass("rotating-panel-heading")
                        .Text(_input.Heading));

                wrap.Append(header);
            }

            var body = new HtmlTag("div").AddClass("rotating-panel-body");

            body.Append(Arrow(IconType.Previous, "-1"));

            var stage = new HtmlTag("div")
                .AddClass("rotating-panel-slides")
                .Attr("style", "min-height:" + _input.MinHeight + "px;");

            var track = new HtmlTag("div").AddClass("rotating-panel-track");

            for (int i = 0; i < slides.Count; i++)
            {
                track.Append(RenderSlide(slides[i]));
            }

            stage.Append(track);

            body.Append(stage);
            body.Append(Arrow(IconType.Next, "1"));
            wrap.Append(body);

            if (slides.Count > 1)
            {
                var dots = new HtmlTag("div").AddClass("rotating-panel-dots");
                for (int i = 0; i < slides.Count; i++)
                {
                    var dot = new HtmlTag("span").AddClass("rotating-panel-dot");
                    if (i == 0) dot.AddClass("rotating-panel-dot--active");
                    dots.Append(dot);
                }
                wrap.Append(dots);
            }

            return wrap.ToString();
        }

        private HtmlTag Arrow(IconType type, string direction)
        {
            var button = new HtmlTag("button")
                .Attr("type", "button")
                .AddClass("rotating-panel-arrow")
                .Attr("data-rotate", direction);

            button.AppendHtml(new Icon(new IconInput
            {
                Type = type,
                Size = 20,
                Color = "#fff"
            }).Render());

            return button;
        }

        private HtmlTag RenderSlide(RotatingPanelSlide slide)
        {
            var panel = new HtmlTag("div").AddClass("rotating-panel-slide");

            if (!string.IsNullOrEmpty(slide.Title))
                panel.Append(new HtmlTag("div")
                    .AddClass("rotating-panel-slide-title")
                    .Text(slide.Title));

            if (!string.IsNullOrEmpty(slide.Body))
            {
                var text = new HtmlTag("div").AddClass("rotating-panel-slide-body");
                if (_input.UseQuoteStyle)
                    text.AddClass("rotating-panel-slide-body--quote");
                text.AppendHtml(slide.Body);
                panel.Append(text);
            }

            if (!string.IsNullOrEmpty(slide.Attribution))
                panel.Append(new HtmlTag("div")
                    .AddClass("rotating-panel-slide-attribution")
                    .Text(slide.Attribution));

            return panel;
        }
    }
}