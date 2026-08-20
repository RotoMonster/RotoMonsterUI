namespace RotoMonsterUI
{
    public class SportradarAttributionInput
    {
        public string LinkUrl { get; set; } = "https://sportradar.com/";

        public string LogoUrl { get; set; } =
            "https://api-docs.sportradar.us/logo/powered-by-sportradar-300w.png";

        public string DarkLogoUrl { get; set; } =
            "https://api-docs.sportradar.us/logo/powered-by-sportradar-reversed-300w.png";

        public int SourceWidth { get; set; } = 300;

        public int LogoHeight { get; set; } = 16;
        public string Text { get; set; } = "";

        public string AltText { get; set; } = "Powered by Sportradar";
    }
}