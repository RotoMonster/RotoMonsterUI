using HtmlTags;
using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class LineupCard
    {
        private readonly LineupCardInput _input;

        public LineupCard(LineupCardInput input)
        {
            _input = input;
        }
        private HtmlTag BuildTeamHeader(LineupCardTeamInput team)
        {
            var header = new HtmlTag("div").AddClass("lineup-card-team-header");

            var nameRow = new HtmlTag("div").AddClass("lineup-card-name-row");

            var bgColor = ColorHelper.GetYellowColorCode(team.ProjectedRuns, 3.5f, 6.5f, true);

            nameRow.AppendHtml(new DisplayTeamCode(new DisplayTeamCodeInput
            {
                TeamCode = team.TeamCode,
                Runs = team.ProjectedRuns,
                BgColor = bgColor,
                GameStarted = false,
                IsWinner = false,
                IsGameFinished = false,
                ShowLineupDot = team.IsLineupConfirmed.HasValue,
                LineupConfirmed = team.IsLineupConfirmed ?? false,
                WarningPlayers = team.WarningPlayers,
                WarningIconType = team.WarningPlayersType.HasValue
                    ? IconType.LineupCard
                    : (IconType?)null,
                WarningIconColor = team.WarningPlayersType.HasValue
                    ? "#F59E0B"
                    : null
            }).Render());

            if (team.Rank.HasValue)
            {
                var rank = new HtmlTag("span").AddClass("lineup-card-rank").Text($"#{team.Rank}");
                nameRow.Append(rank);
            }

            header.Append(nameRow);

            if (!string.IsNullOrEmpty(team.OddsLine))
            {
                var odds = new HtmlTag("div").AddClass("lineup-card-odds").Text(team.OddsLine);
                header.Append(odds);
            }

            return header;
        }

        private HtmlTag BuildPlayerTable(LineupCardTeamInput team, string teamColor)
        {
            var wrapper = new HtmlTag("div").AddClass("lineup-card-table-wrapper");

            // Column header
            var colHeader = new HtmlTag("div").AddClass("lineup-card-col-header");
            colHeader.Attr("style", $"background-color:{teamColor}; color: white;");
            colHeader.AppendHtml("<span class='lineup-card-col-num'>#</span>");
            colHeader.AppendHtml($"<span class='lineup-card-col-name'>Player vs. {team.PlayerVsHandedness}</span>");
            wrapper.Append(colHeader);

            // Player rows
            foreach (var player in team.Players)
            {
                var row = new HtmlTag("div").AddClass("lineup-card-player-row");

               var num = new HtmlTag("span").AddClass("lineup-card-player-num");
                num.Text(player.IsStartingPitcher ? "" : player.BattingOrder?.ToString() ?? "");

                var name = new HtmlTag("span").AddClass("lineup-card-player-name");
                if (player.Player != null)
                {
                    var url = RotoMonsterUIUrls.PlayerUrl(player.Player.PlayerId);
                    var link = new HtmlTag("a")
                        .Attr("href", url)
                        .AddClass("player-link")
                        .Text(player.Player.PlayerName);
                    name.Append(link);
                    name.AppendHtml(" ");
                }

                var hand = new HtmlTag("span").AddClass("lineup-card-player-hand").Text(player.Handedness);
                name.Append(hand);

                if (player.InjuryBadge != null)
                    name.AppendHtml(new InjuryBadge(player.InjuryBadge).Render());

                var pos = new HtmlTag("span").AddClass("lineup-card-player-pos");
                if (!string.IsNullOrEmpty(player.PositionColor))
                    pos.Attr("style", $"color:{NormalizeColor(player.PositionColor)};");
                pos.Text(player.Position);

                row.Append(num);
                row.Append(name);
                row.Append(pos);
                wrapper.Append(row);
            }

            // Confirmed / Expected footer
            var footer = new HtmlTag("div").AddClass("lineup-card-footer");
            if (team.IsVerified)
            {
                footer.AddClass("lineup-card-footer--confirmed");
                footer.AppendHtml("CONFIRMED LINEUP");
            }
            else if (team.LineupExpectedMinutes.HasValue)
            {
                footer.AddClass("lineup-card-footer--expected");
                footer.Text(FormatLineupExpected(team.LineupExpectedMinutes.Value));
            }

            if (team.IsVerified || team.LineupExpectedMinutes.HasValue)
                wrapper.Append(footer);

            return wrapper;
        }

        private string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return color;
            if (color.StartsWith("var(") || color.StartsWith("#")) return color;
            return "#" + color;
        }

        private string FormatLineupExpected(int minutes)
        {
            if (minutes < 0)
            {
                var overdue = Math.Abs(minutes);
                if (overdue >= 1440)
                    return $"Lineup Overdue by {overdue / 1440}d {(overdue % 1440) / 60}h";
                if (overdue >= 60)
                    return $"Lineup Overdue by {overdue / 60}h {overdue % 60}m";
                return $"Lineup Overdue by {overdue}m";
            }

            if (minutes >= 1440)
                return $"Lineup Expected {minutes / 1440}d {(minutes % 1440) / 60}h";
            if (minutes >= 60)
                return $"Lineup Expected {minutes / 60}h {minutes % 60}m";
            return $"Lineup Expected {minutes}m";
        }

        public string Render()
        {
            var card = new HtmlTag("div").AddClass("lineup-card");

            if (!string.IsNullOrEmpty(_input.Id))
                card.Attr("id", _input.Id);

            // Game row at top using RenderSingleGameInner
            if (_input.Game != null)
            {
                var gameRow = new HtmlTag("div").AddClass("lineup-card-game-row game-row-inner");
                gameRow.AppendHtml(new DisplayGames(_input.Game).RenderSingleGameInner(hideTeamCells: true));
                card.Append(gameRow);
            }

            // Two column layout
            var columns = new HtmlTag("div").AddClass("lineup-card-columns");

            // Away team
            var awayTeamColor = NormalizeColor(!string.IsNullOrEmpty(_input.AwayTeam.TeamColor)
                ? _input.AwayTeam.TeamColor
                : TeamColorHelper.GetTeamColorVar(_input.AwayTeam.TeamCode));

            var awayCol = new HtmlTag("div").AddClass("lineup-card-col");
            awayCol.Append(BuildTeamHeader(_input.AwayTeam));
            awayCol.Append(BuildPlayerTable(_input.AwayTeam, awayTeamColor));
            columns.Append(awayCol);

            // Home team
            var homeTeamColor = NormalizeColor(!string.IsNullOrEmpty(_input.HomeTeam.TeamColor)
                ? _input.HomeTeam.TeamColor
                : TeamColorHelper.GetTeamColorVar(_input.HomeTeam.TeamCode));

            var homeCol = new HtmlTag("div").AddClass("lineup-card-col");
            homeCol.Append(BuildTeamHeader(_input.HomeTeam));
            homeCol.Append(BuildPlayerTable(_input.HomeTeam, homeTeamColor));
            columns.Append(homeCol);

            card.Append(columns);

            return card.ToString();
        }
    }
}