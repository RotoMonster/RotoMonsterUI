using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class DraftMonsterOptionsService
    {
        public DraftMonsterOptionsResult Process(string id, Dictionary<string, string> params_,
            DraftMonsterOptionsInput input)
        {
            var result = Process(id, params_);

            if (input == null) return result;

            if (!string.IsNullOrEmpty(input.DraftPicksFieldId))
                ReadDraftPicks(result, params_, input.DraftPicksFieldId);

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
            result.ColumnsOpen = settings.ColumnsOpen;
            result.PositionIds = settings.PositionIds;
            result.AllPositionsSelected = settings.AllPositionsSelected;
            result.ShowTierColumn = settings.ShowTierColumn;
            result.ColorByTier = settings.ColorByTier;

            result.ValuesExpanded = settings.ValuesExpanded;
            result.StandingsSettingsExpanded = settings.StandingsExpanded;
            result.TableSettingsExpanded = settings.TableExpanded;

            result.PickNumber = Text("dmpick" + suffix, params_);
            result.LeagueId = Text("dmleague" + suffix, params_);
            result.SelectedDraftingTeamId = Text("dmteampick" + suffix, params_);
            result.ThirdRoundReversal = Checked("dmrev3" + suffix, params_);
            result.SecondRoundHighToLow = Checked("dmrev2" + suffix, params_);
            result.FifthRoundReversal = Checked("dmrev5" + suffix, params_);

            result.ConnectPressed = Pressed("dmconnect" + suffix, params_, eventTarget);

            ReadDraftPicks(result, params_, null);
            result.ChangePickPressed = Pressed("dmchangepick" + suffix, params_, eventTarget);
            result.RefreshPressed = Pressed("dmrefresh" + suffix, params_, eventTarget);

            result.HideDraftedPlayers = Checked("dmhidedrafted" + suffix, params_);
            result.HighlightDraftedSinceImport = Checked("dmhighlight" + suffix, params_);
            result.IncludeTargetsInAnalysis = Checked("dmtargets" + suffix, params_);
            result.ShowStatFilters = Checked("dmstatfilters" + suffix, params_);

            result.StandingsExpanded = Flag("dmpanel" + suffix + "_standings-toggle", params_);
            result.TeamAnalysisExpanded = Flag("dmpanel" + suffix + "_analysis-toggle", params_);

            result.StandingsCompact = Checked("dmcompact" + suffix + "_standings", params_);
            result.TeamAnalysisCompact = Checked("dmcompact" + suffix + "_analysis", params_);

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

        private static void ReadDraftPicks(DraftMonsterOptionsResult result,
            Dictionary<string, string> params_, string fieldId)
        {
            var ids = string.IsNullOrEmpty(fieldId)
                ? new[] { "espnDraftPicks", "yahooDraftPicks" }
                : new[] { fieldId };

            foreach (var id in ids)
            {
                string picks;
                if (!params_.TryGetValue(id, out picks)) continue;
                if (string.IsNullOrEmpty(picks)) continue;

                result.DraftPicksJson = picks;
                result.DraftPicksDelivered = true;
                return;
            }
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