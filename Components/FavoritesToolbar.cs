using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class FavoritesToolbar
    {
        private readonly FavoritesToolbarInput _input;

        public FavoritesToolbar(FavoritesToolbarInput input)
        {
            _input = input;
        }

        public string Render()
        {
            bool hasCurrent = _input.CurrentPage != null && !string.IsNullOrEmpty(_input.CurrentPage.PageId);
            if ((_input.Pages == null || _input.Pages.Count == 0) && !hasCurrent)
                return "";

            bool allowReorder = _input.AllowReorder && _input.Pages != null && _input.Pages.Count >= 2;

            var wrapper = new HtmlTag("div").AddClass("favorites-toolbar");
            if (allowReorder)
                wrapper.Attr("data-favorites-id", _input.Id);

            var starIcon = new Icon(new IconInput { Type = IconType.Favorite, Size = 20 }).Render();
            var label = new HtmlTag("span")
                .AddClass("favorites-toolbar-label")
                .AppendHtml(new CustomTooltip(starIcon, "Favorites").Render());
            wrapper.Append(label);

            foreach (var page in _input.Pages)
            {
                bool isCurrent = hasCurrent && page.PageId == _input.CurrentPage.PageId;

                var pill = new HtmlTag("span")
                    .AddClass("favorites-toolbar-pill")
                    .Attr("data-pageid", page.PageId);

                if (isCurrent)
                    pill.AddClass("favorites-toolbar-pill--current");

                if (allowReorder)
                {
                    var handle = new HtmlTag("span")
                        .AddClass("favorites-toolbar-handle")
                        .Attr("draggable", "true")
                        .Attr("aria-label", $"Drag to reorder {page.Name}");
                    handle.AppendHtml(new Icon(new IconInput { Type = IconType.DragHandle, Size = 14, Color = "currentColor" }).Render());
                    pill.Append(handle);
                }

                var link = new HtmlTag("a")
                    .AddClass("favorites-toolbar-link")
                    .Attr("href", page.Url)
                    .Text(page.Name);
                pill.Append(link);

                if (isCurrent)
                {
                    var hideBtn = new HtmlTag("button")
                        .AddClass("favorites-toolbar-hide-btn")
                        .Attr("type", "button")
                        .Attr("name", $"{_input.Id}_hide_{page.PageId}")
                        .Attr("aria-label", $"Remove {page.Name} from favorites")
                        .Attr("onclick", $"TriggerPostBack(this, '{_input.Id}_hide_', 'data-pageid')")
                        .Attr("data-pageid", page.PageId);
                    hideBtn.AppendHtml(new Icon(new IconInput { Type = IconType.Trash, Size = 12, Color = "currentColor" }).Render());
                    pill.Append(hideBtn);
                }

                wrapper.Append(pill);
            }

            if (hasCurrent)
            {
                bool isFavorited = _input.Pages.Exists(p => p.PageId == _input.CurrentPage.PageId);

                if (!isFavorited && _input.Pages.Count < _input.MaxPages)
                {
                    var addBtn = new HtmlTag("button")
                        .AddClass("modern-filter-btn favorites-toolbar-current-btn favorites-toolbar-add")
                        .Attr("type", "button")
                        .Attr("name", $"{_input.Id}_addcurrent_{_input.CurrentPage.PageId}")
                        .Attr("aria-label", $"Add {_input.CurrentPage.Name} to favorites")
                        .Attr("onclick", $"TriggerPostBack(this, '{_input.Id}_addcurrent_', 'data-pageid')")
                        .Attr("data-pageid", _input.CurrentPage.PageId);
                    addBtn.AppendHtml(new Icon(new IconInput { Type = IconType.Plus, Size = 12, Color = "currentColor" }).Render());
                    addBtn.AppendHtml($"<span style='margin-left:0.35rem;'>Add {System.Net.WebUtility.HtmlEncode(_input.CurrentPage.Name)} to Favorites</span>");                    wrapper.Append(addBtn);
                }
            }

            if (allowReorder)
            {
                var ids = new List<string>();
                foreach (var p in _input.Pages)
                    ids.Add(p.PageId);

                var orderField = new HtmlTag("input")
                    .Attr("type", "hidden")
                    .Attr("id", $"{_input.Id}_order")
                    .Attr("name", $"{_input.Id}_order")
                    .Attr("value", string.Join(",", ids));
                wrapper.Append(orderField);
            }

            return wrapper.ToString();
        }
    }
}