using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class RotatingPanelSlide
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Attribution { get; set; }
    }

    public class RotatingPanelInput
    {
        public string Id { get; set; }
        public string Heading { get; set; }
        public IconType? Icon { get; set; }
        public string AccentColorCSS { get; set; } = "#185FA5";
        public bool UseQuoteStyle { get; set; }
        public int AutoAdvanceSeconds { get; set; } = 0;
        public int MinHeight { get; set; } = 132;
        public List<RotatingPanelSlide> Slides { get; set; } = new List<RotatingPanelSlide>();
    }
}