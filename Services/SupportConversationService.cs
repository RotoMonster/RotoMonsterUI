using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class SupportConversationService
    {
        public SupportConversationResult Process(string id, Dictionary<string, string> params_)
        {
            var result = new SupportConversationResult();
            if (params_ == null || !params_.ContainsKey(id + "_send")) return result;

            result.ReplyPressed = true;
            result.ReplyText = params_.TryGetValue(id + "_reply", out var text) ? (text ?? "").Trim() : "";

            if (result.ReplyText.Length < 2) result.ReplyError = "Write a reply before sending.";
            else if (result.ReplyText.Length > 5000) result.ReplyError = "Keep your reply under 5000 characters.";

            result.IsValid = result.ReplyError == null;
            return result;
        }
    }
}
