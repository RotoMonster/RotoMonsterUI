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

        public string TagText { get; set; }

        public bool ShowDetailsInBadge { get; set; }

        public bool IsUnofficial { get; set; }
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

            var tooltipParts = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(_input.StatusText)) tooltipParts.Add(_input.StatusText);
            if (!string.IsNullOrEmpty(_input.TagText)) tooltipParts.Add(_input.TagText);
            if (!string.IsNullOrEmpty(_input.StatusDetails)) tooltipParts.Add(_input.StatusDetails);
            if (_input.IsUnofficial) tooltipParts.Add("Status is Unofficial");
            var tooltipText = string.Join(" - ", tooltipParts);

            var color = string.IsNullOrEmpty(_input.StatusColor) ? "e05c00" : _input.StatusColor;
            var normalizedColor = color.StartsWith("#") ? color : "#" + color;

            var badge = new HtmlTag("span")
                .AddClass("injury-badge")
                .Attr("style", $"background-color:{normalizedColor};");

            if (!string.IsNullOrEmpty(badgeText))
                badge.Append(new HtmlTag("span").Text(badgeText));

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

            if (_input.IsUnofficial)
            {
                badge.Append(new HtmlTag("span")
                    .AddClass("injury-badge-tag")
                    .AppendHtml(new Icon(new IconInput
                    {
                        Type = IconType.UnofficialTag,
                        Size = 12,
                        Color = "currentColor"
                    }).Render()));
            }

            if (_input.ShowDetailsInBadge && !string.IsNullOrEmpty(_input.StatusDetails))
                badge.Append(new HtmlTag("span")
                    .AddClass("injury-badge-details")
                    .Text(_input.StatusDetails));

            return new CustomTooltip(badge.ToString(), tooltipText).Render();
        }
    }
}