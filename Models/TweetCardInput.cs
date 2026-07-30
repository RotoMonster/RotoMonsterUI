using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum TweetMediaType
    {
        Photo,
        Video,
        Gif
    }

    public class TweetCardMedia
    {
        public TweetMediaType MediaType { get; set; } = TweetMediaType.Photo;

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public int? DurationMillis { get; set; }
    }
    public class TweetCardTeam
    {
        /// <summary>Short code shown in the selector - "PHI", "WAS", "FA".</summary>
        public string TeamCode { get; set; }

        public string Name { get; set; }

        public string ColorCode { get; set; }
    }

    public class TweetCardKeyword
    {
        public string Text { get; set; }

        public string Category { get; set; }

        public double? Weight { get; set; }
    }

    public class TweetCardPlayer
    {
        public DisplayPlayerInput DisplayPlayerInput { get; set; }

        public string PercentText { get; set; }

        public double? MatchConfidence { get; set; }
    }

    public class TweetCardInput
    {
        public long TweetId { get; set; }

        // ---- Top row ----

        public string ScreenName { get; set; }

        public string DisplayName { get; set; }

        public int? Followers { get; set; }

        public string TweetUrl { get; set; }

        public TimeSpan? TimeSinceCreated { get; set; }

        /// <summary>Profile picture shown upper-right.</summary>
        public string ProfileImageUrl { get; set; }

        public bool IsVerified { get; set; }

        public int? HeaderCount { get; set; }


        public string Text { get; set; }

        public List<TweetCardMedia> Media { get; set; } = new List<TweetCardMedia>();

        public bool HideMedia { get; set; }

        public string AiText { get; set; }

        public bool ShowAiButton { get; set; } = true;

        public bool IsScrollTarget { get; set; }


        public List<TweetCardTeam> Teams { get; set; } = new List<TweetCardTeam>();

        public string SelectedTeamCode { get; set; }

        public List<TweetCardKeyword> Keywords { get; set; } = new List<TweetCardKeyword>();

        public double DimKeywordsBelowWeight { get; set; } = 0.5;

        public List<TweetCardPlayer> Players { get; set; } = new List<TweetCardPlayer>();

  
        public int? SelectedPlayerId { get; set; }

        public NewsCardSport Sport { get; set; } = NewsCardSport.NBA;

        public bool IsDarkMode { get; set; }

        public string StatusTypeText { get; set; }

        public string StatusTypeTag { get; set; }

        public string NewsTitle { get; set; }

        public string NewsDetails { get; set; }

        public string SourceURL { get; set; }

        public bool IsUnofficial { get; set; }

        public NewsLevel NewsLevel { get; set; } = NewsLevel.Low;

        public List<string> StatusTypeOptions { get; set; } = new List<string>();

        public List<string> StatusTypeTagOptions { get; set; } = new List<string>();

        public List<NewsTagOption> AvailableNewsTags { get; set; } = new List<NewsTagOption>();

        public List<NewsTagOption> NewsTags { get; set; } = new List<NewsTagOption>();
        
        public bool ShowMatchConfidence { get; set; }
    }
}