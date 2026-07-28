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

        public string Render()
        {
            if (_input.ShowPlayerInfo && _input.DisplayPlayerInput != null
                && string.IsNullOrEmpty(_input.DisplayPlayerInput.TeamColor))
            {

                _input.DisplayPlayerInput.TeamColor = _input.Sport == NewsCardSport.NBA
                    ? TeamColorHelper.GetNbaTeamColorVar(_input.DisplayPlayerInput.TeamCode)
                    : TeamColorHelper.GetTeamColorVar(_input.DisplayPlayerInput.TeamCode);
            }

            var card = new HtmlTag("div").AddClass("comment-card");

            var ageShadeColor = ColorHelper.GetAgeShadeHex(_input.TimeSinceCreated);
            bool isShaded = ageShadeColor != null;
            if (isShaded)
                card.Attr("style", $"background:{ageShadeColor};");

            // Player title row
            if (_input.ShowPlayerInfo)
            {
                var titleRow = new HtmlTag("div").AddClass("comment-card-title-row d-flex justify-content-between align-items-center");

                var displayPlayerInput = _input.DisplayPlayerInput;
                if (isShaded && !string.IsNullOrEmpty(displayPlayerInput.TeamColor))
                {
                    displayPlayerInput = new DisplayPlayerInput
                    {
                        PlayerName = _input.DisplayPlayerInput.PlayerName,
                        PlayerId = _input.DisplayPlayerInput.PlayerId,
                        TeamCode = _input.DisplayPlayerInput.TeamCode,
                        TeamColor = "#000000",
                        Positions = _input.DisplayPlayerInput.Positions
                    };
                }
                var playerDisplay = new DisplayPlayer(displayPlayerInput).Render();
                var playerTitle = new HtmlTag("span").AddClass("comment-card-player d-flex align-items-center gap-2").AppendHtml(playerDisplay);
                if (isShaded) playerTitle.AddClass("comment-card-player--shaded");

                if (_input.ShowViewAll)
                {
                    var viewAll = new HtmlTag("a")
                        .AddClass("comment-card-viewall")
                        .Attr("href", RotoMonsterUIUrls.PlayerCommentsUrl(_input.DisplayPlayerInput.PlayerId))
                        .Attr("aria-label", "Filter player comments");
                    if (isShaded) viewAll.AddClass("color-shaded");
                    viewAll.AppendHtml(new Icon(new IconInput { Type = IconType.Filter, Size = 14, Color = "currentColor" }).Render());

                    var viewAllTooltip = new CustomTooltip(viewAll.ToString(), "Filter player comments").WithHoverTrigger().Render();
                    playerTitle.AppendHtml(viewAllTooltip);
                }

                titleRow.Append(playerTitle);

                if (_input.TimeSinceCreated.HasValue)
                {
                    var timeSince = new HtmlTag("span").AppendHtml(new TimeSince(_input.TimeSinceCreated.Value).Render());
                    if (isShaded) timeSince.AddClass("color-shaded");
                    titleRow.Append(timeSince);
                }

                card.Append(titleRow);
            }
           
            var usernameRow = new HtmlTag("div").AddClass("comment-card-username d-flex align-items-center gap-2");
            usernameRow.AppendHtml(new DisplayUsername(_input.DisplayUsernameInput).Render());

            if (_input.IsNew)
            {
                // Badge keeps its own background/text color regardless of shading - it already has enough contrast on its own.
                var newBadge = new Badge(new BadgeInput
                {
                    BadgeText = "New",
                    ColorClass = "badge-new"
                }).Render();
                usernameRow.AppendHtml(newBadge);
            }

            if (!_input.ShowPlayerInfo && _input.TimeSinceCreated.HasValue)
            {
                var timeSince = new HtmlTag("span").AddClass("comment-card-time").AppendHtml(new TimeSince(_input.TimeSinceCreated.Value).Render());
                if (isShaded) timeSince.AddClass("color-shaded");
                usernameRow.Append(timeSince);
            }

            card.Append(usernameRow);

            // Comment text
            var commentText = new HtmlTag("div").AddClass("comment-card-text").Text(_input.CommentText);
            if (isShaded) commentText.AddClass("color-shaded");
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
                    ForceDarkText = isShaded
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