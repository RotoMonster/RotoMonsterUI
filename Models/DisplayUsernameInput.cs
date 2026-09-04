namespace RotoMonsterUI
{
    public class DisplayUsernameInput
    {
        public string Username { get; set; }
        public int? UserId { get; set; }
        public string CssClass { get; set; }
        public string AvatarUrl { get; set; }
        public bool ShowAvatar { get; set; } = false;
        public int? TotalPostCount { get; set; }

        public bool LinkToProfile { get; set; } = true;
        public string ProfileUrl { get; set; } = "MessageUserProfile.aspx";
        public string ProfileTarget { get; set; }
    }
}