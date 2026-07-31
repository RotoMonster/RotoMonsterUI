using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class CustomValuesService
    {
        public CustomValuesResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new CustomValuesResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;

            string selectedOption;
            if (params_.TryGetValue("cvvalue" + suffix, out selectedOption))
                result.SelectedOptionId = selectedOption;

            string selectedType;
            if (params_.TryGetValue("cvtype" + suffix, out selectedType))
                result.SelectedType = selectedType == "total"
                    ? CustomValueType.TotalGames
                    : CustomValueType.PerGame;

            if (params_.ContainsKey("cvcol" + suffix + "_rank"))
                result.SelectedColumns.Add(CustomValueColumn.Rank);
            if (params_.ContainsKey("cvcol" + suffix + "_games"))
                result.SelectedColumns.Add(CustomValueColumn.Games);
            if (params_.ContainsKey("cvcol" + suffix + "_mg"))
                result.SelectedColumns.Add(CustomValueColumn.MinutesPerGame);

            result.AddPressed = Pressed("cvadd" + suffix, params_, eventTarget);
            result.UseDefaultsPressed = Pressed("cvdefaults" + suffix, params_, eventTarget);
            result.DefaultOrderPressed = Pressed("cvorder" + suffix, params_, eventTarget);

            result.MoveUpIndex = RowIndex("cvup" + suffix + "_", params_, eventTarget);
            result.MoveDownIndex = RowIndex("cvdown" + suffix + "_", params_, eventTarget);
            result.RemoveIndex = RowIndex("cvremove" + suffix + "_", params_, eventTarget);

            return result;
        }

        private static bool Pressed(string key, Dictionary<string, string> params_, string eventTarget)
        {
            return eventTarget == key || params_.ContainsKey(key);
        }

        private static int? RowIndex(string prefix, Dictionary<string, string> params_, string eventTarget)
        {
            string key = null;

            if (eventTarget.StartsWith(prefix)) key = eventTarget;
            else key = params_.Keys.FirstOrDefault(k => k.StartsWith(prefix));

            if (key == null) return null;

            int index;
            if (!int.TryParse(key.Substring(prefix.Length), out index)) return null;

            return index;
        }
    }
}