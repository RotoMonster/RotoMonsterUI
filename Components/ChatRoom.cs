using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class ChatRoom
    {
        private readonly ChatRoomInput _input;

        public ChatRoom(ChatRoomInput input)
        {
            _input = input;
        }

        public string RenderAdminBar()
        {
            if (!_input.IsAdmin) return "";

            var bar = new HtmlTag("div").AddClass("chat-room-admin");

            string statusText;
            string statusClass;
            if (_input.IsPublished)
            {
                statusText = "Published";
                statusClass = "chat-room-admin-status--published";
            }
            else if (_input.SessionStatus == ChatSessionStatus.Live)
            {
                statusText = "Live";
                statusClass = "chat-room-admin-status--live";
            }
            else if (_input.SessionStatus == ChatSessionStatus.Ended)
            {
                statusText = "Ended";
                statusClass = "chat-room-admin-status--ended";
            }
            else
            {
                statusText = "Not started";
                statusClass = "chat-room-admin-status--idle";
            }

            bar.Append(new HtmlTag("span").AddClass("chat-room-admin-status").AddClass(statusClass).Text(statusText));

            if (_input.SessionStatus != ChatSessionStatus.Live && !_input.IsPublished)
            {
                bar.AppendHtml(new Button("Start Chat")
                    .WithStyle(ButtonStyle.Primary)
                    .WithName($"startchat_{_input.Id}")
                    .Render());
            }

            if (_input.SessionStatus == ChatSessionStatus.Live)
            {
                bar.AppendHtml(new Button("End Chat")
                    .WithStyle(ButtonStyle.Secondary)
                    .WithName($"endchat_{_input.Id}")
                    .WithConfirm("End the chat?")
                    .Render());
            }

            if (_input.SessionStatus == ChatSessionStatus.Ended && !_input.IsPublished)
            {
                bar.AppendHtml(new Button("Publish as Article")
                    .WithStyle(ButtonStyle.Secondary)
                    .WithName($"publishchat_{_input.Id}")
                    .WithConfirm("Publish this chat as an article?")
                    .Render());
            }

            return bar.ToString();
        }

        public string RenderControls()
        {
            var container = new HtmlTag("div").AddClass("chat-room-controls");

            if (_input.ChatState == ChatState.Watching)
            {
                var joinBtn = new Button("Join Chat")
                    .WithStyle(ButtonStyle.Secondary)
                    .WithName($"joinchat_{_input.Id}")
                    .Render();
                container.AppendHtml(joinBtn);
            }
            else
            {
                var switchBtn = new Button("Switch to Watching")
                    .WithStyle(ButtonStyle.Secondary)
                    .WithName($"switchwatching_{_input.Id}")
                    .Render();
                container.AppendHtml(switchBtn);

                var editor = new RichTextEditor(new RichTextEditorInput
                {
                    Id = $"{_input.Id}-editor",
                    Name = $"{_input.Id}-editor-value",
                    Placeholder = _input.MessagePlaceholder,
                    MinHeight = 100
                }).Render();
                container.AppendHtml(editor);

                var actionRow = new HtmlTag("div").AddClass("chat-room-actions");

                var postBtn = new Button("Post Message")
                    .WithStyle(ButtonStyle.Primary)
                    .WithName($"postmessage_{_input.Id}")
                    .Render();
                actionRow.AppendHtml(postBtn);

                if (_input.ShowInsertTeamAnalysisOption)
                {
                    var checkbox = new Checkbox()
                        .WithLabel("Insert Team Analysis at end of post")
                        .WithName($"inserttA_{_input.Id}")
                        .Render();
                    actionRow.AppendHtml(checkbox);
                }

                container.Append(actionRow);
            }

            return container.ToString();
        }

        public string RenderChatLog()
        {
            var container = new HtmlTag("div").AddClass("chat-room-log");

            var statusRow = new HtmlTag("div").AddClass("chat-room-status");

            foreach (var user in _input.ChattingUsers)
            {
                var pill = new HtmlTag("span").AddClass("chat-room-user-pill");
                pill.AppendHtml("<span class=\"chat-room-user-dot\"></span>");
                pill.AppendHtml(new DisplayUsername(user).Render());
                statusRow.Append(pill);
            }

            if (_input.WatchingCount > 0)
            {
                var watchingLabel = new HtmlTag("span")
                    .AddClass("chat-room-watching-label")
                    .Text($"+{_input.WatchingCount} watching");
                statusRow.Append(watchingLabel);
            }

            container.Append(statusRow);

            var list = new HtmlTag("div").AddClass("chat-room-messages");
            foreach (var message in _input.Messages)
                list.Append(RenderMessage(message));

            container.Append(list);

            return container.ToString();
        }

        public string Render()
        {
            var container = new HtmlTag("div").AddClass("chat-room");
            container.AppendHtml(RenderAdminBar());
            container.AppendHtml(RenderControls());
            container.AppendHtml(RenderChatLog());
            return container.ToString();
        }

        private static string RenderChatAvatar(DisplayUsernameInput input)
        {
            var displayText = !string.IsNullOrEmpty(input.Username)
                ? input.Username
                : input.UserId.HasValue ? $"#{input.UserId}" : "";
            var initial = !string.IsNullOrEmpty(displayText) ? displayText.Substring(0, 1).ToUpper() : "?";

            var fallback = new HtmlTag("span")
                .AddClass("display-username-avatar display-username-avatar--fallback")
                .Text(initial);

            if (string.IsNullOrEmpty(input.AvatarUrl))
                return fallback.ToString();

            fallback.Style("display", "none");

            var img = new HtmlTag("img")
                .AddClass("display-username-avatar")
                .Attr("src", input.AvatarUrl)
                .Attr("alt", "")
                .Attr("onerror", "this.onerror=null;this.style.display='none';this.nextElementSibling.style.display='inline-flex';");

            return img.ToString() + fallback.ToString();
        }

        private HtmlTag RenderMessage(ChatMessageInput message)
        {
            var row = new HtmlTag("div").AddClass("chat-card");
            if (message.IsSelected)
                row.AddClass("chat-card--selected");

            var usernameInput = message.DisplayUsernameInput;

            if (usernameInput.ShowAvatar)
                row.AppendHtml($"<span class=\"chat-card-avatar\">{RenderChatAvatar(usernameInput)}</span>");

            var body = new HtmlTag("div").AddClass("chat-card-body");

            var metaRow = new HtmlTag("div").AddClass("chat-card-meta");

            var nameInput = new DisplayUsernameInput
            {
                Username = usernameInput.Username,
                UserId = usernameInput.UserId,
                CssClass = usernameInput.CssClass,
                TotalPostCount = usernameInput.TotalPostCount,
                LinkToProfile = usernameInput.LinkToProfile,
                ProfileUrl = usernameInput.ProfileUrl,
                ProfileTarget = usernameInput.ProfileTarget,
                ShowAvatar = false
            };
            metaRow.AppendHtml(new DisplayUsername(nameInput).Render());
            metaRow.AppendHtml(new TimeSince(message.TimeSinceCreated).Render());
            body.Append(metaRow);

            var text = new HtmlTag("div").AddClass("chat-card-text");
            text.AppendHtml(message.MessageText);
            body.Append(text);

            if (_input.IsAdmin && (message.IsSelected || message.CanSelect))
            {
                var adminRow = new HtmlTag("div").AddClass("chat-card-admin");

                if (message.IsSelected)
                {
                    adminRow.Append(new HtmlTag("span").AddClass("chat-card-selected-badge").Text("Selected"));
                }
                else
                {
                    adminRow.Append(new HtmlTag("button")
                        .AddClass("chat-card-select")
                        .Attr("type", "submit")
                        .Attr("name", $"selectmsg_{_input.Id}_{message.MessageId}")
                        .Text("Select"));
                }

                body.Append(adminRow);
            }

            row.Append(body);

            if (message.UserCanDelete)
            {
                var deleteBtn = new HtmlTag("button")
                    .AddClass("chat-card-delete")
                    .Attr("type", "button")
                    .Attr("data-messageid", message.MessageId.ToString())
                    .Attr("onclick", "DeleteChat(this)")
                    .Attr("aria-label", "Delete message")
                    .AppendHtml(new Icon(new IconInput { Type = IconType.Trash, Size = 15, Color = "#ef4444" }).Render());
                row.Append(deleteBtn);
            }

            return row;
        }
    }
}