using HtmlTags;

namespace RotoMonsterUI
{
    public class CollapseControl
    {
        private readonly CollapseControlInput _input;

        public CollapseControl(CollapseControlInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var contentId = $"{_input.Id}-content";
            var toggleId = $"{_input.Id}-toggle";
            var lockId = $"{_input.Id}-lock";
            var lockButtonId = $"{_input.Id}-lock-btn";

            // Chevron icon
            var chevronSvg = _input.IsExpanded
                ? @"<svg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><polyline points='6 9 12 15 18 9'/></svg>"
                : @"<svg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.5' stroke-linecap='round' stroke-linejoin='round'><polyline points='9 6 15 12 9 18'/></svg>";

            // Button
            var buttonClass = _input.ButtonStyle == ButtonStyle.Primary
                ? "modern-filter-btn modern-filter-btn-primary"
                : "modern-filter-btn modern-filter-btn-secondary";

            var button = new HtmlTag("button")
                .AddClass(buttonClass)
                .Attr("type", "button")
                .Attr("data-toggle", "collapse")
                .Attr("data-target", $"#{contentId}")
                .Attr("aria-expanded", _input.IsExpanded ? "true" : "false")
                .Attr("aria-controls", contentId);

            button.AppendHtml($"{_input.ButtonText}&nbsp;");
            button.AppendHtml(chevronSvg);

            // Header row holds the collapse button and, optionally, the lock toggle
            var header = new HtmlTag("div").AddClass("collapse-control-header");
            header.Append(button);

            if (_input.ShowLock)
            {
                var lockIcon = new Icon(new IconInput
                {
                    Type = _input.IsLocked ? IconType.Lock : IconType.LockOpen,
                    Size = 16
                }).Render();

                var lockButton = new HtmlTag("button")
                    .AddClass("modern-filter-btn modern-filter-btn-icon-only collapse-control-lock")
                    .Attr("type", "button")
                    .Attr("id", lockButtonId)
                    .Attr("data-collapse-lock", _input.Id)
                    .Attr("aria-pressed", _input.IsLocked ? "true" : "false")
                    .Attr("title", _input.IsLocked ? _input.UnlockTitle : _input.LockTitle);

                if (_input.IsLocked)
                    lockButton.AddClass("is-locked");

                if (_input.LockPostsBack)
                    lockButton.Attr("data-collapse-lock-postback", "1");

                lockButton.AppendHtml(lockIcon);
                header.Append(lockButton);

                var lockHidden = new HtmlTag("input")
                    .Attr("type", "hidden")
                    .Attr("name", lockId)
                    .Attr("id", lockId)
                    .Attr("value", _input.IsLocked ? "1" : "0");

                header.Append(lockHidden);
            }

            // Hidden field for postback state
            var hidden = new HtmlTag("input")
                .Attr("type", "hidden")
                .Attr("name", toggleId)
                .Attr("id", toggleId)
                .Attr("value", _input.IsExpanded ? "1" : "0");

            // Collapsible content
            var contentDiv = new HtmlTag("div")
                .Attr("id", contentId)
                .AddClass(_input.IsExpanded ? "collapse show" : "collapse");

            contentDiv.AppendHtml(_input.CollapsibleHtml);

            // Wrapper
            var wrapper = new HtmlTag("div").Attr("id", _input.Id);
            wrapper.Append(header);
            wrapper.Append(hidden);
            wrapper.Append(contentDiv);

            return wrapper.ToString();
        }
    }
}