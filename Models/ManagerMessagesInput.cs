using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class ManagerMessage
    {
        public int MessageId { get; set; }
        public string SubjectHtml { get; set; }
        public string MessageHtml { get; set; }
    }

    public class ManagerMessagesInput
    {
        public string Id { get; set; } = "managerMessages";
        public List<ManagerMessage> Messages { get; set; } = new List<ManagerMessage>();
        public bool ShowDismiss { get; set; }
        public string ButtonTextFormat { get; set; } = "{0} Manager Messages";
        public bool IsExpanded { get; set; }
    }
}