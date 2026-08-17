using HtmlTags;

namespace RotoMonsterUI
{
    public class InjuryBadgeInput
    {
        public string StatusAbbreviation { get; set; }
        public string StatusText { get; set; }
        public string StatusColor { get; set; }
        public int NumberOfGames { get; set; }
        public string StatusDetails { get; set; }

        /// <summary>Resolved to an icon shown in the badge. Falls back to Other when it doesn't match an IconType.</summary>
        public string TagText { get; set; }

        /// <summary>Show StatusDetails in the badge itself. It always appears in the tooltip regardless.</summary>
        public bool ShowDetailsInBadge { get; set; }
    }

    public class InjuryBadge
    {
        private readonly InjuryBadgeInput _input;

        public InjuryBadge(InjuryBadgeInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var badgeText = _input.NumberOfGames > 0
                ? $"{_input.StatusAbbreviation} {_input.NumberOfGames}g"
                : _input.StatusAbbreviation;

            var tooltipText = !string.IsNullOrEmpty(_input.StatusDetails)
                ? $"{_input.StatusText} – {_input.StatusDetails}"
                : _input.StatusText;

            var color = string.IsNullOrEmpty(_input.StatusColor) ? "e05c00" : _input.StatusColor;
            var normalizedColor = color.StartsWith("#") ? color : "#" + color;

            var badge = new HtmlTag("span")
                .AddClass("injury-badge")
                .Attr("style", $"background-color:{normalizedColor};");

            if (!string.IsNullOrEmpty(_input.TagText))
            {
                badge.Append(new HtmlTag("span")
                    .AddClass("injury-badge-tag")
                    .AppendHtml(new Icon(new IconInput
                    {
                        Type = IconTypeResolver.Resolve(_input.TagText, IconType.Other),
                        Size = 12,
                        Color = "currentColor"
                    }).Render()));
            }

            if (!string.IsNullOrEmpty(badgeText))
                badge.Append(new HtmlTag("span").Text(badgeText));

            if (_input.ShowDetailsInBadge && !string.IsNullOrEmpty(_input.StatusDetails))
                badge.Append(new HtmlTag("span")
                    .AddClass("injury-badge-details")
                    .Text(_input.StatusDetails));

            return new CustomTooltip(badge.ToString(), tooltipText).Render();
        }
    }
}