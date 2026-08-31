using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class DraftMonsterOptionsService
    {
        public DraftMonsterOptionsResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new DraftMonsterOptionsResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;

            result.SelectedProjectionSourceId = Text("dmproj" + suffix, params_);
            result.SelectedValueTypeId = Text("dmvaluetype" + suffix, params_);
            result.ReplacementPlayers = Checked("dmreplacement" + suffix, params_);
            result.AssumeGoodHealth = Checked("dmhealth" + suffix, params_);

            var puntPrefix = "dmpunt" + suffix + "_";
            var weightPrefix = "dmpuntw" + suffix + "_";

            foreach (var key in params_.Keys)
            {
                if (key == null) continue;

                if (key.StartsWith(weightPrefix))
                {
                    var wid = key.Substring(weightPrefix.Length);
                    var wval = (params_[key] ?? "").Trim();
                    if (wid.Length > 0 && wval.Length > 0) result.PuntWeights[wid] = wval;
                    continue;
                }

                if (key.StartsWith(puntPrefix))
                {
                    var pid = key.Substring(puntPrefix.Length);
                    if (pid.Length > 0) result.PuntCategoryIds.Add(pid);
                }
            }

            result.ShowRotoStandings = Checked("dmroto" + suffix, params_);
            result.ShowH2HStandings = Checked("dmh2h" + suffix, params_);
            result.UseAdvancedStandings = Checked("dmadvanced" + suffix, params_);
            result.ApplyGameLimits = Checked("dmgamelimits" + suffix, params_);

            result.SelectedStatsDisplayFormatId = Text("dmstatsformat" + suffix, params_);
            result.SelectedValueConsistencyId = Text("dmvaluec" + suffix, params_);
            result.SelectedPlayerFilterId = Text("dmfilter" + suffix, params_);
            result.SelectedTeamId = Text("dmteam" + suffix, params_);
            result.SelectedHomeAwayId = Text("dmhomeaway" + suffix, params_);

            var posPrefix = "dmpos" + suffix + "_";
            foreach (var key in params_.Keys)
            {
                if (key == null) continue;
                if (!key.StartsWith(posPrefix)) continue;

                var posId = key.Substring(posPrefix.Length);
                if (posId.Length > 0) result.PositionIds.Add(posId);
            }
            result.AllPositionsSelected = result.PositionIds.Count == 0;

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
            result.ValuesExpanded = Checked("dmopen" + suffix + "_values", params_);
            result.StandingsSettingsExpanded = Checked("dmopen" + suffix + "_standingset", params_);
            result.TableSettingsExpanded = Checked("dmopen" + suffix + "_table", params_);

            result.StandingsCompact = Checked("dmcompact" + suffix + "_standings", params_);
            result.TeamAnalysisCompact = Checked("dmcompact" + suffix + "_analysis", params_);

            result.ToggledSection = Value("dmtoggle" + suffix + "_", params_, eventTarget);

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