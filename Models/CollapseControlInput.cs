namespace RotoMonsterUI
{
    public class CollapseControlInput
    {
        public string Id { get; set; }
        public string ButtonText { get; set; }
        public ButtonStyle ButtonStyle { get; set; } = ButtonStyle.Secondary;
        public string CollapsibleHtml { get; set; }
        public bool IsExpanded { get; set; } = false;

        public bool ShowLock { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public bool LockPostsBack { get; set; } = true;
        public string LockTitle { get; set; } = "Pin this open";
        public string UnlockTitle { get; set; } = "Unpin";
    }
}