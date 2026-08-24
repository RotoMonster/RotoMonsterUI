using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class CollapseControlService
    {
        public CollapseControlResult Process(string controlId, Dictionary<string, string> formValues)
        {
            var result = new CollapseControlResult();
            var key = $"{controlId}-toggle";

            if (formValues.ContainsKey(key))
                result.IsExpanded = formValues[key] == "1";

            var lockKey = $"{controlId}-lock";

            if (formValues.ContainsKey(lockKey))
                result.IsLocked = formValues[lockKey] == "1";

            string eventTarget;
            if (formValues.TryGetValue("__EVENTTARGET", out eventTarget))
                result.LockChanged = eventTarget == $"{controlId}-lock-btn";

            return result;
        }
    }
}