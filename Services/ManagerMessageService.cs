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