using System.Collections.Generic;
using HtmlTags;

namespace RotoMonsterUI
{
    public class HelpPage
    {
        private readonly HelpPageInput _input;

        public HelpPage(HelpPageInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("help-page");

            if (_input == null) return wrap.ToString();

            var id = _input.Id ?? "helpPage";
            wrap.Attr("id", "help-page-" + id);

            if (_input.ShowSearch) wrap.Append(RenderSearch(id));

            var layout = new HtmlTag("div").AddClass("hp-layout");

            if (_input.ShowContents)
            {
                var toc = RenderContents();
                if (toc != null) layout.Append(toc);
                else layout.AddClass("hp-layout--nocontents");
            }
            else
            {
                layout.AddClass("hp-layout--nocontents");
            }

            var main = new HtmlTag("main").AddClass("hp-main");

            if (!string.IsNullOrEmpty(_input.FromToolName))
                main.Append(RenderFromTool());

            if (!string.IsNullOrEmpty(_input.CalloutHtml))
                main.Append(new HtmlTag("div").AddClass("hp-callout").AppendHtml(_input.CalloutHtml));

            foreach (var group in _input.Groups ?? new List<HelpGroup>())
            {
                if (group == null) continue;
                main.Append(RenderGroup(group));
            }

            if (_input.Tools != null && _input.Tools.Count > 0)
                main.Append(RenderTools());

            if (!string.IsNullOrEmpty(_input.StuckTitle) || !string.IsNullOrEmpty(_input.StuckText))
                main.Append(RenderStuck());

            main.Append(new HtmlTag("div")
                .AddClass("hp-empty")
                .Attr("hidden", "hidden")
                .AppendHtml(_input.EmptyHtml ?? ""));

            layout.Append(main);
            wrap.Append(layout);

            wrap.Append(new HtmlTag("script").AppendHtml(Script(id)));

            return wrap.ToString();
        }

        private HtmlTag RenderSearch(string id)
        {
            var bar = new HtmlTag("div").AddClass("hp-search");

            bar.Append(new HtmlTag("input")
                .Attr("type", "search")
                .Attr("id", "hpq_" + id)
                .Attr("data-hp-search", "1")
                .Attr("placeholder", _input.SearchPlaceholder));

            bar.Append(new HtmlTag("span").AddClass("hp-count").Attr("data-hp-count", "1"));

            if (!string.IsNullOrEmpty(_input.SearchHintText))
                bar.Append(new HtmlTag("span").AddClass("hp-hint").Text(_input.SearchHintText));

            return bar;
        }

        private HtmlTag RenderContents()
        {
            var groups = _input.Groups ?? new List<HelpGroup>();
            var hasTools = _input.Tools != null && _input.Tools.Count > 0;

            if (groups.Count == 0 && !hasTools) return null;

            var nav = new HtmlTag("nav").AddClass("hp-toc");

            foreach (var group in groups)
            {
                if (group == null) continue;

                var heading = string.IsNullOrEmpty(group.TocTitle) ? group.Title : group.TocTitle;

                if (!string.IsNullOrEmpty(heading))
                    nav.Append(new HtmlTag("h3").Text(heading));

                foreach (var topic in group.Topics ?? new List<HelpTopic>())
                {
                    if (topic == null || string.IsNullOrEmpty(topic.Id)) continue;

                    nav.Append(new HtmlTag("a")
                        .Attr("href", "#" + topic.Id)
                        .Text(topic.Title ?? topic.Id));
                }
            }

            if (hasTools)
            {
                nav.Append(new HtmlTag("h3").Text(_input.ToolsGroupTitle));
                nav.Append(new HtmlTag("a")
                    .Attr("href", "#" + _input.ToolsGroupId)
                    .Text(_input.ToolsGroupTitle));
            }

            return nav;
        }

