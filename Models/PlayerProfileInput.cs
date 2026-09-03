using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class PlayerProfileSection
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ContentHtml { get; set; }
        public string CountText { get; set; }
        public bool IsExpanded { get; set; } = true;
        public bool ShowPin { get; set; } = true;
        public bool IsPinned { get; set; }
        public bool PinPostsBack { get; set; } = true;
        public string PinTitle { get; set; } = "Pin this open";
        public string UnpinTitle { get; set; } = "Unpin";
        public string SeeAllUrl { get; set; }
        public string SeeAllText { get; set; } = "See all";
    }

    public class PlayerProfileLink
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public string CountText { get; set; }
    }

    public class PlayerProfileInput
    {
        public string Id { get; set; } = "playerProfile";

        public PlayerStatBadgeInput StatBadges { get; set; }

        public List<PlayerProfileLink> Links { get; set; } = new List<PlayerProfileLink>();

        public string ExpertCommentTitle { get; set; }
        public string ExpertCommentHtml { get; set; }
        public string ExpertCommentByText { get; set; }

        public string NewsTitle { get; set; } = "Recent news";
        public NewsCardInput NewsCard { get; set; }
        public string NewsHtml { get; set; }
        public string NewsSeeAllUrl { get; set; }

        public PlayerProfileSection TrackedLeagues { get; set; }

        public string CommentsTitle { get; set; } = "User comments";
        public string CommentsCountText { get; set; }
        public bool ShowCommentsPin { get; set; } = true;
        public bool CommentsPinned { get; set; }
        public bool CommentsPinPostsBack { get; set; } = true;
        public bool CommentsExpanded { get; set; } = true;
        public int CommentsMaxHeight { get; set; } = 620;
        public bool ShowAddComment { get; set; } = true;
        public string AddCommentText { get; set; } = "Add a comment";
        public List<UserCommentCardInput> Comments { get; set; } = new List<UserCommentCardInput>();
        public string CommentsHtml { get; set; }
        public string NoCommentsText { get; set; } = "No comments yet.";

        public string BelowHtml { get; set; }

        public string Message { get; set; }
    }
}