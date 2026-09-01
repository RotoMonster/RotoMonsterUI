namespace RotoMonsterUI
{
    public class LiveResultsResult
    {
        public string SelectedGameId { get; set; }
        public bool GameChanged { get; set; }

        public bool PreviousDayPressed { get; set; }
        public bool NextDayPressed { get; set; }
        public bool RefreshPressed { get; set; }

        public bool MyPlayersOnly { get; set; }
    }
}