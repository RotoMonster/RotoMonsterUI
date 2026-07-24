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
            if (isShaded)
                card.Attr("style", $"background:{ageShadeColor};");

            // ---- Header row ----
            var headerRow = new HtmlTag("div").AddClass("news-card-header");

            headerRow.Append(RenderStatusBadge());

            if (_input.DisplayPlayerInput != null)
            {
                if (string.IsNullOrEmpty(_input.DisplayPlayerInput.TeamColor))
                {

                    _input.DisplayPlayerInput.TeamColor = _input.Sport == NewsCardSport.NBA
                        ? TeamColorHelper.GetNbaTeamColorVar(_input.DisplayPlayerInput.TeamCode)
                        : TeamColorHelper.GetTeamColorVar(_input.DisplayPlayerInput.TeamCode);
                }

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
                headerRow.AppendHtml(new DisplayPlayer(displayPlayerInput).Render());
            }
            else if (_input.DisplayTeamInput != null)
            {
                var teamColor = isShaded
                    ? ColorHelper.GetAutoColorForLightBackground(_input.DisplayTeamInput.ColorCode)
                    : _input.DisplayTeamInput.ColorCode;

                var teamSpan = new HtmlTag("span")
                    .AddClass("news-card-team")
                    .Attr("style", $"color:{NormalizeColor(teamColor)}")
                    .Text(_input.DisplayTeamInput.Name);
                headerRow.Append(teamSpan);
            }

            var headerRight = new HtmlTag("div").AddClass("news-card-header-right");
            if (isShaded) headerRight.AddClass("color-shaded");

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
                if (isShaded) rosterNames.AddClass("color-shaded");
                rosterRow.Append(rosterNames);

                var teamLevelColor = LevelColor(_input.NewsLevel);
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
                if (isShaded) statusText.AddClass("color-shaded");
                statusRow.Append(statusText);

                if (!string.IsNullOrEmpty(_input.NewsTitle))
                {
                    var newsTitle = new HtmlTag("span").AddClass("news-card-news-title").Text(_input.NewsTitle);
                    if (isShaded) newsTitle.AddClass("color-shaded");
                    statusRow.Append(newsTitle);
                }

                var levelColor = LevelColor(_input.NewsLevel);
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
                if (isShaded) detailsDiv.AddClass("color-shaded");
                card.Append(detailsDiv);
            }

            // ---- Applied news tags ----
            if (_input.NewsTags != null && _input.NewsTags.Count > 0)
            {
                var tagList = new HtmlTag("div").AddClass("news-card-tag-list");
                foreach (var tag in _input.NewsTags)
                {
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
                card.Append(tagList);
            }

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
                        if (isShaded) editBtn.AddClass("color-shaded");
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
                    if (isShaded) countsRow.AddClass("color-shaded");
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

        private HtmlTag RenderStatusBadge(string context = "main")
        {
            var tooltipId = $"newsbadge-tooltip-{_input.NewsId}-{context}";

            var tooltipParts = new List<string>();
            if (!string.IsNullOrEmpty(_input.StatusTypeText)) tooltipParts.Add(_input.StatusTypeText);
            if (!string.IsNullOrEmpty(_input.StatusTypeTag)) tooltipParts.Add(_input.StatusTypeTag);
            if (!string.IsNullOrEmpty(_input.NewsTitle)) tooltipParts.Add(_input.NewsTitle);
            if (_input.IsUnofficial) tooltipParts.Add("Status is Unofficial");
            var tooltipText = string.Join(" - ", tooltipParts);

            var wrap = new HtmlTag("span").AddClass("bm-tooltip-wrap");

            var badge = new HtmlTag("span")
                .AddClass("news-card-status-badge bm-tooltip-trigger")
                .Attr("data-bm-tooltip", tooltipId)
                .Attr("style", $"background:{NormalizeColor(_input.StatusTypeColorCode)}");

            badge.Append(new HtmlTag("span").Text(_input.StatusTypeAbbreviation));

            var tagIcon = StatusTagIcon(_input.StatusTypeTag);
            if (tagIcon.HasValue)
                badge.AppendHtml(new Icon(new IconInput { Type = tagIcon.Value, Size = 14, Color = "white" }).Render());

            if (_input.IsUnofficial)
                badge.AppendHtml(new Icon(new IconInput { Type = IconType.UnofficialTag, Size = 16, Color = "white" }).Render());

            wrap.Append(badge);
            wrap.Append(new HtmlTag("div").AddClass("bm-tooltip-content").Attr("id", tooltipId).Text(tooltipText));

            return wrap;
        }

        private HtmlTag RenderEditForm()
        {
            var currentStatusRow = new HtmlTag("div").AddClass("news-card-field-row");
            currentStatusRow.Append(new HtmlTag("span").AddClass("news-card-field-label").Text("Current Status"));
            currentStatusRow.Append(RenderStatusBadge("edit"));

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
            card.Attr("style", $"background:{cardTint};");

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

            var levelColor = LevelColor(_input.NewsLevel);
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
                card.AppendHtml(new DisplayPlayer(_input.DisplayPlayerInput).Render());
            }

            var bodyParts = new List<string>();
            if (!string.IsNullOrEmpty(_input.NewsTitle)) bodyParts.Add(_input.NewsTitle);
            if (!string.IsNullOrEmpty(_input.NewsDetails)) bodyParts.Add(_input.NewsDetails);
            var bodyContent = string.Join(" - ", bodyParts);
            if (!string.IsNullOrEmpty(bodyContent))
            {
                var bodyRow = new HtmlTag("div").AddClass("news-card-test-body-row");

                var tagIcon = StatusTagIcon(_input.StatusTypeTag);
                if (tagIcon.HasValue)
                {
                    var tagTooltipId = $"newsbadge-tooltip-{_input.NewsId}-tag";
                    var tagIconWrap = new HtmlTag("span").AddClass("bm-tooltip-wrap");
                    var tagIconTrigger = new HtmlTag("span")
                        .AddClass("news-card-test-tag-icon bm-tooltip-trigger")
                        .Attr("data-bm-tooltip", tagTooltipId);
                    tagIconTrigger.AppendHtml(new Icon(new IconInput { Type = tagIcon.Value, Size = 16, Color = "currentColor" }).Render());
                    tagIconWrap.Append(tagIconTrigger);
                    tagIconWrap.Append(new HtmlTag("div").AddClass("bm-tooltip-content").Attr("id", tagTooltipId).Text(_input.StatusTypeTag));
                    bodyRow.Append(tagIconWrap);
                }

                var bodyText = new HtmlTag("span").AddClass("news-card-test-body").Text(bodyContent);
                bodyRow.Append(bodyText);

                card.Append(bodyRow);
            }

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
                .Attr("style", $"background:{NormalizeColor(_input.StatusTypeColorCode)}")
                .Text((_input.StatusTypeText ?? "").ToUpper());

            if (_input.IsUnofficial)
                badge.AppendHtml(new Icon(new IconInput { Type = IconType.UnofficialTag, Size = 16, Color = "white" }).Render());

            wrap.Append(badge);
            wrap.Append(new HtmlTag("div").AddClass("bm-tooltip-content").Attr("id", tooltipId).Text(tooltipText));

            return wrap;
        }
    }
}