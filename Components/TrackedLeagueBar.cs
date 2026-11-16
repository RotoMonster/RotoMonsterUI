using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class TrackedLeagueBar
    {
        private readonly List<TrackedLeagueBarInput> _leagues;

        public TrackedLeagueBar(List<TrackedLeagueBarInput> leagues)
        {
            _leagues = leagues;
        }

        public TrackedLeagueBar(TrackedLeagueBarInput league)
        {
            _leagues = new List<TrackedLeagueBarInput> { league };
        }

        private static string NormalizeProviderColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return "inherit";
            return color.StartsWith("#") ? color : "#" + color;
        }

        public string Render()
        {
            var list = new HtmlTag("div").AddClass("tracked-league-bar-list");

            foreach (var league in _leagues)
            {
                var row = new HtmlTag("div").AddClass("tracked-league-bar");

                var leagueSection = new HtmlTag("div").AddClass("tracked-league-bar-section");
                var leagueLabel = new HtmlTag("span").AddClass("tracked-league-bar-label");
                if (!string.IsNullOrEmpty(league.ProviderName))
                {
                    leagueLabel.Attr("style", $"color:{NormalizeProviderColor(league.ProviderColorCSS)}; text-transform:none;");
                    leagueLabel.Text(league.ProviderName);
                }
                else
                {
                    leagueLabel.Text("LEAGUE");
                }
                var leagueValue = new HtmlTag("span").AddClass("tracked-league-bar-value").Text(league.LeagueName ?? "");
                leagueSection.Append(leagueLabel);
                leagueSection.Append(leagueValue);
                if (!string.IsNullOrEmpty(league.StatusText))
                    leagueSection.Append(new HtmlTag("span").AddClass("tracked-league-bar-status").Text(league.StatusText));
                row.Append(leagueSection);

                var divider = new HtmlTag("div").AddClass("tracked-league-bar-divider");
                row.Append(divider);

                var ownerSection = new HtmlTag("div").AddClass("tracked-league-bar-section");
                if (string.IsNullOrEmpty(league.OwnerName))
                {
                    ownerSection.AddClass("tracked-league-bar-section--empty");
                    ownerSection.Attr("aria-label", "Unowned");
                }
                else
                {
                    if (league.IsOwner)
                        ownerSection.AddClass("tracked-league-bar-section--owner");
                    ownerSection.Append(new HtmlTag("span").AddClass("tracked-league-bar-value").Text(league.OwnerName));
                }
                row.Append(ownerSection);

                list.Append(row);
            }

            return list.ToString();
        }
    }
}