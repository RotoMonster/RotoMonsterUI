using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlTags;

namespace RotoMonsterUI
{
    public class TweetCard
    {
        private readonly TweetCardInput _input;

        public TweetCard(TweetCardInput input)
        {
            _input = input;
        }

        private const string AllTeams = "all";

        private string Key(string prefix)
        {
            return prefix + "_" + _input.TweetId;
        }

        private static string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return color;
            if (color.StartsWith("var(") || color.StartsWith("#")) return color;
            return "#" + color;
        }

        private static string FormatFollowers(int followers)
        {
            return (followers / 1000) + "k";
        }

        private static string FormatDuration(int millis)
        {
            var total = millis / 1000;
            var minutes = total / 60;
            var seconds = total % 60;
            return minutes + ":" + seconds.ToString("00");
        }

        private static string RenderTweetText(string text)
        {
            var decoded = System.Net.WebUtility.HtmlDecode(text);
            var escaped = System.Net.WebUtility.HtmlEncode(decoded);

            return Regex.Replace(escaped, @"&lt;br\s*/?&gt;", "<br />", RegexOptions.IgnoreCase);
        }

        private string SelectedTeam
        {
            get
            {
                return string.IsNullOrEmpty(_input.SelectedTeamCode)
                    ? AllTeams
                    : _input.SelectedTeamCode;
            }
        }

        private List<TweetCardPlayer> VisiblePlayers()
        {
            var players = _input.Players ?? new List<TweetCardPlayer>();
            var team = SelectedTeam;

            if (string.Equals(team, AllTeams, StringComparison.OrdinalIgnoreCase))
                return players.ToList();

            return players
                .Where(p => p.DisplayPlayerInput != null &&
                            (string.Equals(p.DisplayPlayerInput.TeamCode, team, StringComparison.OrdinalIgnoreCase) ||
                             (_input.SelectedPlayerId.HasValue && p.DisplayPlayerInput.PlayerId == _input.SelectedPlayerId.Value)))
                .ToList();
        }

        public string Render()
        {
            var card = new HtmlTag("div").AddClass("tweet-card");

            if (_input.SelectedPlayerId.HasValue)
                card.AddClass("tweet-card--posting");

            card.Append(RenderHeader());

            if (!string.IsNullOrEmpty(_input.Text))
                card.Append(new HtmlTag("div").AddClass("tweet-card-text")
                    .AppendHtml(RenderTweetText(_input.Text)));

            if (_input.Media != null && _input.Media.Any())
                card.Append(RenderMedia());

            if (_input.Teams != null && _input.Teams.Count > 1)
                card.Append(RenderTeamFilter());

            if (_input.Players != null && _input.Players.Any())
                card.Append(RenderPlayers());

            return card.ToString();
        }

        private HtmlTag RenderHeader()
        {
            var header = new HtmlTag("div").AddClass("tweet-card-header");

            if (!string.IsNullOrEmpty(_input.ScreenName))
            {
                var user = new HtmlTag("a")
                    .AddClass("tweet-card-user")
                    .Attr("href", "https://twitter.com/" + _input.ScreenName)
                    .Attr("target", "_blank")
                    .Attr("rel", "noopener noreferrer")
                    .Text(_input.ScreenName);
                header.Append(user);
            }

            if (_input.Followers.HasValue)
            {
                header.Append(new HtmlTag("span")
                    .AddClass("tweet-card-followers")
                    .Text(FormatFollowers(_input.Followers.Value)));
            }

            if (_input.TimeSinceCreated.HasValue)
                header.AppendHtml(new TimeSinceBadge(_input.TimeSinceCreated.Value).Render());

            if (!string.IsNullOrEmpty(_input.TweetUrl))
            {
                var source = new HtmlTag("a")
                    .AddClass("tweet-card-source")
                    .Attr("href", _input.TweetUrl)
                    .Attr("target", "_blank")
                    .Attr("rel", "noopener noreferrer")
                    .Attr("title", "Open the tweet on X");
                source.AppendHtml(new Icon(new IconInput
                {
                    Type = IconType.ExternalLink,
                    Size = 14
                }).Render());
                header.Append(source);
            }

            var right = new HtmlTag("div").AddClass("tweet-card-header-right");

            if (!string.IsNullOrEmpty(_input.ProfileImageUrl))
            {
                right.Append(new HtmlTag("img")
                    .AddClass("tweet-card-avatar")
                    .Attr("src", _input.ProfileImageUrl)
                    .Attr("alt", _input.DisplayName ?? _input.ScreenName ?? ""));
            }

            header.Append(right);

            return header;
        }

        private HtmlTag RenderMedia()
        {
            var wrap = new HtmlTag("div").AddClass("tweet-card-media");

            foreach (var media in _input.Media)
            {
                var isVideo = media.MediaType != TweetMediaType.Photo && !string.IsNullOrEmpty(media.VideoUrl);
                var href = isVideo ? media.VideoUrl : media.ImageUrl;

                if (string.IsNullOrEmpty(href) && string.IsNullOrEmpty(media.ImageUrl))
                    continue;

                var item = new HtmlTag("a")
                    .AddClass("tweet-card-media-item")
                    .Attr("href", href)
                    .Attr("target", "_blank")
                    .Attr("rel", "noopener noreferrer");

                if (!string.IsNullOrEmpty(media.ImageUrl))
                {
                    item.Append(new HtmlTag("img")
                        .Attr("src", media.ImageUrl)
                        .Attr("alt", isVideo ? "Video" : "Image")
                        .Attr("loading", "lazy"));
                }

                if (isVideo)
                {
                    item.Append(new HtmlTag("span").AddClass("tweet-card-media-badge").Text("\u25B6"));

                    if (media.DurationMillis.HasValue && media.DurationMillis.Value > 0)
                    {
                        item.Append(new HtmlTag("span")
                            .AddClass("tweet-card-media-duration")
                            .Text(FormatDuration(media.DurationMillis.Value)));
                    }
                }

                wrap.Append(item);
            }

            return wrap;
        }

