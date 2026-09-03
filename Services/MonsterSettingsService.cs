using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class MonsterSettingsService
    {
        public MonsterSettingsResult Process(string id, Dictionary<string, string> params_,
            MonsterSettingsInput input)
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

        public MonsterSettingsResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new MonsterSettingsResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;

            result.SelectedDateRangeId = Text("msdate" + suffix, params_);
            result.SelectedProjectionSourceId = Text("msproj" + suffix, params_);
            result.SelectedValueTypeId = Text("msvaluetype" + suffix, params_);
            result.RestOfSeason = Checked("msros" + suffix, params_);
            result.ReplacementPlayers = Checked("msreplacement" + suffix, params_);
            result.AssumeGoodHealth = Checked("mshealth" + suffix, params_);

            var puntPrefix = "mspunt" + suffix + "_";
            var weightPrefix = "mspuntw" + suffix + "_";

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

            result.ShowRotoStandings = Checked("msroto" + suffix, params_);
            result.ShowH2HStandings = Checked("msh2h" + suffix, params_);
            result.SelectedLineupPriorityId = Text("mslineup" + suffix, params_);
            result.SelectedBenchHandlingId = Text("msbench" + suffix, params_);
            result.UseAdvancedStandings = Checked("msadvanced" + suffix, params_);
            result.ApplyGameLimits = Checked("msgamelimits" + suffix, params_);

            result.SelectedStatsDisplayFormatId = Text("msstatsformat" + suffix, params_);
            result.SelectedValueConsistencyId = Text("msvaluec" + suffix, params_);
            result.ShowMonsterBar = Checked("msmonsterbar" + suffix, params_);
            result.SelectedPlayerFilterId = Text("msfilter" + suffix, params_);
            result.SelectedTeamId = Text("msteam" + suffix, params_);
            result.SelectedHomeAwayId = Text("mshomeaway" + suffix, params_);

            var posPrefix = "mspos" + suffix + "_";
            foreach (var key in params_.Keys)
            {
                if (key == null) continue;
                if (!key.StartsWith(posPrefix)) continue;

                var posId = key.Substring(posPrefix.Length);
                if (posId.Length > 0) result.PositionIds.Add(posId);
            }
            result.AllPositionsSelected = result.PositionIds.Count == 0;

            result.ShowTierColumn = Checked("mstiercol" + suffix, params_);
            result.ColorByTier = Checked("mstiercolor" + suffix, params_);

            result.ValuesExpanded = Flag("mspanel" + suffix + "_values-toggle", params_);
            result.StandingsExpanded = Flag("mspanel" + suffix + "_standings-toggle", params_);
            result.TableExpanded = Flag("mspanel" + suffix + "_table-toggle", params_);
            result.ColumnsOpen = Flag("mscolumns" + suffix + "-toggle", params_);

            return result;
        }

        private static string Text(string key, Dictionary<string, string> params_)
        {
            string value;
            if (!params_.TryGetValue(key, out value)) return null;
            return (value ?? "").Trim();
        }

        private static bool Flag(string key, Dictionary<string, string> params_)
        {
            string value;
            if (!params_.TryGetValue(key, out value)) return false;
            return value == "1";
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