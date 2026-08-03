using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class DisplayColumnsService
    {
        public DisplayColumnsResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new DisplayColumnsResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;
            var columnPrefix = "dccol" + suffix + "_";

            foreach (var key in params_.Keys)
            {
                if (key == null) continue;
                if (!key.StartsWith(columnPrefix)) continue;

                var columnId = key.Substring(columnPrefix.Length);
                if (columnId.Length > 0) result.SelectedColumnIds.Add(columnId);
            }

            result.ApplyPressed = Pressed("dcapply" + suffix, params_, eventTarget);
            result.ResetPressed = Pressed("dcreset" + suffix, params_, eventTarget);

            return result;
        }

        private static bool Pressed(string key, Dictionary<string, string> params_, string eventTarget)
        {
            return eventTarget == key || params_.ContainsKey(key);
        }
    }
}