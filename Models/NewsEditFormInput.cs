using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class NewsEditFormButton
    {
        public string Text { get; set; }

        public ButtonStyle Style { get; set; } = ButtonStyle.Primary;

        public string Name { get; set; }
    }

    public class NewsEditFormInput
    {
        public string KeyPrefix { get; set; } = "";

        public string KeyId { get; set; }

        public string LeadingHtml { get; set; }

        public List<NewsEditFormButton> Buttons { get; set; } = new List<NewsEditFormButton>();

        public string StatusTypeText { get; set; }

        public string StatusTypeTag { get; set; }

        public string NewsTitle { get; set; }

        public string SourceURL { get; set; }

        public string NewsDetails { get; set; }

        public bool IsUnofficial { get; set; }

        public NewsLevel NewsLevel { get; set; } = NewsLevel.Low;

        public List<string> StatusTypeOptions { get; set; } = new List<string>();

        public List<string> StatusTypeTagOptions { get; set; } = new List<string>();

        public List<NewsTagOption> AvailableNewsTags { get; set; } = new List<NewsTagOption>();

        public List<NewsTagOption> NewsTags { get; set; } = new List<NewsTagOption>();
    }
}