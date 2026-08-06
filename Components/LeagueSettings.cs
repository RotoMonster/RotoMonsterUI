using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class LeagueSettings
    {
        private readonly LeagueSettingsInput _input;

        public LeagueSettings(LeagueSettingsInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var wrapper = new HtmlTag("div").AddClass("league-settings-display");

            var activeCategories = _input.Categories.Where(c => c.IsActive).ToList();
            var puntedCategories = _input.Categories.Where(c => !c.IsActive).ToList();

            var summary = new HtmlTag("span").AddClass("league-settings-summary");
            summary.AppendHtml($"{_input.ScoringType} &middot; {_input.NumTeams} Teams &middot; {_input.PlayersPerTeam} Per Team");
            wrapper.Append(summary);

            if (activeCategories.Count > 0)
            {
                var activeRow = new HtmlTag("span").AddClass("league-settings-categories league-settings-categories--active");
                activeRow.AppendHtml($"{activeCategories.Count} Active = {string.Join(" ", activeCategories.Select(Label))}");
                wrapper.Append(activeRow);
            }

            if (puntedCategories.Count > 0)
            {
                var puntedRow = new HtmlTag("span").AddClass("league-settings-categories league-settings-categories--punted");
                puntedRow.AppendHtml($"{puntedCategories.Count} Punted = {string.Join(" ", puntedCategories.Select(Label))}");
                wrapper.Append(puntedRow);
            }

            return wrapper.ToString();
        }

        private static string Label(LeagueSettingsCategory category)
        {
            if (category.PointsPerStat == 0)
                return category.Abbreviation;

            return category.Abbreviation + " (" + category.PointsPerStat.ToString("0.####") + ")";
        }
    }
}