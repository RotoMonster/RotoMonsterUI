using System;

namespace RotoMonsterUI
{
    public class ForumPostCardInput
    {
        public int PostId { get; set; }
        public DisplayUsernameInput DisplayUsernameInput { get; set; }
        public string PostText { get; set; }
        public TimeSpan? TimeSinceCreated { get; set; }
        public int? AgeShadeSeconds { get; set; }

        // We don't build the actual edit form here - Ken just needs to know the user pressed Edit.
        public bool UserCanEdit { get; set; }

        public UserVoteInput UserVoteInput { get; set; }
        public bool ShowUpDownControls { get; set; }
        public bool CanVote { get; set; } = true;
        public int UpVoteCount { get; set; }
        public int DownVoteCount { get; set; }
        public bool UserCanDelete { get; set; }
        public bool IsNew { get; set; } = false;
    }
}