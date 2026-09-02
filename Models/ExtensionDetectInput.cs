namespace RotoMonsterUI
{
    public class ExtensionDetectInput
    {
        public string Id { get; set; } = "rmExtension";

        public string ExtensionId { get; set; }

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
        public int LateTries { get; set; } = 6;

        public int LateDelayMs { get; set; } = 350;

        public bool ReloadOnReturn { get; set; } = true;

        public string MinimumVersion { get; set; }

        public string OutdatedHtml { get; set; }
            = "Your RotoMonster extension is out of date. Update it to import your draft.";

        public string UpdateUrl { get; set; }

        public string UpdateLinkText { get; set; } = "Update";

        public string OutdatedCssClass { get; set; } = "rmx-note rmx-note--outdated";
    }
}