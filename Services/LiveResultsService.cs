using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class LiveResultsService
    {
        public LiveResultsResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new LiveResultsResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;

            var picked = Value("lrgame" + suffix + "_", params_, eventTarget);

            if (!string.IsNullOrEmpty(picked))
            {
                result.SelectedGameId = picked == "all" ? null : picked;
                result.GameChanged = true;
            }
            else
            {
                string current;
                if (params_.TryGetValue("lrcurrent" + suffix, out current))
                    result.SelectedGameId = string.IsNullOrEmpty(current) ? null : current;
            }

            result.PreviousDayPressed = Pressed("lrprev" + suffix, params_, eventTarget);
            result.NextDayPressed = Pressed("lrnext" + suffix, params_, eventTarget);
            result.RefreshPressed = Pressed("lrrefresh" + suffix, params_, eventTarget);

            result.MyPlayersOnly = params_.ContainsKey("lrmine" + suffix);

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