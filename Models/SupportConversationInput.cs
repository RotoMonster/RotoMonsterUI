using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum SupportTicketStatus
    {
        Open,
        WaitingOnCustomer,
        Closed
    }

    public class SupportConversationInput
    {
        public string Id { get; set; } = "supportticket";
        public string TicketNumber { get; set; }
        public string Subject { get; set; }
        public DateTime? OpenedDate { get; set; }
        public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;
        public string StaffName { get; set; } = "Support";
        public List<SupportConversationMessage> Messages { get; set; } = new List<SupportConversationMessage>();
        public bool LoadFailed { get; set; }
        public bool JustCreated { get; set; }
        public bool Emailed { get; set; }
        public bool JustReplied { get; set; }
        public string ErrorMessage { get; set; }
        public string ReplyText { get; set; }
        public string ReplyError { get; set; }
        public string NewRequestUrl { get; set; }
        public string SendText { get; set; } = "Send reply";
    }

    public class SupportConversationMessage
    {
        public bool FromCustomer { get; set; }
        public DateTime? Date { get; set; }
        public string Text { get; set; }
        public string SafeHtml { get; set; }
    }
}