        private HtmlTag RenderFromTool()
        {
            var box = new HtmlTag("div").AddClass("hp-fromtool");

            box.Append(new HtmlTag("b")
                .Text(string.Format(_input.FromToolFormat, _input.FromToolName)));

            if (!string.IsNullOrEmpty(_input.FromToolNoteText))
                box.Append(new HtmlTag("span").AddClass("hp-hint").Text(_input.FromToolNoteText));

            return box;
        }

        private HtmlTag RenderGroup(HelpGroup group)
        {
            var section = new HtmlTag("section").AddClass("hp-group").Attr("data-hp-group", "1");

            if (!string.IsNullOrEmpty(group.Id)) section.Attr("id", group.Id);

            if (!string.IsNullOrEmpty(group.Title) || !string.IsNullOrEmpty(group.HintText))
            {
                var head = new HtmlTag("div").AddClass("hp-group-head");

                if (!string.IsNullOrEmpty(group.Title))
                    head.Append(new HtmlTag("h2").Text(group.Title));

                if (!string.IsNullOrEmpty(group.HintText))
                    head.Append(new HtmlTag("span").AddClass("hp-hint").Text(group.HintText));

                section.Append(head);
            }

            foreach (var topic in group.Topics ?? new List<HelpTopic>())
            {
                if (topic == null) continue;
                section.Append(RenderTopic(topic));
            }

            return section;
        }

        private HtmlTag RenderTopic(HelpTopic topic)
        {
            var panel = new HtmlTag("div").AddClass("hp-panel").Attr("data-hp-topic", "1");

            if (!string.IsNullOrEmpty(topic.Id)) panel.Attr("id", topic.Id);

            var terms = topic.SearchTerms != null && topic.SearchTerms.Count > 0
                ? string.Join(" ", topic.SearchTerms)
                : "";

            if (terms.Length > 0) panel.Attr("data-hp-terms", terms);

            var contentId = (topic.Id ?? "hp") + "-content";

            var head = new HtmlTag("button")
                .AddClass("hp-panel-head")
                .Attr("type", "button")
                .Attr("data-toggle", "collapse")
                .Attr("data-target", "#" + contentId)
                .Attr("aria-controls", contentId)
                .Attr("aria-expanded", topic.IsExpanded ? "true" : "false");

            head.Append(new HtmlTag("span").AddClass("hp-caret").AppendHtml("&#9662;"));
            head.Append(new HtmlTag("span").AddClass("hp-panel-title").Text(topic.Title ?? ""));

            if (!string.IsNullOrEmpty(topic.WhenText))
                head.Append(new HtmlTag("span").AddClass("hp-when").Text(topic.WhenText));

            if (topic.ShowAnchor && !string.IsNullOrEmpty(topic.Id))
                head.Append(new HtmlTag("span")
                    .AddClass("hp-anchor")
                    .Attr("data-hp-anchor", topic.Id)
                    .Attr("data-hp-copied", _input.AnchorCopiedText)
                    .Text(_input.AnchorText));

            panel.Append(head);

            var body = new HtmlTag("div")
                .Attr("id", contentId)
                .AddClass(topic.IsExpanded ? "hp-panel-body collapse show" : "hp-panel-body collapse");

            if (topic.FlushBody) body.AddClass("hp-panel-body--flush");

            body.AppendHtml(topic.BodyHtml ?? "");
            panel.Append(body);

            return panel;
        }

