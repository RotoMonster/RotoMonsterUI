using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class NbaStartingLineupsService
    {
        public NbaStartingLineupsResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new NbaStartingLineupsResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;

            result.PreviousDayPressed = Pressed("nlprev" + suffix, params_, eventTarget);
            result.NextDayPressed = Pressed("nlnext" + suffix, params_, eventTarget);
            result.RefreshPressed = Pressed("nlrefresh" + suffix, params_, eventTarget);

            result.MyPlayersOnly = params_.ContainsKey("nlmine" + suffix);
            result.ShowBench = params_.ContainsKey("nlbench" + suffix);
            result.ShowProjectedMinutes = params_.ContainsKey("nlminutes" + suffix);

            return result;
        }

        private static bool Pressed(string key, Dictionary<string, string> params_, string eventTarget)
        {
            return eventTarget == key || params_.ContainsKey(key);
        }
    }
}