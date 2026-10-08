using System.Net;
using HtmlTags;

namespace RotoMonsterUI
{
    public class SupportConversation
    {
        private readonly SupportConversationInput _input;

        public SupportConversation(SupportConversationInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var id = _input.Id;
            var closed = _input.Status == SupportTicketStatus.Closed;
            var wrap = new HtmlTag("div").AddClass("support-conversation");

            if (_input.JustCreated)
                wrap.Append(new HtmlTag("div").AddClass("support-alert support-alert-success")
                    .Text("Your request was sent. " + (_input.Emailed ? "We've emailed you a link to this page." : "Bookmark this page to check on it.")));
            if (_input.JustReplied)
                wrap.Append(new HtmlTag("div").AddClass("support-alert support-alert-success").Text("Your reply was sent."));
            if (!string.IsNullOrEmpty(_input.ErrorMessage))
                wrap.Append(new HtmlTag("div").AddClass("support-alert support-alert-error").Text(_input.ErrorMessage));

            var header = new HtmlTag("div").AddClass("support-card support-ticket-header");
            var left = new HtmlTag("div");
            left.Append(new HtmlTag("div").AddClass("support-muted support-small").Text("Request #" + _input.TicketNumber));
            left.Append(new HtmlTag("div").AddClass("support-ticket-subject").Text(_input.Subject ?? ""));
            if (_input.OpenedDate.HasValue)
                left.Append(new HtmlTag("div").AddClass("support-muted support-small").Text("Opened " + _input.OpenedDate.Value.ToString("MMM d, yyyy h:mm tt")));
            header.Append(left);

            string statusClass, statusText;
            switch (_input.Status)
            {
                case SupportTicketStatus.Closed: statusClass = "support-status-closed"; statusText = "Closed"; break;
                case SupportTicketStatus.WaitingOnCustomer: statusClass = "support-status-waiting"; statusText = "Waiting on you"; break;
                default: statusClass = "support-status-open"; statusText = "Open"; break;
            }
            header.Append(new HtmlTag("span").AddClass("support-status " + statusClass).Text(statusText));
            wrap.Append(header);

            if (_input.LoadFailed)
            {
                wrap.Append(new HtmlTag("div").AddClass("support-alert support-alert-warning")
                    .Text("We couldn't load the latest on this request right now. Please try again in a minute."));
            }
            else
            {
                var thread = new HtmlTag("div").AddClass("support-thread");
                foreach (var m in _input.Messages)
                {
                    var message = new HtmlTag("div").AddClass("support-message").AddClass(m.FromCustomer ? "support-message-customer" : "support-message-staff");
                    var top = new HtmlTag("div").AddClass("support-message-header");
                    top.Append(new HtmlTag("span").AddClass("support-message-author").Text(m.FromCustomer ? "You" : _input.StaffName));
                    if (m.Date.HasValue)
                        top.Append(new HtmlTag("span").AddClass("support-muted").Text(m.Date.Value.ToString("MMM d, h:mm tt")));
                    message.Append(top);
                    var content = !string.IsNullOrEmpty(m.SafeHtml)
                        ? m.SafeHtml
                        : WebUtility.HtmlEncode(m.Text ?? "").Replace("\n", "<br>");
                    message.Append(new HtmlTag("div").AddClass("support-message-body").AppendHtml(content));
                    thread.Append(message);
                }
                wrap.Append(thread);
            }

            var reply = new HtmlTag("div").AddClass("support-card");
            reply.Append(new HtmlTag("div").AddClass("support-card-header").Text(closed ? "Reopen this request" : "Add a reply"));
            var body = new HtmlTag("div").AddClass("support-card-body");
            var field = new HtmlTag("div").AddClass("support-field");
            field.Append(new HtmlTag("textarea").AddClass("support-input support-textarea")
                .Attr("name", id + "_reply").Attr("rows", "5").Attr("maxlength", "5000")
                .Attr("aria-label", closed ? "Reopen this request" : "Add a reply")
                .Text(_input.ReplyText ?? ""));
            if (!string.IsNullOrEmpty(_input.ReplyError))
            {
                field.AddClass("support-field-invalid");
                field.Append(new HtmlTag("span").AddClass("support-field-error").Text(_input.ReplyError));
            }
            body.Append(field);
            body.Append(new HtmlTag("button").AddClass("support-btn support-btn-primary")
                .Attr("type", "submit").Attr("name", id + "_send").Text(_input.SendText));
            reply.Append(body);
            wrap.Append(reply);

            if (!string.IsNullOrEmpty(_input.NewRequestUrl))
            {
                var footer = new HtmlTag("p").AddClass("support-muted support-small support-footer");
                footer.Append(new HtmlTag("a").Attr("href", _input.NewRequestUrl).Text("Contact support about something else"));
                wrap.Append(footer);
            }

            return wrap.ToString();
        }
    }
}
