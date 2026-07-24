using System.Collections.Generic;

namespace RotoMonsterUI
{

    public class TweetCardResult
    {
        public long? TweetId { get; set; }

        public int? SelectedPlayerId { get; set; }

        public bool PlayerSelectionChanged { get; set; }

        public string SelectedTeamCode { get; set; }

        public bool PostPressed { get; set; }

        public bool SetTagPressed { get; set; }

        public string StatusTypeText { get; set; }

        public string StatusTypeTag { get; set; }

        public string NewsTitle { get; set; }

        public string SourceURL { get; set; }

        public string NewsDetails { get; set; }

        public bool IsUnofficial { get; set; }

        public NewsLevel? NewsLevel { get; set; }

        public List<int> CheckedNewsTagIds { get; set; } = new List<int>();
    }
}