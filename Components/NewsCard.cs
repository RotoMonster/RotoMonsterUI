using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace RotoMonsterUI
{
    public class NewsCard
    {
        private readonly NewsCardInput _input;

        public NewsCard(NewsCardInput input)
        {
            _input = input;
        }

        private static string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return color;
            return color.StartsWith("#") ? color : "#" + color;
        }

        private static string TintBackground(string color, double alpha = 0.12)
        {
            var hex = NormalizeColor(color).TrimStart('#');
            if (hex.Length != 6) return $"rgba(148,163,184,{alpha})";
            var r = System.Convert.ToInt32(hex.Substring(0, 2), 16);
            var g = System.Convert.ToInt32(hex.Substring(2, 2), 16);
            var b = System.Convert.ToInt32(hex.Substring(4, 2), 16);
            return $"rgba({r},{g},{b},{alpha})";
        }

        private static IconType? StatusTagIcon(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return null;
            return IconTypeResolver.Resolve(tagName, IconType.Other);
        }

        private static (IconType icon, string color) NewsTagDefaults(string tagName)
        {
            var n = (tagName ?? "").ToLower();

            if (n.Contains("shootaround"))
                return (n.Contains("no") || n.Contains("missed")) ? (IconType.MissedShootaround, "#ef4444") : (IconType.MadeShootaround, "#22c55e");
            if (n.Contains("practice"))
                return (n.Contains("no") || n.Contains("missed")) ? (IconType.MissedPractice, "#ef4444") : (IconType.Practiced, "#22c55e");
            if (n.Contains("game") && n.Contains("decision"))
                return (IconType.GameTimeDecision, "#3b82f6");
            if (n.Contains("spot start"))
                return (IconType.SpotStart, "#22c55e");
            if (n.Contains("limited minutes"))
                return (IconType.LimitedMinutes, "#3b82f6");
            if (n.Contains("team update"))
                return (IconType.IsTeamUpdate, "#3b82f6");

            return (IconType.Note, "#94a3b8");
        }

        private bool ShowLevelBadge => _input.ShowMediumLevel || _input.NewsLevel != NewsLevel.Medium;

        private static string LevelColor(NewsLevel level)
        {
            switch (level)
            {
                case NewsLevel.Low: return "#8c8c8c";
                case NewsLevel.Medium: return "#6699ff";
                case NewsLevel.High: return "#e68a00";
                case NewsLevel.Monster: return "#cc3300";
                default: return null;
            }
        }

            public string Render()
            {
                var card = new HtmlTag("div").AddClass("news-card");

            var ageShadeColor = ColorHelper.GetAgeShadeHex(_input.TimeSinceCreated);
            bool isShaded = ageShadeColor != null;
            // Only override while it's shaded - once it expires the card's normal border comes back.
            if (isShaded)
                card.Attr("style", AgeRing(ageShadeColor));

            // ---- Header row ----
            var headerRow = new HtmlTag("div").AddClass("news-card-header");

            headerRow.Append(RenderStatusBadge("main", FirstLetter(_input.StatusTypeText), _input.StatusTypeColorCode, _input.StatusTypeTag, _input.StatusTypeText, _input.NewsTitle, _input.IsUnofficial));

            if (_input.DisplayPlayerInput != null)
            {
                if (string.IsNullOrEmpty(_input.DisplayPlayerInput.TeamColor))
                {

                    _input.DisplayPlayerInput.TeamColor = _input.Sport == NewsCardSport.NBA
                        ? TeamColorHelper.GetNbaTeamColorVar(_input.DisplayPlayerInput.TeamCode)
                        : TeamColorHelper.GetTeamColorVar(_input.DisplayPlayerInput.TeamCode);
                }

                headerRow.AppendHtml(RenderPlayerWithCurrentStatus());
            }
            else if (_input.DisplayTeamInput != null)
            {
                var teamColor = _input.DisplayTeamInput.ColorCode;
                if (string.IsNullOrEmpty(teamColor) && !string.IsNullOrEmpty(_input.DisplayTeamInput.TeamCode))
                    teamColor = _input.Sport == NewsCardSport.NBA
                        ? TeamColorHelper.GetNbaTeamColorVar(_input.DisplayTeamInput.TeamCode)
                        : TeamColorHelper.GetTeamColorVar(_input.DisplayTeamInput.TeamCode);

                var teamSpan = new HtmlTag("span")
                    .AddClass("news-card-team")
                    .Attr("style", $"color:{NormalizeColor(teamColor)}")
                    .Text(_input.DisplayTeamInput.Name);
                headerRow.Append(teamSpan);
            }

            var headerRight = new HtmlTag("div").AddClass("news-card-header-right");

            if (!string.IsNullOrEmpty(_input.SourceURL))
            {
                var sourceLink = new HtmlTag("a")
                    .AddClass("news-card-source")
                    .Attr("href", _input.SourceURL)
                    .Attr("target", "_blank")
                    .Attr("rel", "noopener noreferrer")
                    .Attr("aria-label", "Source");
                sourceLink.AppendHtml(new Icon(new IconInput { Type = IconType.ExternalLink, Size = 14 }).Render());
                headerRight.Append(sourceLink);
            }

            if (_input.TimeSinceCreated.HasValue)
                headerRight.AppendHtml(new TimeSince(_input.TimeSinceCreated.Value).Render());

            headerRow.Append(headerRight);
            card.Append(headerRow);

            // ---- Status text / team roster row ----
            if (_input.DisplayTeamInput != null && _input.TeamPlayerNames.Any())
            {
                var rosterRow = new HtmlTag("div").AddClass("news-card-roster");
                var rosterNames = new HtmlTag("span").Text(string.Join(", ", _input.TeamPlayerNames));
                rosterRow.Append(rosterNames);

                var teamLevelColor = ShowLevelBadge ? LevelColor(_input.NewsLevel) : null;
                if (teamLevelColor != null)
                {
                    // Level badge keeps its own background/text color - already reads fine on its own.
                    var teamLevelBadge = new HtmlTag("span")
                        .AddClass("news-card-level-badge")
                        .Attr("style", $"background:{teamLevelColor}")
                        .Text($"{_input.NewsLevel.ToString().ToLower()}");
                    rosterRow.Append(teamLevelBadge);
                }

                card.Append(rosterRow);
            }
            else if (!string.IsNullOrEmpty(_input.StatusTypeText))
            {
                var statusRow = new HtmlTag("div").AddClass("news-card-status-row");
                var statusText = new HtmlTag("span").AddClass("news-card-status-text").Text(_input.StatusTypeText);
                statusRow.Append(statusText);

                if (!string.IsNullOrEmpty(_input.NewsTitle))
                {
                    var newsTitle = new HtmlTag("span").AddClass("news-card-news-title").Text(_input.NewsTitle);
                    statusRow.Append(newsTitle);
                }

                var levelColor = ShowLevelBadge ? LevelColor(_input.NewsLevel) : null;
                if (levelColor != null)
                {
                    // Level badge keeps its own background/text color - already reads fine on its own.
                    var levelBadge = new HtmlTag("span")
                        .AddClass("news-card-level-badge")
                        .Attr("style", $"background:{levelColor}")
                        .Text($"{_input.NewsLevel.ToString().ToLower()}");
                    statusRow.Append(levelBadge);
                }

                card.Append(statusRow);
            }

            // ---- More details ----
            if (!_input.IsEditing && !string.IsNullOrEmpty(_input.NewsDetails))
            {
                var detailsDiv = new HtmlTag("div").AddClass("news-card-details").Text(_input.NewsDetails);
                card.Append(detailsDiv);
            }

            var appliedTags = RenderTagList();
            if (appliedTags != null)
                card.Append(appliedTags);

            // ---- Counts + edit/delete actions on one row ----
            bool hasCounts = _input.UserOwnCount.HasValue || _input.UserFreeAgentCount.HasValue;
            bool hasActions = _input.UserCanEdit || _input.UserCanDelete;

            if (hasCounts || hasActions)
            {
                var bottomRow = new HtmlTag("div").AddClass("news-card-bottom-row");

                if (hasActions)
                {
                    var actionRow = new HtmlTag("div").AddClass("news-card-actions");

                    if (_input.UserCanEdit)
                    {
                        var editBtn = new HtmlTag("button")
                            .AddClass("news-card-action-btn")
                            .Attr("type", "button")
                            .Attr("name", $"editnews_{_input.NewsId}")
                            .Attr("data-newsid", _input.NewsId.ToString())
                            .Attr("onclick", "EditNews(this)")
                            .Attr("aria-label", "Edit")
                            .AppendHtml(new Icon(new IconInput { Type = IconType.Edit, Size = 15 }).Render());
                        actionRow.Append(editBtn);
                    }

                    if (_input.UserCanDelete)
                    {
                        // Delete icon has its own hardcoded red color already - no shading needed.
                        // Fires via manual __doPostBack (DeleteNews JS function) so it runs under AJAX,
                        // same fix as EditNews and ChatRoom's DeleteChat.
                        var deleteIconBtn = new HtmlTag("button")
                            .AddClass("news-card-action-btn news-card-action-btn--delete")
                            .Attr("type", "button")
                            .Attr("data-newsid", _input.NewsId.ToString())
                            .Attr("onclick", "DeleteNews(this)")
                            .Attr("aria-label", "Delete")
                            .AppendHtml(new Icon(new IconInput { Type = IconType.Trash, Size = 15, Color = "#ef4444" }).Render());
                        actionRow.Append(deleteIconBtn);
                    }

                    bottomRow.Append(actionRow);
                }

                if (hasCounts)
                {
                    var countsRow = new HtmlTag("div").AddClass("news-card-counts");
                    if (_input.UserOwnCount.HasValue)
                        countsRow.AppendHtml($"yours {_input.UserOwnCount.Value}");
                    if (_input.UserFreeAgentCount.HasValue)
                        countsRow.AppendHtml($" fa {_input.UserFreeAgentCount.Value}");
                    bottomRow.Append(countsRow);
                }

                card.Append(bottomRow);
            }

            // ---- Edit form ----
            if (_input.IsEditing)
                card.Append(RenderEditForm());

            return card.ToString();
        }

        private static string FirstLetter(string text)
        {
            return string.IsNullOrEmpty(text) ? "" : text.Substring(0, 1).ToUpper();
        }

        private HtmlTag RenderStatusBadge(string context, string label, string colorCode, string tag, string text, string newsTitle, bool isUnofficial)
        {
            var tooltipId = $"newsbadge-tooltip-{_input.NewsId}-{context}";

            var tooltipParts = new List<string>();
            if (!string.IsNullOrEmpty(text)) tooltipParts.Add(text);
            if (!string.IsNullOrEmpty(tag)) tooltipParts.Add(tag);
            if (!string.IsNullOrEmpty(newsTitle)) tooltipParts.Add(newsTitle);
            if (isUnofficial) tooltipParts.Add("Status is Unofficial");
            var tooltipText = string.Join(" - ", tooltipParts);

            var wrap = new HtmlTag("span").AddClass("bm-tooltip-wrap");

            var badge = new HtmlTag("span")
                .AddClass("news-card-status-badge bm-tooltip-trigger")
                .Attr("data-bm-tooltip", tooltipId)
                .Attr("style", $"background:{NormalizeColor(colorCode)}");

            badge.Append(new HtmlTag("span").Text(label));

            var tagIcon = StatusTagIcon(tag);
            if (tagIcon.HasValue)
                badge.AppendHtml(new Icon(new IconInput { Type = tagIcon.Value, Size = 14, Color = "white" }).Render());

            if (isUnofficial)
                badge.AppendHtml(new Icon(new IconInput { Type = IconType.UnofficialTag, Size = 16, Color = "white" }).Render());

            wrap.Append(badge);
            wrap.Append(new HtmlTag("div").AddClass("bm-tooltip-content").Attr("id", tooltipId).Text(tooltipText));

            return wrap;
        }

        private bool HasCurrentStatus()
        {
            return !string.IsNullOrEmpty(_input.CurrentStatusTypeText) || !string.IsNullOrEmpty(_input.CurrentStatusTypeAbbreviation);
        }

        private HtmlTag RenderCurrentStatusBadge(string context)
        {
            var label = string.IsNullOrEmpty(_input.CurrentStatusTypeAbbreviation)
                ? FirstLetter(_input.CurrentStatusTypeText)
                : _input.CurrentStatusTypeAbbreviation;
            return RenderStatusBadge(context, label, _input.CurrentStatusTypeColorCode, _input.CurrentStatusTypeTag, _input.CurrentStatusTypeText, _input.CurrentStatusNewsTitle, false);
        }

        private string RenderPlayerWithCurrentStatus()
        {
            var playerHtml = new DisplayPlayer(_input.DisplayPlayerInput).Render();
            if (!HasCurrentStatus()) return playerHtml;

            var row = new HtmlTag("div").AddClass("news-card-player-status");
            row.AppendHtml(playerHtml);
            row.Append(RenderCurrentStatusBadge("player").AddClass("news-card-current-status"));
            return row.ToString();
        }

        private HtmlTag RenderEditForm()
        {
            var currentStatusRow = new HtmlTag("div").AddClass("news-card-field-row");
            currentStatusRow.Append(new HtmlTag("span").AddClass("news-card-field-label").Text("Current Status"));
            if (string.IsNullOrEmpty(_input.CurrentStatusTypeText) && string.IsNullOrEmpty(_input.CurrentStatusTypeAbbreviation))
            {
                currentStatusRow.Append(new HtmlTag("span").Attr("style", "color:#94a3b8;font-size:.85rem;").Text("None"));
            }
            else
            {
                currentStatusRow.Append(RenderCurrentStatusBadge("edit"));
            }

            return new NewsEditForm(new NewsEditFormInput
            {
                KeyId = _input.NewsId.ToString(),
                LeadingHtml = currentStatusRow.ToString(),
                Buttons = new List<NewsEditFormButton>
                {
                    new NewsEditFormButton { Text = "Save",   Style = ButtonStyle.Primary,   Name = $"savenews_{_input.NewsId}" },
                    new NewsEditFormButton { Text = "Cancel", Style = ButtonStyle.Secondary, Name = $"cancelnews_{_input.NewsId}" }
                },
                StatusTypeText = _input.StatusTypeText,
                StatusTypeTag = _input.StatusTypeTag,
                NewsTitle = _input.NewsTitle,
                SourceURL = _input.SourceURL,
                NewsDetails = _input.NewsDetails,
                IsUnofficial = _input.IsUnofficial,
                NewsLevel = _input.NewsLevel,
                StatusTypeOptions = _input.StatusTypeOptions,
                StatusTypeTagOptions = _input.StatusTypeTagOptions,
                AvailableNewsTags = _input.AvailableNewsTags,
                NewsTags = _input.NewsTags
            }).RenderTag();
        }

        public string RenderTest()
        {
            var card = new HtmlTag("div").AddClass("news-card news-card--test");

            var cardTint = TintBackground(_input.StatusTypeColorCode, 0.08);
            var cardStyle = $"background:{cardTint};";

            // Inset ring rather than a border here. The corner status badge is absolutely
            // positioned against the padding box, so a real border would push it inward and
            // leave a gap in the corner. An inset shadow paints inside and moves nothing.
            var testAgeShadeColor = ColorHelper.GetAgeShadeHex(_input.TimeSinceCreated);
            if (testAgeShadeColor != null)
                cardStyle += AgeRing(testAgeShadeColor);

            card.Attr("style", cardStyle);

            // Status ribbon is absolutely positioned flush in the top-right corner.
            var cornerGroup = new HtmlTag("div").AddClass("news-card-test-corner-group");
            cornerGroup.Append(RenderCornerStatusBadge());
            card.Append(cornerGroup);

            var headerRow = new HtmlTag("div").AddClass("news-card-test-header");

            var headerLeft = new HtmlTag("div").AddClass("news-card-test-header-left");

            if (!string.IsNullOrEmpty(_input.SourceURL))
            {
                var sourceLink = new HtmlTag("a")
                    .AddClass("news-card-test-source")
                    .Attr("href", _input.SourceURL)
                    .Attr("target", "_blank")
                    .Attr("rel", "noopener noreferrer")
                    .Attr("aria-label", "Source");
                sourceLink.AppendHtml(new Icon(new IconInput { Type = IconType.ExternalLink, Size = 18, Color = "currentColor" }).Render());
                headerLeft.Append(sourceLink);
            }

            if (_input.TimeSinceCreated.HasValue)
            {
                var timeBadge = new HtmlTag("span").AppendHtml(new TimeSinceBadge(_input.TimeSinceCreated.Value).Render());
                headerLeft.Append(timeBadge);
            }

            var levelColor = ShowLevelBadge ? LevelColor(_input.NewsLevel) : null;
            if (levelColor != null)
            {
                var levelBadge = new HtmlTag("span")
                    .AddClass("news-card-test-level")
                    .Attr("style", $"background:{levelColor}")
                    .Text(_input.NewsLevel.ToString().ToLower());
                headerLeft.Append(levelBadge);
            }

            headerRow.Append(headerLeft);
            card.Append(headerRow);

            if (_input.DisplayPlayerInput != null)
            {
                if (string.IsNullOrEmpty(_input.DisplayPlayerInput.TeamColor))
                {
                    _input.DisplayPlayerInput.TeamColor = _input.Sport == NewsCardSport.NBA
                        ? TeamColorHelper.GetNbaTeamColorVar(_input.DisplayPlayerInput.TeamCode)
                        : TeamColorHelper.GetTeamColorVar(_input.DisplayPlayerInput.TeamCode);
                }
                var main = new HtmlTag("div").AddClass("news-card-test-main");
                var left = new HtmlTag("div").AddClass("news-card-test-main-left");
                left.AppendHtml(RenderPlayerWithCurrentStatus());
                var gameText = GameText();
                if (gameText != null && !_input.ShowGameInCorner)
                    left.Append(new HtmlTag("div").AddClass("news-card-test-game").Text(gameText));
                main.Append(left);
                var pills = RenderUserPills();
                if (pills != null) main.Append(pills);
                card.Append(main);
            }

            var bodyParts = new List<string>();
            if (!string.IsNullOrEmpty(_input.NewsTitle)) bodyParts.Add(_input.NewsTitle);
            if (!string.IsNullOrEmpty(_input.NewsDetails)) bodyParts.Add(_input.NewsDetails);
            var bodyContent = string.Join(" - ", bodyParts);
            if (!string.IsNullOrEmpty(bodyContent))
            {
                var bodyRow = new HtmlTag("div").AddClass("news-card-test-body-row");

                var bodyText = new HtmlTag("span").AddClass("news-card-test-body").Text(bodyContent);
                bodyRow.Append(bodyText);

                card.Append(bodyRow);
            }

            if (!_input.IsEditing)
            {
                var testTags = RenderTagList();
                if (testTags != null)
                    card.Append(testTags);
            }

            bool hasCounts = _input.UserOwnCount.HasValue || (_input.UserFreeAgentCount.HasValue && _input.UserFreeAgentCount.Value != 0);
            bool hasActions = _input.UserCanEdit || _input.UserCanDelete;

            if (hasCounts || hasActions)
            {
                var bottomRow = new HtmlTag("div").AddClass("news-card-bottom-row");

                if (hasActions)
                {
                    var actionRow = new HtmlTag("div").AddClass("news-card-actions");

                    if (_input.UserCanEdit)
                    {
                        var editBtn = new HtmlTag("button")
                            .AddClass("news-card-action-btn")
                            .Attr("type", "button")
                            .Attr("data-newsid", _input.NewsId.ToString())
                            .Attr("onclick", "EditNews(this)")
                            .Attr("aria-label", "Edit")
                            .AppendHtml(new Icon(new IconInput { Type = IconType.Edit, Size = 15 }).Render());
                        actionRow.Append(editBtn);
                    }

                    if (_input.UserCanDelete)
                    {
                        var deleteIconBtn = new HtmlTag("button")
                            .AddClass("news-card-action-btn news-card-action-btn--delete")
                            .Attr("type", "button")
                            .Attr("data-newsid", _input.NewsId.ToString())
                            .Attr("onclick", "DeleteNews(this)")
                            .Attr("aria-label", "Delete")
                            .AppendHtml(new Icon(new IconInput { Type = IconType.Trash, Size = 15, Color = "#ef4444" }).Render());
                        actionRow.Append(deleteIconBtn);
                    }

                    bottomRow.Append(actionRow);
                }

                if (hasCounts)
                {
                    var countsRow = new HtmlTag("div").AddClass("news-card-counts");
                    var countParts = new List<string>();
                    if (_input.UserOwnCount.HasValue)
                        countParts.Add($"own {_input.UserOwnCount.Value}");
                    if (_input.UserFreeAgentCount.HasValue && _input.UserFreeAgentCount.Value != 0)
                        countParts.Add($"fa {_input.UserFreeAgentCount.Value}");
                    countsRow.AppendHtml(string.Join(" &middot; ", countParts));
                    bottomRow.Append(countsRow);
                }

                card.Append(bottomRow);
            }

            if (_input.IsEditing)
                card.Append(RenderEditForm());

            return card.ToString();
        }

        private HtmlTag RenderTagList()
        {
            if (_input.NewsTags == null || _input.NewsTags.Count == 0)
                return null;

            var tagList = new HtmlTag("div").AddClass("news-card-tag-list");
            foreach (var applied in _input.NewsTags)
            {
                var tag = ResolveTag(applied);
                if (string.IsNullOrWhiteSpace(tag.Name)) continue;
                var defaults = NewsTagDefaults(tag.Name);
                var icon = tag.Icon ?? defaults.icon;
                var color = tag.Color ?? defaults.color;

                var tagRow = new HtmlTag("div")
                    .AddClass("news-card-tag")
                    .Attr("style", $"color:{NormalizeColor(color)}; background:{TintBackground(color)};");
                tagRow.AppendHtml(new Icon(new IconInput { Type = icon, Size = 14, Color = "currentColor" }).Render());
                tagRow.Append(new HtmlTag("span").Text(tag.Name));
                tagList.Append(tagRow);
            }
            return tagList.Children.Count > 0 ? tagList : null;
        }

        private NewsTagOption ResolveTag(NewsTagOption tag)
        {
            var known = _input.AvailableNewsTags?.FirstOrDefault(t => t.Id == tag.Id);
            if (known == null) return tag;
            return new NewsTagOption
            {
                Id = tag.Id,
                Name = string.IsNullOrWhiteSpace(tag.Name) ? known.Name : tag.Name,
                Icon = tag.Icon ?? known.Icon,
                Color = string.IsNullOrEmpty(tag.Color) ? known.Color : tag.Color
            };
        }

        private HtmlTag RenderUserPills()
        {
            if (!_input.IsMyPlayer && !_input.OwnPercent.HasValue) return null;

            var pills = new HtmlTag("div").AddClass("news-card-test-pills");
            if (_input.IsMyPlayer)
                pills.Append(new HtmlTag("span").AddClass("news-card-test-mine").Attr("title", "On your team").Text("My player"));
            if (_input.OwnPercent.HasValue)
                pills.Append(new HtmlTag("span").AddClass("news-card-test-own-pill").Attr("title", "Owned in tracked leagues").Text($"{_input.OwnPercent.Value:0}% own"));
            return pills;
        }

        private static string AgeRing(string color)
        {
            return ColorHelper.AgeRingStyle(color);
        }

        private string CornerText()
        {
            var text = _input.StatusTypeText ?? "";
            var game = _input.ShowGameInCorner ? GameText() : null;
            return game == null ? text : (text + " " + game).Trim();
        }

        private string GameText()
        {
            var g = _input.Game;
            if (g == null) return null;

            var home = g.HomeGameTeam?.TeamCode ?? "";
            var away = g.AwayGameTeam?.TeamCode ?? "";
            var zone = g.DisplayTimezone ?? System.TimeZoneInfo.Local;
            var when = g.GameTimeUtc == default ? (System.DateTime?)null
                : System.TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.SpecifyKind(g.GameTimeUtc, System.DateTimeKind.Utc), zone);
            var day = when.HasValue ? when.Value.ToString("ddd", System.Globalization.CultureInfo.InvariantCulture) + " " : "";

            var team = _input.DisplayPlayerInput?.TeamCode ?? "";
            if (team.Length > 0 && string.Equals(team, home, System.StringComparison.OrdinalIgnoreCase))
                return (day + "vs " + away).Trim();
            if (team.Length > 0 && string.Equals(team, away, System.StringComparison.OrdinalIgnoreCase))
                return (day + "@ " + home).Trim();
            if (home.Length == 0 && away.Length == 0) return day.Trim().Length > 0 ? day.Trim() : null;
            return (day + away + " @ " + home).Trim();
        }

        private HtmlTag RenderCornerStatusBadge()
        {
            var tooltipId = $"newsbadge-tooltip-{_input.NewsId}-corner";

            var tooltipText = $"{_input.StatusTypeText} - {_input.StatusTypeTag}";
            if (!string.IsNullOrEmpty(_input.NewsTitle))
                tooltipText += $" - {_input.NewsTitle}";
            if (_input.IsUnofficial)
                tooltipText += " - Status is Unofficial";

            var wrap = new HtmlTag("span").AddClass("bm-tooltip-wrap news-card-test-status-corner-wrap");

            var badge = new HtmlTag("span")
                .AddClass("news-card-test-status-corner bm-tooltip-trigger")
                .Attr("data-bm-tooltip", tooltipId)
                .Attr("style", $"background:{NormalizeColor(_input.StatusTypeColorCode)}");

            var cornerIcon = StatusTagIcon(_input.StatusTypeTag);
            if (cornerIcon.HasValue)
                badge.AppendHtml(new Icon(new IconInput { Type = cornerIcon.Value, Size = 14, Color = "white" }).Render());
            badge.Append(new HtmlTag("span").Text(CornerText()));

            if (_input.IsUnofficial)
                badge.AppendHtml(new Icon(new IconInput { Type = IconType.UnofficialTag, Size = 16, Color = "white" }).Render());

            wrap.Append(badge);
            wrap.Append(new HtmlTag("div").AddClass("bm-tooltip-content").Attr("id", tooltipId).Text(tooltipText));

            return wrap;
        }
    }
}