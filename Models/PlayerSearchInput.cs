using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class PlayerSearchInput
    {
        public string Id { get; set; } = "playerSearch";

        public List<DisplayPlayerInput> AvailablePlayers { get; set; } = new List<DisplayPlayerInput>();

        public string Placeholder { get; set; } = "Search players...";

        public string UrlFormat { get; set; }

        public int MaxResults { get; set; } = 8;

        public bool PostBackOnSelect { get; set; } = true;

        public bool ShowSelectButton { get; set; } = true;

        public string SelectButtonText { get; set; } = "Select";

        public int? SelectedPlayerId { get; set; }
    }
}