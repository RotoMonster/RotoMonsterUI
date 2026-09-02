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
    var LATE_TRIES = " + _input.LateTries + @";
    var LATE_DELAY_MS = " + _input.LateDelayMs + @";
    var RELOAD_ON_RETURN = " + (_input.ReloadOnReturn ? "true" : "false") + @";
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

    function hide(suffix) {
        var el = document.getElementById(PREFIX + suffix);
        if (el) el.style.display = ""none"";
    }

    // Deliberately not guarded by settled. The timeout below may already have
    // shown the install prompt, and a late answer has to be able to take it
    // back down - the extension runs at document_idle, so on a slow page it
    // can easily answer after we have given up on it.
    function found(version) {
        settled = true;

        hide(""-prompt"");

        if (MIN_VERSION && version && older(version, MIN_VERSION)) {
            hide(""-installed"");
            show(""-outdated"");
            return;
        }

        hide(""-outdated"");
        show(""-installed"");
    }

    function missing() {
        if (settled) return;
        // Not settled - the listener stays live so a late answer still wins.
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
        return;
    }

    document.dispatchEvent(new Event(""rm-extension-ping""));

    setTimeout(function () {
        var late = document.documentElement.getAttribute(""data-rm-extension"");
        if (late) {
            found(late);
            return;
        }

        // Nothing yet. Keep looking rather than showing the prompt now - the
        // extension runs at document_idle so it commonly answers just after
        // this point, and showing an install prompt only to replace it a
        // moment later reads worse than showing nothing for a beat.
        //
        // A second ping covers an extension that loaded between the first one
        // and now; the marker is polled after in case it never answers a ping.
        document.dispatchEvent(new Event(""rm-extension-ping""));

        var tries = 0;
        var poll = setInterval(function () {
            tries++;

            var mark = document.documentElement.getAttribute(""data-rm-extension"");
            if (mark) {
                found(mark);
                clearInterval(poll);
                return;
            }

            // Out of patience. It really is not there, so say so.
            if (tries >= LATE_TRIES) {
                clearInterval(poll);
                missing();
                watchForInstall();
            }
        }, LATE_DELAY_MS);
    }, TIMEOUT_MS);

    // Someone who follows the install link comes back to this tab expecting it
    // to have noticed. Chrome does not inject into a page that is already open,
    // so the marker only appears after a reload - but the pong listener is
    // still live, and re-pinging on focus catches an extension that loaded
    // while the tab was in the background.
    function watchForInstall() {
        // Only reload if they actually left. Clicking around this page should
        // not trigger it, only coming back from somewhere else.
        var wentAway = false;
        var reloaded = false;

        function recheck() {
            if (settled) return;

            var mark = document.documentElement.getAttribute(""data-rm-extension"");
            if (mark) {
                found(mark);
                return;
            }

            // Nothing in the page to ask. Chrome does not inject into a tab
            // that was already open, so an extension installed since this
            // loaded is invisible here until the page reloads.
            if (RELOAD_ON_RETURN && wentAway && !reloaded) {
                reloaded = true;
                location.reload();
            }
        }

        window.addEventListener(""blur"", function () { wentAway = true; });

        document.addEventListener(""visibilitychange"", function () {
            if (document.hidden) wentAway = true;
            else recheck();
        });

        window.addEventListener(""focus"", recheck);
    }
})();
";
        }
    }
}