using System;
using HtmlTags;

namespace RotoMonsterUI
{
    public class ForumPostCard
    {
        private ForumPostCardInput _input;

        public ForumPostCard(ForumPostCardInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var card = new HtmlTag("div").AddClass("forum-post-card");

            var ageShadeColor = ColorHelper.GetAgeShadeHex(_input.TimeSinceCreated);
            bool isShaded = ageShadeColor != null;
            // Only override while it's shaded - once it expires the card's normal border comes back.
            if (isShaded)
            {
                card.AddClass("card-age-shade");
                card.Attr("style", $"border-color:{ageShadeColor};");
            }

            // Username row (with avatar + post count via DisplayUsername) + New badge + TimeSince
            var usernameRow = new HtmlTag("div").AddClass("forum-post-card-username d-flex align-items-center gap-2");
            _input.DisplayUsernameInput.ShowAvatar = true;

            if (_input.IsNew)
                usernameRow.AppendHtml(new Badge(new BadgeInput { BadgeText = "New", ColorClass = "badge-new" }).Render());

            usernameRow.AppendHtml(new DisplayUsername(_input.DisplayUsernameInput).Render());

            if (_input.TimeSinceCreated.HasValue)
            {
                var timeSince = new HtmlTag("span")
                    .AddClass("forum-post-card-time")
                    .AppendHtml(new TimeSinceBadge(_input.TimeSinceCreated.Value).Render());
                usernameRow.Append(timeSince);
            }

            card.Append(usernameRow);

            // Post text - rendered as HTML since posts can contain formatting (lists, links) from the rich text editor
            var postText = new HtmlTag("div").AddClass("forum-post-card-text").AppendHtml(_input.PostText);
            card.Append(postText);

            // Actions row - vote controls, edit, delete
            var actionsRow = new HtmlTag("div").AddClass("forum-post-card-actions");

            if (_input.ShowUpDownControls)
            {
                var voteControl = new VoteControl(new VoteControlInput
                {
                    Id = _input.PostId.ToString(),
                    NamePrefix = "forum",
                    CanVote = _input.CanVote,
                    UpVoteCount = _input.UpVoteCount,
                    DownVoteCount = _input.DownVoteCount,
                    VotedUp = _input.UserVoteInput != null && _input.UserVoteInput.HasVoted && _input.UserVoteInput.VotedUp,
                    VotedDown = _input.UserVoteInput != null && _input.UserVoteInput.HasVoted && _input.UserVoteInput.VotedDown,
                    ForceDarkText = false
                }).Render();
                actionsRow.AppendHtml(voteControl);
            }

            if (_input.UserCanEdit)
            {
                var editBtn = new HtmlTag("button")
                    .AddClass("forum-post-card-btn forum-post-card-btn-edit")
                    .Attr("type", "button")
                    .Attr("name", $"forumedit_{_input.PostId}")
                    .Attr("data-postid", _input.PostId.ToString())
                    .Attr("onclick", "TriggerPostBack(this, 'forumedit_', 'data-postid')")
                    .Attr("aria-label", "Edit post");
                editBtn.AppendHtml(new Icon(new IconInput { Type = IconType.Edit, Size = 15 }).Render());
                editBtn.AppendHtml("Edit");
                actionsRow.Append(editBtn);
            }

            if (_input.UserCanDelete)
            {
                var trashIcon = new Icon(new IconInput { Type = IconType.Trash, Size = 16, Color = "#ef4444" }).Render();
                var deleteBtn = new HtmlTag("button")
                    .AddClass("forum-post-card-btn forum-post-card-btn-delete")
                    .Attr("type", "button")
                    .Attr("name", $"forumdelete_{_input.PostId}")
                    .Attr("data-postid", _input.PostId.ToString())
                    .Attr("onclick", "TriggerPostBack(this, 'forumdelete_', 'data-postid')")
                    .Attr("style", "margin-left: auto;")
                    .AppendHtml(trashIcon);
                actionsRow.Append(deleteBtn);
            }

            card.Append(actionsRow);

            return card.ToString();
        }
    }
}