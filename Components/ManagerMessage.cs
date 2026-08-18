using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class ManagerMessages
    {
        private readonly ManagerMessagesInput _input;

        public ManagerMessages(ManagerMessagesInput input)
        {
            _input = input;
        }

        public static string DismissPrefix(string controlId)
        {
            return $"{controlId}_dismiss_";
        }

        public static string DismissName(string controlId, int messageId)
        {
            return DismissPrefix(controlId) + messageId;
        }

        public string Render()
        {
            var messages = _input.Messages ?? new List<ManagerMessage>();
            if (messages.Count == 0) return "";

            var list = new HtmlTag("div").AddClass("manager-messages");

            foreach (var message in messages)
            {
                if (message == null) continue;

                var row = new HtmlTag("div").AddClass("manager-message");
                var body = new HtmlTag("div").AddClass("manager-message-body");

                if (!string.IsNullOrEmpty(message.SubjectHtml))
                    body.Append(new HtmlTag("div")
                        .AddClass("manager-message-subject")
                        .AppendHtml(message.SubjectHtml));

                if (!string.IsNullOrEmpty(message.MessageHtml))
                    body.Append(new HtmlTag("div")
                        .AddClass("manager-message-text")
                        .AppendHtml(message.MessageHtml));

                row.Append(body);

                if (_input.ShowDismiss)
                {
                    var name = DismissName(_input.Id, message.MessageId);

                    var dismiss = new HtmlTag("button")
                        .AddClass("manager-message-dismiss")
                        .Attr("type", "button")
                        .Attr("name", name)
                        .Attr("onclick", "__doPostBack('" + name + "','',this.form)")
                        .Text("Dismiss");

                    row.Append(dismiss);
                }

                list.Append(row);
            }


            return list.ToString();
        }
    }
}