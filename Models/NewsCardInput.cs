using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum NewsLevel
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Monster = 4
    }

    public enum NewsCardSport
    {
        MLB,
        NBA
    }

    public class NewsCardInput
    {
        public int NewsId { get; set; }


        public DisplayPlayerInput DisplayPlayerInput { get; set; }

        public DisplayTeamInput DisplayTeamInput { get; set; }

        public NewsCardSport Sport { get; set; } = NewsCardSport.MLB;

        public bool IsDarkMode { get; set; }


        public string StatusTypeText { get; set; }



        public string StatusTypeColorCode { get; set; }


        public string StatusTypeTag { get; set; }

        public string CurrentStatusTypeText { get; set; }

        public string CurrentStatusTypeAbbreviation { get; set; }

        public string CurrentStatusTypeColorCode { get; set; }

        public string CurrentStatusTypeTag { get; set; }

        public string CurrentStatusNewsTitle { get; set; }

        public bool UserCanDelete { get; set; }
        public bool UserCanEdit { get; set; }
        public TimeSpan? TimeSinceCreated { get; set; }


        public string NewsTitle { get; set; }

        public string NewsDetails { get; set; }


        public int? UserOwnCount { get; set; }

        public int? UserFreeAgentCount { get; set; }

        public bool IsEditing { get; set; }

        public bool IsUnofficial { get; set; }

        public string SourceURL { get; set; }

        public NewsLevel NewsLevel { get; set; } = NewsLevel.Low;
        public bool ShowMediumLevel { get; set; }

        public List<NewsTagOption> NewsTags { get; set; } = new List<NewsTagOption>();

        public List<string> TeamPlayerNames { get; set; } = new List<string>();

        public List<string> StatusTypeOptions { get; set; } = new List<string>();

        public List<string> StatusTypeTagOptions { get; set; } = new List<string>();

        public List<NewsTagOption> AvailableNewsTags { get; set; } = new List<NewsTagOption>();
    }
}