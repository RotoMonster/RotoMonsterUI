using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class TradeMonsterService
    {
        public TradeMonsterResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new TradeMonsterResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;

            var task = Value("tmtask" + suffix + "_", params_, eventTarget);

            if (!string.IsNullOrEmpty(task))
            {
                result.SelectedTask = ParseTask(task);
                result.TaskChanged = true;
            }
            else
            {
                string current;
                if (params_.TryGetValue("tmcurrent" + suffix, out current))
                    result.SelectedTask = ParseTask(current);
            }

            Collect("tmpick" + suffix + "_mine_", params_, result.MyTeamPlayerIds);
            Collect("tmpick" + suffix + "_theirs_", params_, result.OtherTeamPlayerIds);
            Collect("tmpick" + suffix + "_fa_", params_, result.FreeAgentPlayerIds);

            result.SelectedMyTeamValue = Text("tmteam" + suffix + "_mine", params_);
            result.SelectedOtherTeamValue = Text("tmteam" + suffix + "_theirs", params_);
            result.SelectedFreeAgentCountValue = Text("tmteam" + suffix + "_fa", params_);

            result.GoPressed = Pressed("tmgo" + suffix, params_, eventTarget);
            result.ClearPressed = Pressed("tmclear" + suffix, params_, eventTarget);

            return result;
        }

        private static TradeMonsterTask ParseTask(string value)
        {
            switch (value)
            {
                case "find": return TradeMonsterTask.FindTrade;
                case "adddrop": return TradeMonsterTask.CheckAddDrop;
                case "findfa": return TradeMonsterTask.FindFreeAgent;
                default: return TradeMonsterTask.CheckTrade;
            }
        }

        private static void Collect(string prefix, Dictionary<string, string> params_, List<string> into)
        {
            foreach (var key in params_.Keys)
            {
                if (key == null) continue;
                if (!key.StartsWith(prefix)) continue;

                var id = key.Substring(prefix.Length);
                if (id.Length > 0) into.Add(id);
            }
        }

        private static string Text(string key, Dictionary<string, string> params_)
        {
            string value;
            if (!params_.TryGetValue(key, out value)) return null;
            return (value ?? "").Trim();
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