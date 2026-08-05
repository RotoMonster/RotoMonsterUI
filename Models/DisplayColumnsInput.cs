using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class ColumnItem
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public bool IsChecked { get; set; }
        public string Tooltip { get; set; }
        public bool IsMembership { get; set; }
    }

    public class ColumnGroup
    {
        public string Title { get; set; }
        public List<ColumnItem> Items { get; set; } = new List<ColumnItem>();
    }

    public class DisplayColumnsInput
    {
        public string Id { get; set; }
        public List<ColumnGroup> Groups { get; set; } = new List<ColumnGroup>();

        public bool ShowSearch { get; set; } = true;
        public string SearchPlaceholder { get; set; } = "Find a column...";

        public bool ShowSelectAll { get; set; } = true;
        public bool ShowGroupCounts { get; set; } = true;

        public bool ShowFooter { get; set; } = true;
        public string ApplyButtonText { get; set; } = "Apply";
        public string ResetButtonText { get; set; } = "Reset";
        public string SaveButtonText { get; set; }

        public int ColumnCount { get; set; } = 3;

        public string MembershipUrl { get; set; }
        public string MembershipTooltip { get; set; } = "Available with a membership.";
        public bool ShowMembershipLegend { get; set; } = true;
        public string MembershipLegendText { get; set; } = "Locked columns need a membership.";
        public string Message { get; set; }
    }
}