using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class HelpTopic
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string WhenText { get; set; }
        public string BodyHtml { get; set; }
        public List<string> SearchTerms { get; set; } = new List<string>();
        public bool IsExpanded { get; set; }
        public bool ShowAnchor { get; set; } = true;
        public bool FlushBody { get; set; }
    }

    public class HelpGroup
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string HintText { get; set; }
        public string TocTitle { get; set; }
        public List<HelpTopic> Topics { get; set; } = new List<HelpTopic>();
    }

    public class HelpToolLink
    {
        public string Text { get; set; }
        public string Url { get; set; }
    }

    public class HelpTool
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public List<string> SearchTerms { get; set; } = new List<string>();
        public List<HelpToolLink> Links { get; set; } = new List<HelpToolLink>();
    }

    public class HelpPageInput
    {
        public string Id { get; set; } = "helpPage";

        public bool ShowSearch { get; set; } = true;
        public string SearchPlaceholder { get; set; } = "Search help...";
        public string SearchHintText { get; set; } = "Press / to search, Esc to clear.";
        public string TopicsWord { get; set; } = "topics";
        public string MatchWord { get; set; } = "match";
        public string MatchesWord { get; set; } = "matches";
        public string EmptyHtml { get; set; }
            = "Nothing matches that. Try a shorter word.";

        public bool ShowContents { get; set; } = true;
        public string ContentsTitle { get; set; } = "Contents";

        public string CalloutHtml { get; set; }

        public List<HelpGroup> Groups { get; set; } = new List<HelpGroup>();

        public string ToolsGroupId { get; set; } = "tools";
        public string ToolsGroupTitle { get; set; } = "Tools";
        public string ToolsHintText { get; set; }
        public List<HelpTool> Tools { get; set; } = new List<HelpTool>();

        public string FromToolName { get; set; }
        public string FromToolNoteText { get; set; } = "Its card is first below.";
        public string FromToolFormat { get; set; } = "You came from {0}";

        public string StuckTitle { get; set; }
        public string StuckText { get; set; }
        public List<HelpToolLink> StuckLinks { get; set; } = new List<HelpToolLink>();

        public string AnchorText { get; set; } = "Link";
        public string AnchorCopiedText { get; set; } = "Copied";
    }
}