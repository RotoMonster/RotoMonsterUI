namespace RotoMonsterUI
{
    public class ExtensionDetectInput
    {
        public string Id { get; set; } = "rmExtension";

        /// <summary>
        /// The Chrome Web Store id. Permanent once the listing is published,
        /// so this is the one thing worth setting in one place. InstallUrl is
        /// built from it.
        /// </summary>
        public string ExtensionId { get; set; }

        /// <summary>
        /// Set this to override the store link entirely, e.g. to point at a
        /// help page instead. Otherwise it is built from ExtensionId.
        /// </summary>
        public string InstallUrl { get; set; }

        public string InstallLinkText { get; set; } = "Install";

        public string PromptHtml { get; set; }
            = "Install the RotoMonster extension to import your draft.";

        public string UnsupportedHtml { get; set; }
            = "Draft import needs desktop Chrome, Edge, Brave or another Chromium browser.";

        public string InstalledHtml { get; set; }
            = "The RotoMonster Chrome Extension is installed so use the button in the "
              + "upper-right to refresh your draft once your ESPN Draft Room is open in "
              + "a separate tab.";

        public bool ShowInstalled { get; set; } = true;

        public string PromptCssClass { get; set; } = "rmx-note rmx-note--prompt";
        public string UnsupportedCssClass { get; set; } = "rmx-note rmx-note--unsupported";
        public string InstalledCssClass { get; set; } = "rmx-note rmx-note--installed";

        public int TimeoutMs { get; set; } = 400;

        /// <summary>
        /// Nothing renders until the check settles, so the common case goes
        /// straight to the right message with no flicker. These bound how long
        /// it waits before deciding the extension is not there - LateTries
        /// polls of LateDelayMs, on top of TimeoutMs.
        /// </summary>
        public int LateTries { get; set; } = 6;

        public int LateDelayMs { get; set; } = 350;

        /// <summary>
        /// Reload the page once when the tab comes back into focus, but only
        /// while the install prompt is showing.
        ///
        /// Chrome does not inject a content script into a page that was already
        /// open, so someone who follows the install link and comes back finds a
        /// page that still says the extension is missing, and no amount of
        /// re-checking can see it. A reload is the only thing that works.
        ///
        /// Safe by construction rather than by luck - it only fires when the
        /// prompt is up, which means there is no extension, which means nothing
        /// has been imported to lose. Off if you would rather tell people to
        /// refresh themselves.
        /// </summary>
        public bool ReloadOnReturn { get; set; } = true;

        public string MinimumVersion { get; set; }

        public string OutdatedHtml { get; set; }
            = "Your RotoMonster extension is out of date. Update it to import your draft.";

        /// <summary>
        /// Where Update sends them. Falls back to InstallUrl when not set,
        /// since the store listing is where you update from anyway.
        /// </summary>
        public string UpdateUrl { get; set; }

        public string UpdateLinkText { get; set; } = "Update";

        public string OutdatedCssClass { get; set; } = "rmx-note rmx-note--outdated";
    }
}