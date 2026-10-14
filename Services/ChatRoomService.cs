using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RotoMonsterUI
{
    public class ChatRoomResult
    {
        public bool JoinChatPressed { get; set; }
        public bool SwitchToWatchingPressed { get; set; }
        public bool PostMessagePressed { get; set; }
        public bool InsertTeamAnalysis { get; set; }
        public int? DeleteMessageId { get; set; }
        public string PostedMessageHtml { get; set; }
        public bool StartChatPressed { get; set; }
        public bool EndChatPressed { get; set; }
        public bool PublishChatPressed { get; set; }
        public int? SelectedMessageId { get; set; }
    }

    public class ChatRoomService
    {
        private static readonly Regex TrailingEmptyDiv = new Regex(
            @"(<div>\s*(<br\s*/?>)?\s*</div>\s*)+$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string StripTrailingEmptyDivs(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;
            return TrailingEmptyDiv.Replace(html.TrimEnd(), "").TrimEnd();
        }

        public ChatRoomResult Process(string id, Dictionary<string, string> formValues)
        {
            var result = new ChatRoomResult();

            if (formValues.ContainsKey($"joinchat_{id}"))
                result.JoinChatPressed = true;

            if (formValues.ContainsKey($"switchwatching_{id}"))
                result.SwitchToWatchingPressed = true;

            if (formValues.ContainsKey($"postmessage_{id}"))
                result.PostMessagePressed = true;

            if (formValues.ContainsKey($"startchat_{id}"))
                result.StartChatPressed = true;

            if (formValues.ContainsKey($"endchat_{id}"))
                result.EndChatPressed = true;

            if (formValues.ContainsKey($"publishchat_{id}"))
                result.PublishChatPressed = true;

            result.InsertTeamAnalysis = formValues.ContainsKey($"inserttA_{id}") && formValues[$"inserttA_{id}"] == "1";

            var editorFieldName = $"{id}-editor-value";
            if (formValues.TryGetValue(editorFieldName, out var rawValue) && !string.IsNullOrEmpty(rawValue))
            {
                try
                {
                    var bytes = Convert.FromBase64String(rawValue);
                    result.PostedMessageHtml = StripTrailingEmptyDivs(System.Text.Encoding.UTF8.GetString(bytes));
                }
                catch
                {
                    result.PostedMessageHtml = StripTrailingEmptyDivs(rawValue);
                }
            }

            var selectPrefix = $"selectmsg_{id}_";
            foreach (var key in formValues.Keys)
            {
                if (key.StartsWith(selectPrefix) && int.TryParse(key.Substring(selectPrefix.Length), out var selId))
                {
                    result.SelectedMessageId = selId;
                    break;
                }
            }

            if (formValues.TryGetValue("__EVENTTARGET", out var eventTarget) && eventTarget.StartsWith("deletechat_"))
            {
                if (int.TryParse(eventTarget.Substring("deletechat_".Length), out var targetMsgId))
                    result.DeleteMessageId = targetMsgId;
            }
            else
            {
                foreach (var key in formValues.Keys)
                {
                    if (key.StartsWith($"deletemsg_{id}_") && int.TryParse(key.Substring($"deletemsg_{id}_".Length), out var msgId))
                    {
                        result.DeleteMessageId = msgId;
                        break;
                    }
                }
            }

            return result;
        }
    }
}