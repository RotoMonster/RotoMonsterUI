using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class PlayerSearchResult
    {
        public int? SelectedPlayerId { get; set; }
        public bool SelectPressed { get; set; }
    }

    public class PlayerSearchService
    {
        public PlayerSearchResult Process(string id, Dictionary<string, string> formValues)
        {
            var result = new PlayerSearchResult();
            if (formValues == null) return result;

            if (formValues.TryGetValue($"{id}-selected", out var raw)
                && !string.IsNullOrEmpty(raw)
                && int.TryParse(raw, out var playerId))
            {
                result.SelectedPlayerId = playerId;
            }

            formValues.TryGetValue("__EVENTTARGET", out var eventTarget);
            result.SelectPressed = eventTarget == $"{id}-select" || formValues.ContainsKey($"{id}-select");

            return result;
        }
    }
}