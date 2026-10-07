using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class CommentCardService
    {
        public CommentCardResult Process(Dictionary<string, string> params_)
        {
            var result = new CommentCardResult();

            params_.TryGetValue("__EVENTTARGET", out var eventTarget);

            int? ExtractId(string prefix)
            {
                if (eventTarget != null && eventTarget.StartsWith(prefix) &&
                    int.TryParse(eventTarget.Substring(prefix.Length), out var idFromTarget))
                    return idFromTarget;

                foreach (var key in params_.Keys)
                {
                    if (key.StartsWith(prefix) && int.TryParse(key.Substring(prefix.Length), out var idFromKey))
                        return idFromKey;
                }
                return null;
            }

            result.DeleteCommentId = ExtractId("delete_");
            result.ExpandCommentId = ExtractId("expand_");

            var voteResult = new VoteControlService().Process("comment", params_);
            result.UpVoteCommentId = voteResult.UpVoteId;
            result.DownVoteCommentId = voteResult.DownVoteId;
            result.CancelVoteCommentId = voteResult.CancelVoteId;

            foreach (var key in params_.Keys)
            {
                if (key.StartsWith("post_") && int.TryParse(key.Replace("post_", ""), out int postId))
                {
                    result.PostPressed = true;
                    result.PostCommentId = postId;
                }
                else if (key.StartsWith("comment_"))
                    result.UserComment = params_[key];
            }

            foreach (var key in params_.Keys)
            {
                if (key.StartsWith("replysubmit_") && int.TryParse(key.Substring("replysubmit_".Length), out var replyId))
                {
                    params_.TryGetValue("reply_" + replyId, out var text);
                    text = (text ?? "").Trim();
                    if (text.Length > 0)
                    {
                        result.ReplyPressed = true;
                        result.ReplyCommentId = replyId;
                        result.ReplyText = text;
                    }
                    break;
                }
            }

            return result;
        }
    }
}