        private HtmlTag RenderTools()
        {
            var section = new HtmlTag("section")
                .AddClass("hp-group")
                .Attr("data-hp-group", "1")
                .Attr("id", _input.ToolsGroupId);

            var head = new HtmlTag("div").AddClass("hp-group-head");
            head.Append(new HtmlTag("h2").Text(_input.ToolsGroupTitle));

            if (!string.IsNullOrEmpty(_input.ToolsHintText))
                head.Append(new HtmlTag("span").AddClass("hp-hint").Text(_input.ToolsHintText));

            section.Append(head);

            var cards = new HtmlTag("div").AddClass("hp-cards");

            var tools = new List<HelpTool>();
            HelpTool first = null;

            foreach (var tool in _input.Tools)
            {
                if (tool == null) continue;

                if (first == null && !string.IsNullOrEmpty(_input.FromToolName)
                    && string.Equals(tool.Name, _input.FromToolName,
                        System.StringComparison.OrdinalIgnoreCase))
                {
                    first = tool;
                    continue;
                }

                tools.Add(tool);
            }

            if (first != null) tools.Insert(0, first);

            foreach (var tool in tools) cards.Append(RenderTool(tool));

            section.Append(cards);

            return section;
        }

        private HtmlTag RenderTool(HelpTool tool)
        {
            var card = new HtmlTag("div").AddClass("hp-card").Attr("data-hp-topic", "1");

            var terms = tool.SearchTerms != null && tool.SearchTerms.Count > 0
                ? string.Join(" ", tool.SearchTerms)
                : "";

            if (terms.Length > 0) card.Attr("data-hp-terms", terms);

            var title = new HtmlTag("h4");

            if (string.IsNullOrEmpty(tool.Url))
                title.Text(tool.Name ?? "");
            else
                title.Append(new HtmlTag("a").Attr("href", tool.Url).Text(tool.Name ?? ""));

            card.Append(title);

            if (!string.IsNullOrEmpty(tool.Description))
                card.Append(new HtmlTag("p").Text(tool.Description));

            if (tool.Links != null && tool.Links.Count > 0)
            {
                var links = new HtmlTag("div").AddClass("hp-card-links");

                foreach (var link in tool.Links)
                {
                    if (link == null || string.IsNullOrEmpty(link.Text)) continue;

                    links.Append(new HtmlTag("a")
                        .Attr("href", string.IsNullOrEmpty(link.Url) ? "#" : link.Url)
                        .Text(link.Text));
                }

                card.Append(links);
            }

            return card;
        }

        private HtmlTag RenderStuck()
        {
            var box = new HtmlTag("section").AddClass("hp-stuck");

            if (!string.IsNullOrEmpty(_input.StuckTitle))
                box.Append(new HtmlTag("h3").Text(_input.StuckTitle));

            if (!string.IsNullOrEmpty(_input.StuckText))
                box.Append(new HtmlTag("p").Text(_input.StuckText));

            if (_input.StuckLinks != null && _input.StuckLinks.Count > 0)
            {
                var links = new HtmlTag("div").AddClass("hp-stuck-links");

                foreach (var link in _input.StuckLinks)
                {
                    if (link == null || string.IsNullOrEmpty(link.Text)) continue;

                    links.Append(new HtmlTag("a")
                        .Attr("href", string.IsNullOrEmpty(link.Url) ? "#" : link.Url)
                        .Text(link.Text));
                }

                box.Append(links);
            }

            return box;
        }

