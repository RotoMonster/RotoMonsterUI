using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class SupportFormResult
    {
        public bool SubmitPressed { get; set; }
        public bool IsSpam { get; set; }
        public bool IsValid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}
