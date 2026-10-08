using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class SupportForm
    {
        private readonly SupportFormInput _input;

        public SupportForm(SupportFormInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var id = _input.Id;
            var wrap = new HtmlTag("div").AddClass("support-form");

            if (!string.IsNullOrEmpty(_input.ErrorMessage))
                wrap.Append(new HtmlTag("div").AddClass("support-alert support-alert-error").Text(_input.ErrorMessage));

            if (_input.MyTickets != null && _input.MyTickets.Count > 0)
            {
                var mine = new HtmlTag("div").AddClass("support-card support-my-tickets");
                mine.Append(new HtmlTag("div").AddClass("support-card-header").Text("Your requests"));
                var list = new HtmlTag("div").AddClass("support-ticket-list");
                foreach (var t in _input.MyTickets)
                {
                    var link = new HtmlTag("a").AddClass("support-ticket-link").Attr("href", t.Url ?? "#");
                    link.Append(new HtmlTag("span").Text("#" + t.TicketNumber + " · " + t.Subject));
                    if (t.CreatedDate.HasValue)
                        link.Append(new HtmlTag("small").AddClass("support-muted").Text(t.CreatedDate.Value.ToString("MMM d")));
                    list.Append(link);
                }
                mine.Append(list);
                wrap.Append(mine);
            }

            var card = new HtmlTag("div").AddClass("support-card");
            card.Append(new HtmlTag("div").AddClass("support-card-header").Text("Contact " + _input.SiteLabel + " support"));
            var body = new HtmlTag("div").AddClass("support-card-body");

            if (!string.IsNullOrEmpty(_input.IntroText))
                body.Append(new HtmlTag("p").AddClass("support-muted support-intro").Text(_input.IntroText));

            var row = new HtmlTag("div").AddClass("support-row");
            row.Append(Field("Name", id + "_name", "text", _input.Name, "name", 80));
            row.Append(Field("Email", id + "_email", "email", _input.Email, "email", 256));
            body.Append(row);

            var categoryGroup = new HtmlTag("div").AddClass("support-field");
            categoryGroup.Append(new HtmlTag("label").AddClass("support-label").Attr("for", id + "_category").Text("What's it about?"));
            var select = new HtmlTag("select").AddClass("support-input").Attr("id", id + "_category").Attr("name", id + "_category");
            select.Append(new HtmlTag("option").Attr("value", "").Text("Choose one"));
            foreach (var c in _input.Categories ?? new List<string>())
            {
                var option = new HtmlTag("option").Attr("value", c).Text(c);
                if (c == _input.Category) option.Attr("selected", "selected");
                select.Append(option);
            }
            categoryGroup.Append(select);
            AppendError(categoryGroup, "category");
            body.Append(categoryGroup);

            body.Append(Field("Subject", id + "_subject", "text", _input.Subject, "subject", 150));

            var detailsGroup = new HtmlTag("div").AddClass("support-field");
            detailsGroup.Append(new HtmlTag("label").AddClass("support-label").Attr("for", id + "_description").Text("Details"));
            detailsGroup.Append(new HtmlTag("textarea").AddClass("support-input support-textarea")
                .Attr("id", id + "_description").Attr("name", id + "_description").Attr("rows", "7").Attr("maxlength", "5000")
                .Text(_input.Description ?? ""));
            AppendError(detailsGroup, "description");
            body.Append(detailsGroup);

            var trap = new HtmlTag("div").Attr("style", "position:absolute; left:-10000px;").Attr("aria-hidden", "true");
            trap.Append(new HtmlTag("input").Attr("type", "text").Attr("name", id + "_website").Attr("tabindex", "-1").Attr("autocomplete", "off"));
            body.Append(trap);

            body.Append(new HtmlTag("button").AddClass("support-btn support-btn-primary")
                .Attr("type", "submit").Attr("name", id + "_submit").Text(_input.SubmitText));

            card.Append(body);
            wrap.Append(card);
            return wrap.ToString();
        }

        private HtmlTag Field(string label, string name, string type, string value, string errorKey, int maxLength)
        {
            var group = new HtmlTag("div").AddClass("support-field");
            group.Append(new HtmlTag("label").AddClass("support-label").Attr("for", name).Text(label));
            group.Append(new HtmlTag("input").AddClass("support-input")
                .Attr("type", type).Attr("id", name).Attr("name", name).Attr("maxlength", maxLength.ToString())
                .Attr("value", value ?? ""));
            AppendError(group, errorKey);
            return group;
        }

        private void AppendError(HtmlTag group, string key)
        {
            if (_input.FieldErrors != null && _input.FieldErrors.TryGetValue(key, out var message) && !string.IsNullOrEmpty(message))
            {
                group.AddClass("support-field-invalid");
                group.Append(new HtmlTag("span").AddClass("support-field-error").Text(message));
            }
        }
    }
}
