using HtmlTags;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class DisplayTeamCodeInput
    {
        public string TeamCode { get; set; }
        public float Runs { get; set; }
        public string BgColor { get; set; }
        public bool IsWinner { get; set; }
        public bool IsGameFinished { get; set; }
        public bool GameStarted { get; set; }
        public bool ShowLineupDot { get; set; } = false;
        public bool LineupConfirmed { get; set; } = false;
        public List<LineupPlayer> LineupPlayers { get; set; }
        public bool HighlightOwnedPlayers { get; set; }
        public List<WarningPlayer> WarningPlayers { get; set; }
        public IconType? WarningIconType { get; set; }
        public string WarningIconColor { get; set; }
    }

    public class DisplayTeamCode
    {
        private readonly DisplayTeamCodeInput _input;

        public DisplayTeamCode(DisplayTeamCodeInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var cell = new HtmlTag("div").AddClass("game-team-cell");
            if (_input.IsWinner && _input.IsGameFinished)
                cell.AddClass("winner");
            cell.Attr("style", $"background-color:#{_input.BgColor};");

            if (_input.ShowLineupDot && !_input.GameStarted)
            {
                cell.AppendHtml(new LineupDot(new LineupDotInput
                {
                    IsConfirmed = _input.LineupConfirmed,
                    Players = _input.LineupPlayers,
                    HighlightOwnedPlayers = _input.HighlightOwnedPlayers
                }).Render());
            }

            cell.Append(new HtmlTag("span").AddClass("game-team-code").Text(_input.TeamCode));

            if (_input.GameStarted)
                cell.Append(new HtmlTag("span").AddClass("game-team-runs").Text(_input.Runs.ToString("0")));
            else if (_input.Runs != 0)
                cell.Append(new HtmlTag("span").AddClass("game-team-runs").Text(_input.Runs.ToString("0.0")));

            if (_input.ShowLineupDot && !_input.GameStarted && _input.LineupConfirmed && _input.WarningPlayers != null && _input.WarningIconType.HasValue)
            {
                cell.AppendHtml(new WarningIcon(new WarningIconInput
                {
                    TeamCode = _input.TeamCode,
                    WarningPlayers = _input.WarningPlayers,
                    IconType = _input.WarningIconType.Value,
                    IconColor = _input.WarningIconColor
                }).Render());
            }

            return cell.ToString();
        }
    }
}