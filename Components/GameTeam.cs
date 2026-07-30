using System;
using HtmlTags;

namespace RotoMonsterUI
{
    public class GameTeam
    {
        private readonly GameTeamInput _input;

        public GameTeam(GameTeamInput input)
        {
            _input = input;
        }

        public string Render()
        {
            bool gameStarted = _input.GameStarted || _input.IsGameLive || _input.IsGameFinished;
            float runs = gameStarted ? _input.CurrentRuns : _input.ProjectedRuns;

            string bgColor = _input.BgColor;
            if (string.IsNullOrEmpty(bgColor) || bgColor == "FFFFFF")
            {
                if (!gameStarted)
                {
                    bgColor = ColorHelper.GetYellowColorCode(runs, 3.5f, 6.5f, true);
                }
                else
                {
                    float avgRuns;
                    if (_input.Sport == GameSport.Basketball)
                    {
                        float avgGameScore = 112f; // rough average NBA team total score
                        var totalMinutes = _input.TotalQuarters * _input.QuarterLengthMinutes;
                        var elapsedMinutes = (_input.CurrentQuarter - 1) * _input.QuarterLengthMinutes
                            + (_input.QuarterLengthMinutes - _input.QuarterMinutesRemaining);
                        var elapsedFraction = totalMinutes > 0 ? Math.Max(0, Math.Min(1, elapsedMinutes / totalMinutes)) : 0;
                        avgRuns = avgGameScore * (float)elapsedFraction;

                        if (runs >= avgRuns)
                            bgColor = ColorHelper.GetGreenColorCode(runs - avgRuns, 0f, avgRuns * 2.5f, true);
                        else
                            bgColor = ColorHelper.GetRedColorCode(avgRuns - runs, 0f, avgGameScore, true);
                    }
                    else
                    {
                        float avgGameRuns = 4.5f;
                        avgRuns = avgGameRuns * (float)Math.Min(1, _input.CurrentOuts / 54.0);
                        if (runs >= avgRuns)
                            bgColor = ColorHelper.GetGreenColorCode(runs - avgRuns, 0f, avgRuns * 2.5f, true);
                        else
                            bgColor = ColorHelper.GetRedColorCode(avgRuns - runs, 0f, avgGameRuns, true);
                    }
                }
            }

            var cell = new HtmlTag("div").AddClass("game-team-cell");
            if (_input.IsWinner && _input.IsGameFinished)
                cell.AddClass("winner");
            cell.Attr("style", $"background-color:#{bgColor};");

            // Area A: lineup dot (always rendered, even if empty, to keep fixed width)
            var areaA = new HtmlTag("span").AddClass("game-team-cell-a");
            if (!gameStarted)
            {
                areaA.AppendHtml(new LineupDot(new LineupDotInput
                {
                    IsConfirmed = _input.LineupConfirmed,
                    Players = _input.LineupPlayers,
                    HighlightOwnedPlayers = _input.HighlightOwnedPlayers
                }).Render());
            }
            cell.Append(areaA);

            // Area B: team code + runs (always rendered)
            var areaB = new HtmlTag("span").AddClass("game-team-cell-b");
            areaB.Append(new HtmlTag("span").AddClass("game-team-code").Text(_input.TeamCode));
            if (gameStarted)
                areaB.Append(new HtmlTag("span").AddClass("game-team-runs").Text(runs.ToString("0")));
            else if (runs != 0)
                areaB.Append(new HtmlTag("span").AddClass("game-team-runs").Text(runs.ToString("0.0")));
            cell.Append(areaB);

            // Area C: lineup/other icon (always rendered, even if empty, to keep fixed width)
            var areaC = new HtmlTag("span").AddClass("game-team-cell-c");
            if (_input.PlayerCount.HasValue && _input.PlayerCount.Value > 0 && _input.PlayerIconType.HasValue)
            {
                bool hasWarnings = !gameStarted && _input.WarningPlayers != null && _input.WarningPlayers.Exists(p => p.TeamCode == _input.TeamCode);

                if (hasWarnings)
                {
                    areaC.AppendHtml(new WarningIcon(new WarningIconInput
                    {
                        TeamCode = _input.TeamCode,
                        WarningPlayers = _input.WarningPlayers,
                        IconType = IconType.LineupCard,
                        IconColor = _input.PlayerIconColor
                    }).Render());
                }
                else
                {
                    var icon = new Icon(new IconInput { Type = _input.PlayerIconType.Value, Color = _input.PlayerIconColor ?? "#94a3b8", Size = 14 }).Render();
                    areaC.AppendHtml(new CustomTooltip(icon, $"{_input.PlayerCount} {SingularPlural.Get("player", _input.PlayerCount.GetValueOrDefault(0))} on this team").Render());
                }
            }
            cell.Append(areaC);

            return cell.ToString();
        }
    }
}