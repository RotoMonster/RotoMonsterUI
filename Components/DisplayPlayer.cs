using HtmlTags;

namespace RotoMonsterUI
{
    public class DisplayPlayer
    {
        private DisplayPlayerInput _input;

        public DisplayPlayer(DisplayPlayerInput input)
        {
            _input = input;
        }

        private string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return color;
            if (color.StartsWith("var(") || color.StartsWith("#")) return color;
            return "#" + color;
        }

        public string Render()
        {
            var wrapper = new HtmlTag("span").AddClass("display-player");

            var playerLink = new HtmlTag("a")
                .AddClass("display-player-name player-link")
                .Attr("href", RotoMonsterUIUrls.PlayerUrl(_input.PlayerId))
                .Text(_input.PlayerName);
            wrapper.Append(playerLink);

            var team = new HtmlTag("span")
                .AddClass("ml-2 mr-1 ms-2 me-1")
                .AddClass("display-player-team");
            if (!string.IsNullOrEmpty(_input.TeamColor))
                team.Attr("style", $"color:{NormalizeColor(_input.TeamColor)}");
            team.Text(_input.TeamCode);
            wrapper.Append(team);

            foreach (var pos in _input.Positions)
            {
                var posTag = new HtmlTag("span")
                    .AddClass("ml-1 ms-1")
                    .AddClass("display-player-pos")
                    .Attr("style", $"color:{NormalizeColor(pos.Color)}")
                    .Text(pos.Abbreviation);
                wrapper.Append(posTag);
            }

            return wrapper.ToString();
        }
    }
}