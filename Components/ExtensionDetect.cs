using HtmlTags;

namespace RotoMonsterUI
{
    public class ExtensionDetect
    {
        private readonly ExtensionDetectInput _input;

        public ExtensionDetect(ExtensionDetectInput input)
        {
            _input = input;
        }

        public string Render()
        {
            var wrap = new HtmlTag("div").AddClass("rm-extension-detect");

            if (_input == null) return wrap.ToString();

            var id = string.IsNullOrEmpty(_input.Id) ? "rmExtension" : _input.Id;
            wrap.Attr("id", id);

            wrap.Append(State(id + "-prompt", _input.PromptCssClass, PromptBody()));
            wrap.Append(State(id + "-unsupported", _input.UnsupportedCssClass,
                Html(_input.UnsupportedHtml)));

            if (_input.ShowInstalled)
                wrap.Append(State(id + "-installed", _input.InstalledCssClass,
                    Html(_input.InstalledHtml)));

            if (!string.IsNullOrEmpty(_input.MinimumVersion))
                wrap.Append(State(id + "-outdated", _input.OutdatedCssClass, OutdatedBody()));

            wrap.Append(new HtmlTag("script").AppendHtml(Script(id)));

            return wrap.ToString();
        }

        private static HtmlTag State(string id, string cssClass, HtmlTag body)
        {
            var block = new HtmlTag("div")
                .Attr("id", id)
                .Attr("style", "display:none");

            if (!string.IsNullOrEmpty(cssClass)) block.AddClass(cssClass);

            block.Append(body);
            return block;
        }

        private static HtmlTag Html(string html)
        {
            return new HtmlTag("span").AppendHtml(html ?? "");
        }

        /// <summary>
        /// An explicit InstallUrl wins, otherwise it is built from the store
        /// id. With neither, the prompt still renders, just without a link -
        /// better than shipping a dead placeholder url.
        /// </summary>
        private string StoreUrl()
        {
            if (!string.IsNullOrEmpty(_input.InstallUrl)) return _input.InstallUrl;
            if (string.IsNullOrEmpty(_input.ExtensionId)) return null;

            return "https://chromewebstore.google.com/detail/" + _input.ExtensionId.Trim();
        }

        private HtmlTag OutdatedBody()
        {
            var url = string.IsNullOrEmpty(_input.UpdateUrl)
                ? StoreUrl()
                : _input.UpdateUrl;

            return Linked(_input.OutdatedHtml, url, _input.UpdateLinkText);
        }

        private HtmlTag PromptBody()
        {
            return Linked(_input.PromptHtml, StoreUrl(), _input.InstallLinkText);
        }

        private static HtmlTag Linked(string html, string url, string linkText)
        {
            var body = new HtmlTag("span").AppendHtml(html ?? "");

            if (!string.IsNullOrEmpty(url))
            {
                body.AppendHtml(" ");
                body.Append(new HtmlTag("a")
                    .Attr("href", url)
                    .Attr("target", "_blank")
                    .Attr("rel", "noopener")
                    .Text(linkText));
            }

            return body;
        }

        /// <summary>
        /// There is no api for "does this browser support extensions", so this
        /// identifies the browser instead. Chrome Web Store extensions run on
        /// desktop Chromium, and navigator.userAgentData is itself the signal
        /// since only Chromium implements it. The mobile flag matters more than
        /// it looks - Chrome on Android and iOS supports no extensions at all,
        /// so a plain is-this-Chrome test would prompt an install to people who
        /// could never finish it.
        ///
        /// The extension sets data-rm-extension on the html element and also
        /// answers an rm-extension-ping with its version, so either load order
        /// works.
        /// </summary>
        private string Script(string id)
        {
            var min = string.IsNullOrEmpty(_input.MinimumVersion)
                ? "null"
                : "\"" + _input.MinimumVersion + "\"";

            return @"
(function () {
    var TIMEOUT_MS = " + _input.TimeoutMs + @";
    var MIN_VERSION = " + min + @";
    var PREFIX = """ + id + @""";
    var settled = false;

    function browserCanInstall() {
        var uaData = navigator.userAgentData;
        if (uaData) return uaData.mobile === false;

        var ua = navigator.userAgent;
        var mobile = /Android|iPhone|iPad|iPod/i.test(ua);
        var chromium = /Chrome\/|Chromium\//.test(ua) && !/Edge\//.test(ua);
        return chromium && !mobile;
    }

    function show(suffix) {
        var el = document.getElementById(PREFIX + suffix);
        if (el) el.style.display = """";
    }

    function older(a, b) {
        var x = String(a).split("".""), y = String(b).split(""."");
        for (var i = 0; i < Math.max(x.length, y.length); i++) {
            var n = parseInt(x[i] || ""0"", 10), m = parseInt(y[i] || ""0"", 10);
            if (n !== m) return n < m;
        }
        return false;
    }

    function found(version) {
        if (settled) return;
        settled = true;

        if (MIN_VERSION && version && older(version, MIN_VERSION)) {
            show(""-outdated"");
            return;
        }

        show(""-installed"");
    }

    function missing() {
        if (settled) return;
        settled = true;
        show(""-prompt"");
    }

    if (!browserCanInstall()) {
        show(""-unsupported"");
        return;
    }

    document.addEventListener(""rm-extension-pong"", function (e) {
        found(e.detail && e.detail.version);
    });

    var marked = document.documentElement.getAttribute(""data-rm-extension"");
    if (marked) {
        found(marked);
    } else {
        document.dispatchEvent(new Event(""rm-extension-ping""));
        setTimeout(function () {
            var late = document.documentElement.getAttribute(""data-rm-extension"");
            if (late) found(late);
            else missing();
        }, TIMEOUT_MS);
    }
})();
";
        }
    }
}