        private HtmlTag RenderTeamFilter()
        {
            var wrap = new HtmlTag("div").AddClass("tweet-card-teams");

            var group = new HtmlTag("div").AddClass("bm-segmented");
            var groupName = Key("tweetteam");
            var selected = SelectedTeam;

            var options = new List<Tuple<string, string, string>>();
            foreach (var team in _input.Teams)
            {
                if (team == null || string.IsNullOrEmpty(team.TeamCode)) continue;
                options.Add(Tuple.Create(team.TeamCode, team.TeamCode, NormalizeColor(team.ColorCode)));
            }
            options.Add(Tuple.Create("All", AllTeams, (string)null));

            foreach (var option in options)
            {
                var inputId = groupName + "-" + option.Item2;

                var radio = new HtmlTag("input")
                    .Attr("type", "radio")
                    .Attr("name", groupName)
                    .Attr("id", inputId)
                    .Attr("value", option.Item2)
                    .Attr("onchange", "__doPostBack('" + groupName + "','',this.form)")
                    .Attr("language", "javascript");

                if (string.Equals(option.Item2, selected, StringComparison.OrdinalIgnoreCase))
                    radio.Attr("checked", "checked");

                var label = new HtmlTag("label").Attr("for", inputId).Text(option.Item1);
                if (!string.IsNullOrEmpty(option.Item3))
                    label.Attr("style", "--segment-color:" + option.Item3 + ";");

                group.Append(radio);
                group.Append(label);
            }

            wrap.Append(group);

            return wrap;
        }

        private HtmlTag RenderPlayers()
        {
            var list = new HtmlTag("div").AddClass("tweet-card-players");
            var groupName = Key("tweetplayer");
            var hiddenId = groupName + "-value";

            list.Append(new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("name", groupName)
                .Attr("id", hiddenId)
                .Attr("value", _input.SelectedPlayerId.HasValue
                    ? _input.SelectedPlayerId.Value.ToString()
                    : ""));

            foreach (var player in VisiblePlayers())
            {
                if (player == null || player.DisplayPlayerInput == null) continue;

                var display = player.DisplayPlayerInput;
                var isSelected = _input.SelectedPlayerId.HasValue &&
                                 _input.SelectedPlayerId.Value == display.PlayerId;

                if (string.IsNullOrEmpty(display.TeamColor))
                {
                    display.TeamColor = _input.Sport == NewsCardSport.NBA
                        ? TeamColorHelper.GetNbaTeamColorVar(display.TeamCode)
                        : TeamColorHelper.GetTeamColorVar(display.TeamCode);
                }

                var row = new HtmlTag("div").AddClass("tweet-card-player-row");
                var inputId = groupName + "-" + display.PlayerId;

                var check = new HtmlTag("input")
                    .Attr("type", "checkbox")
                    .AddClass("tweet-card-player-check")
                    .Attr("id", inputId)
                    .Attr("data-target", hiddenId)
                    .Attr("data-playerid", display.PlayerId.ToString())
                    .Attr("data-postback", groupName);

                if (isSelected)
                    check.Attr("checked", "checked");

                row.Append(check);

                var label = new HtmlTag("label").Attr("for", inputId);

                if (!string.IsNullOrEmpty(display.TeamCode))
                {
                    label.Append(new HtmlTag("span")
                        .AddClass("tweet-card-player-team")
                        .Text(display.TeamCode));
                }

                label.AppendHtml(new DisplayPlayer(display).Render());

                if (player.MatchConfidence.HasValue)
                {
                    label.Append(new HtmlTag("span")
                        .AddClass("tweet-card-player-confidence")
                        .Text(player.MatchConfidence.Value.ToString("0.00")));
                }

                if (!string.IsNullOrEmpty(player.PercentText))
                {
                    label.Append(new HtmlTag("span")
                        .AddClass("tweet-card-player-pct")
                        .Text(player.PercentText));
                }

                row.Append(label);
                list.Append(row);

                if (isSelected)
                    list.Append(RenderPostForm());
            }

            return list;
        }

        private HtmlTag RenderPostForm()
        {
            var wrap = new HtmlTag("div").AddClass("tweet-card-post-form");

            wrap.Append(new NewsEditForm(new NewsEditFormInput
            {
                KeyPrefix = "tweet",
                KeyId = _input.TweetId.ToString(),
                Buttons = new List<NewsEditFormButton>
                {
                    new NewsEditFormButton { Text = "Post", Style = ButtonStyle.Primary, Name = Key("tweetpost") },
                    new NewsEditFormButton { Text = "Cancel", Style = ButtonStyle.Secondary, Name = Key("tweetcancel") }
                },
                StatusTypeText = _input.StatusTypeText,
                StatusTypeTag = _input.StatusTypeTag,
                NewsTitle = _input.NewsTitle,
                SourceURL = string.IsNullOrEmpty(_input.SourceURL) ? _input.TweetUrl : _input.SourceURL,
                NewsDetails = _input.NewsDetails,
                IsUnofficial = _input.IsUnofficial,
                NewsLevel = _input.NewsLevel,
                StatusTypeOptions = _input.StatusTypeOptions,
                StatusTypeTagOptions = _input.StatusTypeTagOptions,
                AvailableNewsTags = _input.AvailableNewsTags,
                NewsTags = _input.NewsTags
            }).RenderTag());

            return wrap;
        }
    }
}