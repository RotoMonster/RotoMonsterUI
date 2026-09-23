using System.Globalization;
using HtmlTags;

namespace RotoMonsterUI
{
    public class PlayerShapeMatches
    {
        private readonly PlayerShapeMatchesInput _input;

        public PlayerShapeMatches(PlayerShapeMatchesInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var table = new HtmlTag("table").AddClass("player-shape-matches");
            var head = new HtmlTag("tr");
            foreach (var h in new[] { "#", "Player", "Team", "Pos", "Rank", "Round" })
                head.Append(new HtmlTag("th").Text(h));
            head.Append(HeaderWithTip("9-Cat Value", "Overall projected value across the nine categories. A match can be worth much less than the selected player and still have the same shape."));
            head.Append(HeaderWithTip("Match", "How closely this player's strengths and weaknesses line up with the selected player's, regardless of how good either one is overall. 100% is an identical profile."));
            head.Append(HeaderWithTip("Best", "This player's strongest category."));
            head.Append(HeaderWithTip("Weakest", "This player's weakest category."));
            table.Append(new HtmlTag("thead").Append(head));

            var body = new HtmlTag("tbody");
            if (_input != null && _input.Matches != null)
            {
                for (var i = 0; i < _input.Matches.Count; i++)
                {
                    var m = _input.Matches[i];
                    if (m == null || m.Player == null) continue;

                    var row = new HtmlTag("tr");
                    var number = new HtmlTag("td").AddClass("player-shape-matches-rank");
                    if (i < _input.ColoredRows)
                    {
                        var color = PlayerShapeRadarInput.DefaultColors[(i + 1) % PlayerShapeRadarInput.DefaultColors.Length];
                        number.Append(new HtmlTag("span").AddClass("player-shape-radar-swatch").Attr("style", "background:" + color));
                    }
                    number.Append(new HtmlTag("span").Text((i + 1).ToString(CultureInfo.InvariantCulture)));
                    row.Append(number);

                    row.Append(new HtmlTag("td").AddClass("player-shape-matches-name").Text(m.Player.Name));
                    row.Append(new HtmlTag("td").Text(m.Player.Team));
                    row.Append(new HtmlTag("td").Text(m.Player.Position));
                    row.Append(new HtmlTag("td").Text(m.Player.Rank.ToString(CultureInfo.InvariantCulture)));
                    row.Append(new HtmlTag("td").Text(m.Player.Round.ToString(CultureInfo.InvariantCulture)));
                    row.Append(new HtmlTag("td").Text(m.Player.Value.ToString("0.000", CultureInfo.InvariantCulture)));
                    row.Append(new HtmlTag("td").AddClass("player-shape-matches-match").Text((m.Match * 100).ToString("0.0", CultureInfo.InvariantCulture) + "%"));
                    row.Append(new HtmlTag("td").Text(m.BestCategory));
                    row.Append(new HtmlTag("td").Text(m.WeakestCategory));
                    body.Append(row);
                }
            }
            table.Append(body);

            return new HtmlTag("div").AddClass("player-shape-matches-wrap").Append(table).ToString();
        }

        private static HtmlTag HeaderWithTip(string label, string tip)
        {
            var trigger = "<span class=\"player-shape-matches-tip\">" + System.Net.WebUtility.HtmlEncode(label) + "</span>";
            var tooltip = new CustomTooltip(trigger, System.Net.WebUtility.HtmlEncode(tip))
                .WithHoverTrigger()
                .WithPlacement(TooltipPlacement.Above)
                .WithMaxWidth(280)
                .Render();
            return new HtmlTag("th").AppendHtml(tooltip);
        }
    }
}