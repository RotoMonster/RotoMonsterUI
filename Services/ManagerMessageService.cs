using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class ManagerMessagesService
    {
        public ManagerMessagesResult Process(string controlId, Dictionary<string, string> formValues)
        {
            var result = new ManagerMessagesResult();

            var toggleKey = $"{controlId}-toggle";
            if (formValues.ContainsKey(toggleKey))
                result.IsExpanded = formValues[toggleKey] == "1";

            var prefix = ManagerMessages.DismissPrefix(controlId);

            // Dismiss fires via __doPostBack, so the control name arrives as the
            // VALUE of __EVENTTARGET, not as its own form key. Check that first.
            if (formValues.TryGetValue("__EVENTTARGET", out var eventTarget)
                && eventTarget.StartsWith(prefix)
                && int.TryParse(eventTarget.Substring(prefix.Length), out int targetId))
            {
                result.DismissedMessageId = targetId;
                return result;
            }

            // Fallback: control name posted as its own key.
            foreach (var kvp in formValues)
            {
                if (!kvp.Key.StartsWith(prefix)) continue;

                if (int.TryParse(kvp.Key.Substring(prefix.Length), out int id))
                {
                    result.DismissedMessageId = id;
                    break;
                }
            }

            return result;
        }
    }
}