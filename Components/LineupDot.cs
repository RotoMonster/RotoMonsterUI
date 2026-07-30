using HtmlTags;
using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class LineupDotInput
    {
        public bool IsConfirmed { get; set; }
        public List<LineupPlayer> Players { get; set; }
        public bool HighlightOwnedPlayers { get; set; }
    }

    public class LineupDot
    {
        private readonly LineupDotInput _input;

        public LineupDot(LineupDotInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var dot = new HtmlTag("span")
                .AddClass(_input.IsConfirmed ? "lineup-dot lineup-dot-confirmed" : "lineup-dot lineup-dot-empty")
                .ToString();

            if (_input.IsConfirmed && _input.Players != null && _input.Players.Count > 0)
                return new CustomTooltip(dot, BuildLineupHtml()).WithMaxWidth(260).Render();

            return new CustomTooltip(dot, _input.IsConfirmed ? "Lineup Confirmed" : "Lineup Not Confirmed").Render();
        }

        private string BuildLineupHtml()
        {
            var wrap = new HtmlTag("div").AddClass("lineup-tip");
            wrap.Append(new HtmlTag("div").AddClass("lineup-tip-title").Text("Confirmed Lineup"));

            var ordered = _input.Players
                .OrderBy(p => p.IsStartingPitcher ? 1 : 0)
                .ThenBy(p => p.BattingOrder.HasValue ? p.BattingOrder.Value : int.MaxValue)
                .ToList();

            bool pitcherDividerDone = false;

            foreach (var player in ordered)
            {
                var row = new HtmlTag("div").AddClass("lineup-tip-row");

                if (player.IsStartingPitcher && !pitcherDividerDone)
                {
                    row.AddClass("lineup-tip-row--pitcher");
                    pitcherDividerDone = true;
                }

                if (_input.HighlightOwnedPlayers && player.IsOwned)
                    row.AddClass("lineup-tip-row--owned");

                var num = new HtmlTag("span").AddClass("lineup-tip-num");
                if (!player.IsStartingPitcher && player.BattingOrder.HasValue)
                    num.Text(player.BattingOrder.Value.ToString());
                row.Append(num);

                var name = new HtmlTag("span").AddClass("lineup-tip-name");
                name.Text(player.Player != null ? player.Player.PlayerName : "");
                row.Append(name);

                var pos = new HtmlTag("span").AddClass("lineup-tip-pos");
                if (!string.IsNullOrEmpty(player.PositionColor))
                    pos.Attr("style", "color:" + NormalizeColor(player.PositionColor) + ";");
                pos.Text(player.Position);
                row.Append(pos);

                wrap.Append(row);
            }

            return wrap.ToString();
        }

        private string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return color;
            if (color.StartsWith("var(") || color.StartsWith("#")) return color;
            return "#" + color;
        }
    }
}