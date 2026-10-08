using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class SupportFormInput
    {
        public string Id { get; set; } = "support";
        public string SiteLabel { get; set; } = "RotoMonster";
        public string IntroText { get; set; } = "Tell us what's going on and we'll get back to you. You'll get a link to track your request and reply.";
        public List<string> Categories { get; set; } = SupportFormService.DefaultCategories();
        public string Name { get; set; }
        public string Email { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> FieldErrors { get; set; } = new Dictionary<string, string>();
        public string ErrorMessage { get; set; }
        public List<SupportTicketLink> MyTickets { get; set; } = new List<SupportTicketLink>();
        public string SubmitText { get; set; } = "Send request";
        public bool ShowSubject { get; set; } = false;
    }

    public class SupportTicketLink
    {
        public string TicketNumber { get; set; }
        public string Subject { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
