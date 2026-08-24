using HtmlTags;
using System.Collections.Generic;
using System.Globalization;

namespace RotoMonsterUI
{
    public class NbaLineupCard
    {
        private readonly NbaLineupCardInput _input;

        public NbaLineupCard(NbaLineupCardInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var card = new HtmlTag("div").AddClass("nba-lineup-card");

            if (!string.IsNullOrEmpty(_input.Id))
                card.Attr("id", _input.Id);

            card.Append(BuildHeader());

            var teams = new HtmlTag("div").AddClass("nba-lineup-teams");
            teams.Append(BuildTeam(_input.AwayTeam, false));
            teams.Append(BuildTeam(_input.HomeTeam, true));
            card.Append(teams);

            return card.ToString();
        }

        private HtmlTag BuildHeader()
        {
            var header = new HtmlTag("div").AddClass("nba-lineup-head");

            var away = _input.AwayTeam != null ? _input.AwayTeam.TeamCode : "";
            var home = _input.HomeTeam != null ? _input.HomeTeam.TeamCode : "";

            var matchup = away + " @ " + home;
            if (!string.IsNullOrEmpty(_input.TipTime))
                matchup += " \u00b7 " + _input.TipTime;

            header.Append(new HtmlTag("span").AddClass("nba-lineup-when").Text(matchup));

            if (!string.IsNullOrEmpty(_input.OddsLine))
                header.Append(new HtmlTag("span").AddClass("nba-lineup-line").Text(_input.OddsLine));

            return header;
        }

        private HtmlTag BuildTeam(NbaLineupTeamInput team, bool isHome)
        {
            var col = new HtmlTag("div").AddClass("nba-lineup-team");

            if (team == null)
                return col;

            col.Append(BuildTeamHeader(team, isHome));

            var players = team.Players ?? new List<NbaLineupPlayer>();
            foreach (var player in players)
                col.Append(BuildRow(player));

            var bench = team.BenchPlayers ?? new List<NbaLineupPlayer>();
            if (_input.ShowBench && bench.Count > 0)
            {
                var benchId = (_input.Id ?? "nba-lineup") + "-bench-" + (isHome ? "h" : "a");

                var toggle = new HtmlTag("button")
                    .AddClass("nba-lineup-divider")
                    .Attr("type", "button")
                    .Attr("data-nba-bench", benchId)
                    .Attr("aria-expanded", _input.BenchCollapsed ? "false" : "true");

                toggle.Append(new HtmlTag("span").Text(_input.BenchLabel));
                toggle.Append(new HtmlTag("span").AddClass("nba-lineup-caret"));
                col.Append(toggle);

                var wrap = new HtmlTag("div").AddClass("nba-lineup-bench").Attr("id", benchId);
                if (_input.BenchCollapsed)
                    wrap.AddClass("nba-lineup-bench--collapsed");

                foreach (var player in bench)
                    wrap.Append(BuildRow(player));

                col.Append(wrap);
            }

            return col;
        }

        private HtmlTag BuildTeamHeader(NbaLineupTeamInput team, bool isHome)
        {
            var header = new HtmlTag("div").AddClass("nba-lineup-team-head");

            var code = (isHome ? "@" : "") + team.TeamCode;
            header.Append(new HtmlTag("span").AddClass("nba-lineup-team-code").Text(code));

            if (team.ProjectedPoints.HasValue)
            {
                header.Append(new HtmlTag("span")
                    .AddClass("nba-lineup-team-proj")
                    .Text(team.ProjectedPoints.Value.ToString("0", CultureInfo.InvariantCulture)));
            }

            if (team.IsBackToBack)
            {
                var b2b = new HtmlTag("span").AddClass("nba-lineup-b2b").Text("B2B");
                header.AppendHtml(new CustomTooltip(b2b.ToString(),
                    "Second night of a back to back").WithHoverTrigger().Render());
            }

            var status = new HtmlTag("span")
                .AddClass("nba-lineup-status")
                .AddClass(team.IsVerified ? "nba-lineup-status--verified" : "nba-lineup-status--projected");

            status.Append(new HtmlTag("span").AddClass("nba-lineup-dot"));
            status.Append(new HtmlTag("span").Text(team.IsVerified ? "Verified" : "Projected"));

            header.Append(status);

            return header;
        }

        private HtmlTag BuildRow(NbaLineupPlayer player)
        {
            var row = new HtmlTag("div").AddClass("nba-lineup-row");

            var slot = player.Slot ?? "";
            var slotTag = new HtmlTag("span")
                .AddClass("nba-lineup-slot")
                .Attr("data-pos", slot.Trim().ToUpperInvariant())
                .Text(slot);
            row.Append(slotTag);

            if (string.IsNullOrEmpty(slot))
                row.AddClass("nba-lineup-row--noslot");

            var hasPlayer = player.Player != null && !string.IsNullOrEmpty(player.Player.PlayerName);

            if (!hasPlayer)
            {
                row.AddClass("nba-lineup-row--empty");
                row.Append(new HtmlTag("span").AddClass("nba-lineup-name").Text("Not announced"));
                row.Append(new HtmlTag("span").AddClass("nba-lineup-mins"));
                return row;
            }

            if (player.IsOwned)
                row.AddClass("nba-lineup-row--owned");

            var nameCell = new HtmlTag("span").AddClass("nba-lineup-name");
            nameCell.AppendHtml(new DisplayPlayer(player.Player).Render());

            if (player.InjuryBadge != null)
                nameCell.AppendHtml(new InjuryBadge(player.InjuryBadge).Render());

            row.Append(nameCell);

            var mins = new HtmlTag("span").AddClass("nba-lineup-mins");
            if (_input.ShowProjectedMinutes && player.ProjectedMinutes.HasValue)
                mins.Text(player.ProjectedMinutes.Value.ToString("0", CultureInfo.InvariantCulture) + "m");
            row.Append(mins);

            return row;
        }
    }
}