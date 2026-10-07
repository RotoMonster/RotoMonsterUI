using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class UserCommentCardInput
    {
        public int CommentId { get; set; }
        public DisplayPlayerInput DisplayPlayerInput { get; set; }
        public DisplayUsernameInput DisplayUsernameInput { get; set; }
        public UserVoteInput UserVoteInput { get; set; }
        public string CommentText { get; set; }
        public bool ShowUpDownControls { get; set; }
        public bool CanVote { get; set; } = true;

        public int UpVoteCount { get; set; }
        public int DownVoteCount { get; set; }
        public bool UserCanDelete { get; set; }
        public bool UserCanPostComment { get; set; }
        public bool IsCommentExpanded { get; set; }
        public string CurrentCommentText { get; set; }
        public TimeSpan? TimeSinceCreated { get; set; }
        public bool ShowPlayerInfo { get; set; } = true;
        public bool ShowViewAll { get; set; } = false;
        public bool IsNew { get; set; } = false;

        public NewsCardSport Sport { get; set; } = NewsCardSport.NBA;
        public bool IsDarkMode { get; set; }

        public bool ShowReply { get; set; } = false;
        public string ReplyButtonText { get; set; } = "Reply";
        public string ReplyPlaceholder { get; set; } = "Write a reply...";
        public string ReplySubmitText { get; set; } = "Submit";
        public string ReplyCancelText { get; set; } = "Cancel";
        public int ReplyMaxLength { get; set; } = 1000;
    }
}
