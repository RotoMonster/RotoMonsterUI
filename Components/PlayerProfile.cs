using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class PlayerProfile
    {
        private readonly PlayerProfileInput _input;

        public PlayerProfile(PlayerProfileInput input)
        {
            _input = input;
        }

        private string Key(string prefix)
        {
            return prefix + "_" + _input.Id;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("player-profile");

            if (_input == null) return wrap.ToString();

            wrap.Attr("id", "player-profile-" + _input.Id);

            if (!string.IsNullOrEmpty(_input.Message))
                wrap.Append(new HtmlTag("div").AddClass("pp-message").Text(_input.Message));

            var cols = new HtmlTag("div").AddClass("pp-cols");

            cols.Append(RenderLeft());

            var right = RenderComments();
            if (right != null) cols.Append(new HtmlTag("div").AddClass("pp-right").Append(right));

            wrap.Append(cols);

            if (!string.IsNullOrEmpty(_input.BelowHtml))
                wrap.Append(new HtmlTag("div").AddClass("pp-below").AppendHtml(_input.BelowHtml));

            return wrap.ToString();
        }

        private HtmlTag RenderLeft()
        {
            var left = new HtmlTag("div").AddClass("pp-left");

            var facts = RenderFacts();
            if (facts != null) left.Append(facts);

            var expert = RenderExpertComment();
            if (expert != null) left.Append(expert);

            var news = RenderNews();
            if (news != null) left.Append(news);

            if (_input.TrackedLeagues != null)
                left.Append(RenderSection(_input.TrackedLeagues, "tracked"));

            return left;
        }

        private HtmlTag RenderFacts()
        {
            var hasBadges = _input.StatBadges != null
                && _input.StatBadges.Stats != null
                && _input.StatBadges.Stats.Count > 0;

            var hasLinks = _input.Links != null && _input.Links.Count > 0;

            if (!hasBadges && !hasLinks) return null;

            var card = new HtmlTag("div").AddClass("pp-card pp-facts");

            if (hasBadges)
                card.Append(new HtmlTag("div")
                    .AddClass("pp-badges")
                    .AppendHtml(new PlayerStatBadge(_input.StatBadges).Render()));

            if (hasLinks)
            {
                var row = new HtmlTag("div").AddClass("pp-links");

                foreach (var link in _input.Links)
                {
                    if (link == null || string.IsNullOrEmpty(link.Text)) continue;

                    var tag = new HtmlTag("a")
                        .AddClass("pp-link")
                        .Attr("href", string.IsNullOrEmpty(link.Url) ? "#" : link.Url);

                    tag.Append(new HtmlTag("span").Text(link.Text));

                    if (!string.IsNullOrEmpty(link.CountText))
                        tag.Append(new HtmlTag("span").AddClass("pp-link-count").Text(link.CountText));

                    row.Append(tag);
                }

                card.Append(row);
            }

            return card;
        }

        private HtmlTag RenderExpertComment()
        {
            if (string.IsNullOrEmpty(_input.ExpertCommentHtml)) return null;

            var card = new HtmlTag("div").AddClass("pp-card");

            if (!string.IsNullOrEmpty(_input.ExpertCommentTitle))
                card.Append(Head(_input.ExpertCommentTitle, null, null));

            var body = new HtmlTag("div").AddClass("pp-body pp-comment");
            body.AppendHtml(_input.ExpertCommentHtml);

            if (!string.IsNullOrEmpty(_input.ExpertCommentByText))
                body.Append(new HtmlTag("span").AddClass("pp-by").Text(_input.ExpertCommentByText));

            card.Append(body);

            return card;
        }

        private HtmlTag RenderNews()
        {
            var hasCard = _input.NewsCard != null;
            var hasHtml = !string.IsNullOrEmpty(_input.NewsHtml);

            if (!hasCard && !hasHtml) return null;

            var card = new HtmlTag("div").AddClass("pp-card");

            card.Append(Head(_input.NewsTitle, null,
                string.IsNullOrEmpty(_input.NewsSeeAllUrl) ? null : _input.NewsSeeAllUrl));

            var body = new HtmlTag("div").AddClass("pp-news");

            if (hasCard) body.AppendHtml(new NewsCard(_input.NewsCard).Render());
            if (hasHtml) body.AppendHtml(_input.NewsHtml);

            card.Append(body);

            return card;
        }

        private HtmlTag RenderSection(PlayerProfileSection section, string fallbackId)
        {
            var card = new HtmlTag("div").AddClass("pp-card");

            var id = string.IsNullOrEmpty(section.Id) ? Key("pp" + fallbackId) : section.Id;
            var contentId = id + "-content";
            var toggleId = id + "-toggle";

            var head = new HtmlTag("div").AddClass("pp-head");

            var button = new HtmlTag("button")
                .AddClass("pp-head-btn")
                .Attr("type", "button")
                .Attr("data-toggle", "collapse")
                .Attr("data-target", "#" + contentId)
                .Attr("aria-controls", contentId)
                .Attr("aria-expanded", section.IsExpanded ? "true" : "false");

            button.Append(new HtmlTag("span").AddClass("pp-caret").AppendHtml("&#9662;"));
            button.Append(new HtmlTag("span").AddClass("pp-title").Text(section.Title ?? ""));

            if (!string.IsNullOrEmpty(section.CountText))
                button.Append(new HtmlTag("span").AddClass("pp-count").Text(section.CountText));

            head.Append(button);

            if (section.ShowPin)
                head.AppendHtml(Pin(id, section.IsPinned, section.PinPostsBack,
                    section.PinTitle, section.UnpinTitle));

            if (!string.IsNullOrEmpty(section.SeeAllUrl))
                head.Append(new HtmlTag("a")
                    .AddClass("pp-seeall")
                    .Attr("href", section.SeeAllUrl)
                    .Text(section.SeeAllText));

            card.Append(head);

            card.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("id", toggleId)
                .Attr("name", toggleId)
                .Attr("value", section.IsExpanded ? "1" : "0"));

            var body = new HtmlTag("div")
                .Attr("id", contentId)
                .AddClass(section.IsExpanded ? "pp-body collapse show" : "pp-body collapse");

            body.AppendHtml(section.ContentHtml ?? "");
            card.Append(body);

            return card;
        }

        private HtmlTag RenderComments()
        {
            var hasList = _input.Comments != null && _input.Comments.Count > 0;
            var hasHtml = !string.IsNullOrEmpty(_input.CommentsHtml);

            var card = new HtmlTag("div").AddClass("pp-card pp-comments");

            var id = Key("ppcomments");
            var contentId = id + "-content";
            var toggleId = id + "-toggle";

            var head = new HtmlTag("div").AddClass("pp-head");

            var button = new HtmlTag("button")
                .AddClass("pp-head-btn")
                .Attr("type", "button")
                .Attr("data-toggle", "collapse")
                .Attr("data-target", "#" + contentId)
                .Attr("aria-controls", contentId)
                .Attr("aria-expanded", _input.CommentsExpanded ? "true" : "false");

            button.Append(new HtmlTag("span").AddClass("pp-caret").AppendHtml("&#9662;"));
            button.Append(new HtmlTag("span").AddClass("pp-title").Text(_input.CommentsTitle));

            if (!string.IsNullOrEmpty(_input.CommentsCountText))
                button.Append(new HtmlTag("span").AddClass("pp-count").Text(_input.CommentsCountText));

            head.Append(button);

            if (_input.ShowCommentsPin)
                head.AppendHtml(Pin(id, _input.CommentsPinned, _input.CommentsPinPostsBack,
                    "Pin this open", "Unpin"));

            card.Append(head);

            card.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("id", toggleId)
                .Attr("name", toggleId)
                .Attr("value", _input.CommentsExpanded ? "1" : "0"));

            var body = new HtmlTag("div")
                .Attr("id", contentId)
                .AddClass(_input.CommentsExpanded ? "pp-body-plain collapse show" : "pp-body-plain collapse");

            if (_input.ShowAddComment)
                body.AppendHtml(new HtmlTag("div")
                    .AddClass("pp-addcomment")
                    .AppendHtml(new Button(_input.AddCommentText)
                        .WithStyle(ButtonStyle.Secondary)
                        .WithName(Key("ppaddcomment"))
                        .WithPostBack()
                        .Render())
                    .ToString());

            var list = new HtmlTag("div").AddClass("pp-comment-list");

            if (_input.CommentsMaxHeight > 0)
                list.Attr("style", "max-height:" + _input.CommentsMaxHeight + "px;");

            if (hasHtml)
            {
                list.AppendHtml(_input.CommentsHtml);
            }
            else if (hasList)
            {
                foreach (var comment in _input.Comments)
                {
                    if (comment == null) continue;
                    list.AppendHtml(new CommentCard(comment).Render());
                }
            }
            else
            {
                list.Append(new HtmlTag("div").AddClass("pp-nocomments").Text(_input.NoCommentsText));
            }

            body.Append(list);
            card.Append(body);

            return card;
        }

        private static HtmlTag Head(string title, string countText, string seeAllUrl)
        {
            var head = new HtmlTag("div").AddClass("pp-head");

            head.Append(new HtmlTag("span").AddClass("pp-title").Text(title ?? ""));

            if (!string.IsNullOrEmpty(countText))
                head.Append(new HtmlTag("span").AddClass("pp-count").Text(countText));

            if (!string.IsNullOrEmpty(seeAllUrl))
                head.Append(new HtmlTag("a")
                    .AddClass("pp-seeall")
                    .Attr("href", seeAllUrl)
                    .Text("See all"));

            return head;
        }

        private static string Pin(string id, bool pinned, bool postsBack,
            string pinTitle, string unpinTitle)
        {
            var button = new HtmlTag("button")
                .AddClass("pp-pin")
                .Attr("type", "button")
                .Attr("id", id + "-pin-btn")
                .Attr("data-collapse-lock", id)
                .Attr("aria-pressed", pinned ? "true" : "false")
                .Attr("aria-label", pinned ? unpinTitle : pinTitle);

            if (pinned) button.AddClass("is-locked");
            if (postsBack) button.Attr("data-collapse-lock-postback", "1");

            button.AppendHtml(new Icon(new IconInput { Type = IconType.Pin, Size = 15 }).Render());

            var wrap = new HtmlTag("span");

            wrap.Append(CustomTooltip.Wrap(button, pinned ? unpinTitle : pinTitle,
                TooltipPlacement.Left));

            wrap.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("id", id + "-lock")
                .Attr("name", id + "-lock")
                .Attr("value", pinned ? "1" : "0"));

            return wrap.ToString();
        }
    }
}