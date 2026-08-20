using HtmlTags;

namespace RotoMonsterUI
{
    public class SportradarAttribution
    {
        private readonly SportradarAttributionInput _input;

        public SportradarAttribution(SportradarAttributionInput input = null)
        {
            _input = input ?? new SportradarAttributionInput();
        }

        public string Render()
        {
            var wrapper = new HtmlTag("div").AddClass("sportradar-attribution");

            if (!string.IsNullOrWhiteSpace(_input.Text))
            {
                wrapper.Append(
                    new HtmlTag("span")
                        .AddClass("sportradar-attribution-text")
                        .Text(_input.Text));
            }

            var link = new HtmlTag("a")
                .AddClass("sportradar-attribution-logo-link")
                .Attr("href", _input.LinkUrl)
                .Attr("target", "_blank")
                .Attr("rel", "noopener noreferrer");

            var hasDark = !string.IsNullOrWhiteSpace(_input.DarkLogoUrl);

            link.Append(Logo(_input.LogoUrl, hasDark ? "--light" : null));

            if (hasDark)
            {
                link.Append(Logo(_input.DarkLogoUrl, "--dark"));
            }

            wrapper.Append(link);

            return wrapper.ToString();
        }

        private HtmlTag Logo(string url, string modifier)
        {
            var img = new HtmlTag("img")
                .AddClass("sportradar-attribution-logo")
                .Attr("src", url)
                .Attr("alt", _input.AltText)
                .Attr("width", _input.SourceWidth.ToString())
                .Attr("height", _input.LogoHeight.ToString())
                .Attr("loading", "lazy");

            if (!string.IsNullOrEmpty(modifier))
            {
                img.AddClass("sportradar-attribution-logo" + modifier);
            }

            return img;
        }
    }
}