        private string Script(string id)
        {
            var scope = "#help-page-" + id;

            return @"
(function () {
    var root = document.querySelector('" + scope + @"');
    if (!root) return;

    var search = root.querySelector('[data-hp-search]');
    var count = root.querySelector('[data-hp-count]');
    var empty = root.querySelector('.hp-empty');
    var topics = root.querySelectorAll('[data-hp-topic]');
    var groups = root.querySelectorAll('[data-hp-group]');

    var topicsWord = '" + Js(_input.TopicsWord) + @"';
    var matchWord = '" + Js(_input.MatchWord) + @"';
    var matchesWord = '" + Js(_input.MatchesWord) + @"';

    function setCount(text) { if (count) count.textContent = text; }

    setCount(topics.length + ' ' + topicsWord);

    function clearMarks(node) {
        node.querySelectorAll('mark').forEach(function (m) {
            var parent = m.parentNode;
            parent.replaceChild(document.createTextNode(m.textContent), m);
            parent.normalize();
        });
    }

    function mark(node, query) {
        var walker = document.createTreeWalker(node, NodeFilter.SHOW_TEXT, null, false);
        var found = [];
        var current;

        while ((current = walker.nextNode())) {
            if (current.parentNode.tagName === 'MARK') continue;
            if (current.nodeValue.toLowerCase().indexOf(query) > -1) found.push(current);
        }

        found.forEach(function (textNode) {
            var text = textNode.nodeValue;
            var frag = document.createDocumentFragment();
            var at = 0;
            var hit = text.toLowerCase().indexOf(query, at);

            while (hit > -1) {
                frag.appendChild(document.createTextNode(text.slice(at, hit)));
                var em = document.createElement('mark');
                em.textContent = text.slice(hit, hit + query.length);
                frag.appendChild(em);
                at = hit + query.length;
                hit = text.toLowerCase().indexOf(query, at);
            }

            frag.appendChild(document.createTextNode(text.slice(at)));
            textNode.parentNode.replaceChild(frag, textNode);
        });
    }

    function open(panel) {
        var head = panel.querySelector('[data-toggle]');
        if (!head) return;

        head.setAttribute('aria-expanded', 'true');

        var body = panel.querySelector('.hp-panel-body');
        if (body) body.classList.add('show');
    }

    if (search) {
        search.addEventListener('input', function () {
            var query = search.value.trim().toLowerCase();
            var shown = 0;

            topics.forEach(clearMarks);

            topics.forEach(function (topic) {
                var terms = topic.getAttribute('data-hp-terms') || '';
                var hay = (terms + ' ' + topic.textContent).toLowerCase();
                var hit = !query || hay.indexOf(query) > -1;

                topic.hidden = !hit;
                if (!hit) return;

                shown++;

                if (query) {
                    open(topic);
                    mark(topic, query);
                }
            });

            groups.forEach(function (group) {
                var any = false;

                group.querySelectorAll('[data-hp-topic]').forEach(function (t) {
                    if (!t.hidden) any = true;
                });

                group.hidden = !any;
            });

            setCount(query
                ? shown + ' ' + (shown === 1 ? matchWord : matchesWord)
                : topics.length + ' ' + topicsWord);

            if (empty) empty.hidden = shown > 0;
        });
    }

    root.querySelectorAll('[data-hp-anchor]').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            e.preventDefault();

            location.hash = btn.getAttribute('data-hp-anchor');

            var was = btn.textContent;
            btn.textContent = btn.getAttribute('data-hp-copied') || was;
            setTimeout(function () { btn.textContent = was; }, 1200);
        });
    });

    var links = root.querySelectorAll('.hp-toc a');

    window.addEventListener('scroll', function () {
        var best = null;

        links.forEach(function (link) {
            var target = document.getElementById(link.getAttribute('href').slice(1));
            if (target && target.getBoundingClientRect().top < 120) best = link;
        });

        links.forEach(function (link) { link.classList.remove('is-current'); });
        if (best) best.classList.add('is-current');
    });

    function openHash() {
        var hash = location.hash.slice(1);
        if (!hash) return;

        var panel = root.querySelector('#' + hash);
        if (!panel || !panel.classList.contains('hp-panel')) return;

        open(panel);
        panel.scrollIntoView({ block: 'start', behavior: 'smooth' });
        panel.classList.add('hp-flash');
        setTimeout(function () { panel.classList.remove('hp-flash'); }, 1600);
    }

    window.addEventListener('hashchange', openHash);

    document.addEventListener('keydown', function (e) {
        if (!search) return;

        if (e.key === '/' && document.activeElement !== search) {
            e.preventDefault();
            search.focus();
            search.select();
            return;
        }

        if (e.key === 'Escape' && document.activeElement === search && search.value) {
            search.value = '';
            search.dispatchEvent(new Event('input'));
        }
    });

    openHash();
})();
";
        }

        private static string Js(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\\", "\\\\").Replace("'", "\\'");
        }
    }
}