using System;
using HtmlTags;

namespace RotoMonsterUI
{
    public class CommentCard
    {
        private UserCommentCardInput _input;

        public CommentCard(UserCommentCardInput input)
        {
            _input = input;
        }

        private static string NewBadgeHtml()
        {
            return new Badge(new BadgeInput { BadgeText = "New", ColorClass = "badge-new" }).Render();
        }

        public string Render()
        {
            if (_input.ShowPlayerInfo && _input.DisplayPlayerInput != null
                && string.IsNullOrEmpty(_input.DisplayPlayerInput.TeamColor))
            {

                _input.DisplayPlayerInput.TeamColor = _input.Sport == NewsCardSport.NBA
                    ? TeamColorHelper.GetNbaTeamColorVar(_input.DisplayPlayerInput.TeamCode)
                    : TeamColorHelper.GetTeamColorVar(_input.DisplayPlayerInput.TeamCode);
            }

            var card = new HtmlTag("div").AddClass("comment-card").AddClass("card-age-shade");

            var ageShadeColor = ColorHelper.GetAgeShadeHex(_input.TimeSinceCreated);
            bool isShaded = ageShadeColor != null;
            // Width lives in CSS so it's one place to adjust; only the color changes here.
            // Unshaded cards keep a transparent border of the same width so nothing shifts as a card ages.
            card.Attr("style", $"border-color:{(isShaded ? ageShadeColor : "transparent")};");

            // Player title row
            if (_input.ShowPlayerInfo && _input.DisplayPlayerInput != null)
            {
                var titleRow = new HtmlTag("div").AddClass("comment-card-title-row d-flex justify-content-between align-items-center");
                if (_input.IsNew) titleRow.AppendHtml(NewBadgeHtml());

                var displayPlayerInput = _input.DisplayPlayerInput;
                var playerDisplay = new DisplayPlayer(displayPlayerInput).Render();
                var playerTitle = new HtmlTag("span").AddClass("comment-card-player d-flex align-items-center gap-2").AppendHtml(playerDisplay);

                if (_input.ShowViewAll)
                {
                    var viewAll = new HtmlTag("a")
                        .AddClass("comment-card-viewall")
                        .Attr("href", RotoMonsterUIUrls.PlayerCommentsUrl(_input.DisplayPlayerInput.PlayerId))
                        .Attr("aria-label", "Filter player comments");
                    viewAll.AppendHtml(new Icon(new IconInput { Type = IconType.Filter, Size = 14, Color = "currentColor" }).Render());

                    var viewAllTooltip = new CustomTooltip(viewAll.ToString(), "Filter player comments").WithHoverTrigger().Render();
                    playerTitle.AppendHtml(viewAllTooltip);
                }

                titleRow.Append(playerTitle);

                if (_input.TimeSinceCreated.HasValue)
                {
                    var timeSince = new HtmlTag("span").AppendHtml(new TimeSince(_input.TimeSinceCreated.Value).Render());
                    titleRow.Append(timeSince);
                }

                card.Append(titleRow);
            }
           
            var usernameRow = new HtmlTag("div").AddClass("comment-card-username d-flex align-items-center gap-2");

            // Badge keeps its own background/text color regardless of shading - it already has enough contrast on its own.
            if (_input.IsNew && !(_input.ShowPlayerInfo && _input.DisplayPlayerInput != null))
                usernameRow.AppendHtml(NewBadgeHtml());

            usernameRow.AppendHtml(new DisplayUsername(_input.DisplayUsernameInput).Render());

            if (!_input.ShowPlayerInfo && _input.TimeSinceCreated.HasValue)
            {
                var timeSince = new HtmlTag("span").AddClass("comment-card-time").AppendHtml(new TimeSince(_input.TimeSinceCreated.Value).Render());
                usernameRow.Append(timeSince);
            }

            card.Append(usernameRow);

            // Comment text
            var commentText = new HtmlTag("div").AddClass("comment-card-text").Text(_input.CommentText);
            card.Append(commentText);

            // Actions row - vote buttons and delete icon keep their own styling (borders/explicit colors already read fine on yellow)
            var actionsRow = new HtmlTag("div").AddClass("comment-card-actions");

            if (_input.ShowUpDownControls)
            {
                var voteControl = new VoteControl(new VoteControlInput
                {
                    Id = _input.CommentId.ToString(),
                    NamePrefix = "comment",
                    CanVote = _input.CanVote,
                    UpVoteCount = _input.UpVoteCount,
                    DownVoteCount = _input.DownVoteCount,
                    VotedUp = _input.UserVoteInput != null && _input.UserVoteInput.HasVoted && _input.UserVoteInput.VotedUp,
                    VotedDown = _input.UserVoteInput != null && _input.UserVoteInput.HasVoted && _input.UserVoteInput.VotedDown,
                    ForceDarkText = false
                }).Render();
                actionsRow.Append(new HtmlTag("span").AppendHtml(voteControl));
            }

            if (_input.UserCanDelete)
            {
                var trashIcon = new Icon(new IconInput { Type = IconType.Trash, Size = 16, Color = "#ef4444" }).Render();
                var deleteBtn = new HtmlTag("button")
                    .AddClass("comment-card-btn comment-card-btn-delete")
                    .Attr("name", $"delete_{_input.CommentId}")
                    .Attr("style", "margin-left: auto;")
                    .AppendHtml(trashIcon);
                actionsRow.Append(deleteBtn);
            }

            if (_input.UserCanPostComment && !_input.IsCommentExpanded)
            {
                var expandBtn = new HtmlTag("button")
                    .AddClass("comment-card-btn comment-card-btn-expand")
                    .Attr("name", $"expand_{_input.CommentId}");
                expandBtn.AppendHtml("<i class='fas fa-reply'></i>");
                actionsRow.Append(expandBtn);
            }

            card.Append(actionsRow);

            // Comment input area (if expanded)
            if (_input.IsCommentExpanded)
            {
                var expandedArea = new HtmlTag("div").AddClass("comment-card-expanded");
                var textarea = new HtmlTag("textarea")
                    .AddClass("comment-card-textarea")
                    .Attr("name", $"comment_{_input.CommentId}")
                    .Text(_input.CurrentCommentText ?? "");
                var postBtn = new HtmlTag("button")
                    .AddClass("comment-card-btn comment-card-btn-post")
                    .Attr("name", $"post_{_input.CommentId}")
                    .Text("Post");
                expandedArea.Append(textarea);
                expandedArea.Append(postBtn);
                card.Append(expandedArea);
            }

            return card.ToString();
        }
    }
}