using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class DraftMonsterOptionsService
    {
        /// <summary>
        /// Pass the same input you rendered with and the column picker and
        /// custom values are read too.
        /// </summary>
        public DraftMonsterOptionsResult Process(string id, Dictionary<string, string> params_,
            DraftMonsterOptionsInput input)
        {
            var result = Process(id, params_);

            if (input == null) return result;

            if (input.ColumnsInput != null)
                result.Columns = new DisplayColumnsService()
                    .Process(input.ColumnsInput.Id, params_);

            if (input.CustomValuesInput != null)
                result.CustomValues = new CustomValuesService()
                    .Process(input.CustomValuesInput.Id, params_);

            return result;
        }

        public DraftMonsterOptionsResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new DraftMonsterOptionsResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;

            var settings = new MonsterSettingsService().Process(id + "settings", params_);

            result.SelectedProjectionSourceId = settings.SelectedProjectionSourceId;
            result.SelectedValueTypeId = settings.SelectedValueTypeId;
            result.ReplacementPlayers = settings.ReplacementPlayers;
            result.AssumeGoodHealth = settings.AssumeGoodHealth;
            result.PuntCategoryIds = settings.PuntCategoryIds;
            result.PuntWeights = settings.PuntWeights;

            result.ShowRotoStandings = settings.ShowRotoStandings;
            result.ShowH2HStandings = settings.ShowH2HStandings;
            result.UseAdvancedStandings = settings.UseAdvancedStandings;
            result.ApplyGameLimits = settings.ApplyGameLimits;

            result.SelectedStatsDisplayFormatId = settings.SelectedStatsDisplayFormatId;
            result.SelectedValueConsistencyId = settings.SelectedValueConsistencyId;
            result.SelectedPlayerFilterId = settings.SelectedPlayerFilterId;
            result.SelectedTeamId = settings.SelectedTeamId;
            result.SelectedHomeAwayId = settings.SelectedHomeAwayId;
            result.ColumnsPressed = settings.ColumnsPressed;
            result.PositionIds = settings.PositionIds;
            result.AllPositionsSelected = settings.AllPositionsSelected;

            // Already flipped by MonsterSettingsService if one was toggled.
            result.ValuesExpanded = settings.ValuesExpanded;
            result.StandingsSettingsExpanded = settings.StandingsExpanded;
            result.TableSettingsExpanded = settings.TableExpanded;

            result.PickNumber = Text("dmpick" + suffix, params_);
            result.ThirdRoundReversal = Checked("dmrev3" + suffix, params_);
            result.SecondRoundHighToLow = Checked("dmrev2" + suffix, params_);
            result.FifthRoundReversal = Checked("dmrev5" + suffix, params_);

            result.ConnectPressed = Pressed("dmconnect" + suffix, params_, eventTarget);
            result.ChangePickPressed = Pressed("dmchangepick" + suffix, params_, eventTarget);
            result.RefreshPressed = Pressed("dmrefresh" + suffix, params_, eventTarget);

            result.HideDraftedPlayers = Checked("dmhidedrafted" + suffix, params_);
            result.HighlightDraftedSinceImport = Checked("dmhighlight" + suffix, params_);
            result.IncludeTargetsInAnalysis = Checked("dmtargets" + suffix, params_);
            result.ShowStatFilters = Checked("dmstatfilters" + suffix, params_);

            result.StandingsExpanded = Checked("dmopen" + suffix + "_standings", params_);
            result.TeamAnalysisExpanded = Checked("dmopen" + suffix + "_analysis", params_);

            // Same as the settings panels - the hidden field is the state it
            // was rendered in, so a toggle needs flipping before the caller
            // sees it. Assigning the result back now just works.
            var toggledOutput = Value("dmtoggle" + suffix + "_", params_, eventTarget);

            if (toggledOutput == "standings")
                result.StandingsExpanded = !result.StandingsExpanded;
            else if (toggledOutput == "analysis")
                result.TeamAnalysisExpanded = !result.TeamAnalysisExpanded;

            result.StandingsCompact = Checked("dmcompact" + suffix + "_standings", params_);
            result.TeamAnalysisCompact = Checked("dmcompact" + suffix + "_analysis", params_);

            result.ToggledSection = Value("dmtoggle" + suffix + "_", params_, eventTarget);

            if (string.IsNullOrEmpty(result.ToggledSection)
                && !string.IsNullOrEmpty(settings.ToggledPanel))
            {
                // the shared component names its standings panel "standings",
                // which is our team analysis section's neighbour, so map it
                switch (settings.ToggledPanel)
                {
                    case "values": result.ToggledSection = "values"; break;
                    case "standings": result.ToggledSection = "standingset"; break;
                    case "table": result.ToggledSection = "table"; break;
                }
            }

            return result;
        }

        private static string Text(string key, Dictionary<string, string> params_)
        {
            string value;
            if (!params_.TryGetValue(key, out value)) return null;
            return (value ?? "").Trim();
        }

        private static bool Checked(string key, Dictionary<string, string> params_)
        {
            return params_.ContainsKey(key);
        }

        private static bool Pressed(string key, Dictionary<string, string> params_, string eventTarget)
        {
            return eventTarget == key || params_.ContainsKey(key);
        }

        private static string Value(string prefix, Dictionary<string, string> params_, string eventTarget)
        {
            string key = null;

            if (eventTarget.StartsWith(prefix)) key = eventTarget;
            else key = params_.Keys.FirstOrDefault(k => k != null && k.StartsWith(prefix));

            if (key == null) return null;

            var value = key.Substring(prefix.Length);
            return value.Length > 0 ? value : null;
        }
    }
}