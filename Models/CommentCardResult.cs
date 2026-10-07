namespace RotoMonsterUI
{
    public class CommentCardResult
    {
        public int? UpVoteCommentId { get; set; }
        public int? DownVoteCommentId { get; set; }
        public int? CancelVoteCommentId { get; set; }
        public int? DeleteCommentId { get; set; }
        public int? ExpandCommentId { get; set; }
        public bool PostPressed { get; set; } = false;
        public string UserComment { get; set; }
        public int? PostCommentId { get; set; }

        public bool ReplyPressed { get; set; } = false;
        public int? ReplyCommentId { get; set; }
        public string ReplyText { get; set; }
    }
}
