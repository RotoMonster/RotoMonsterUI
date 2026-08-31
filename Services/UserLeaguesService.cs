using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class UserLeaguesService
    {
        public UserLeaguesResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new UserLeaguesResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;

            string selectedTab;
            if (params_.TryGetValue("ultab" + suffix, out selectedTab))
                result.SelectedTab = selectedTab;

            result.TabChangedTo = Value("ultabgo" + suffix + "_", params_, eventTarget);
            if (!string.IsNullOrEmpty(result.TabChangedTo))
                result.SelectedTab = result.TabChangedTo;

            result.ConnectProvider = Value("ulconnect" + suffix + "_", params_, eventTarget);

            if (!string.IsNullOrEmpty(result.ConnectProvider))
            {
                var fieldPrefix = "ulfield" + suffix + "_" + result.ConnectProvider + "_";

                foreach (var key in params_.Keys)
                {
                    if (key == null) continue;
                    if (!key.StartsWith(fieldPrefix)) continue;

                    var fieldName = key.Substring(fieldPrefix.Length);
                    if (fieldName.Length == 0) continue;

                    result.ConnectValues[fieldName] = (params_[key] ?? "").Trim();
                }
            }

            result.DisconnectProvider = Value("uldisconnect" + suffix + "_", params_, eventTarget);

            result.ImportProvider = Value("ulimport" + suffix + "_", params_, eventTarget);
            result.ImportPressed = !string.IsNullOrEmpty(result.ImportProvider);

            var pickPrefix = "ulpick" + suffix + "_";
            foreach (var key in params_.Keys)
            {
                if (key == null) continue;
                if (!key.StartsWith(pickPrefix)) continue;

                var leagueId = key.Substring(pickPrefix.Length);
                if (leagueId.Length > 0) result.SelectedLeagueIds.Add(leagueId);
            }

            var one = Value("ulimportone" + suffix + "_", params_, eventTarget);
            if (!string.IsNullOrEmpty(one))
            {
                var split = one.IndexOf("__");
                if (split > 0)
                {
                    result.ImportLeagueProvider = one.Substring(0, split);
                    result.ImportLeagueId = one.Substring(split + 2);
                }
            }

            result.ManualEntryProvider = Value("ulmanual" + suffix + "_", params_, eventTarget);
            if (!string.IsNullOrEmpty(result.ManualEntryProvider))
            {
                string manualId;
                if (params_.TryGetValue("ulmanualid" + suffix + "_" + result.ManualEntryProvider, out manualId))
                    result.ManualEntryLeagueId = (manualId ?? "").Trim();
            }

            result.ToggleTrackUserLeagueId = Value("ultrack" + suffix + "_", params_, eventTarget);
            result.RemoveUserLeagueId = Value("ulremove" + suffix + "_", params_, eventTarget);

            result.CreateCustomPressed = Pressed("ulcustom" + suffix, params_, eventTarget);

            return result;
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