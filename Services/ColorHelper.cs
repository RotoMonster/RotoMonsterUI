using System;

namespace RotoMonsterUI
{
    public static class ColorHelper
    {
        public const string Black = "000000";
        public const string White = "FFFFFF";

        private static string GetColorCode(int r, int g, int b)
        {
            return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

        private static double GetPercentFloat(float value, float low, float high, bool colorHigh)
        {
            if (high == low) return 0;
            double percent = (double)(value - low) / (high - low) * 100;
            percent = Math.Max(0, Math.Min(100, percent));
            return colorHigh ? percent : 100 - percent;
        }

        // Float versions (real implementation)
        public static string GetYellowColorCode(float value, float low, float high, bool colorHigh)
        {
            double percent = GetPercentFloat(value, low, high, colorHigh);
            int otherColor = (int)Math.Round(255 - percent / 200 * 255, 0);
            return GetColorCode(255, 255, otherColor);
        }

        public static string GetRedColorCode(float value, float low, float high, bool colorHigh)
        {
            double percent = GetPercentFloat(value, low, high, colorHigh);
            int otherColor = (int)Math.Round(255 - percent / 200 * 255, 0);
            return GetColorCode(255, otherColor, otherColor);
        }

        public static string GetGreenColorCode(float value, float low, float high, bool colorHigh)
        {
            double percent = GetPercentFloat(value, low, high, colorHigh);
            int otherColor = (int)Math.Round(255 - percent / 200 * 255, 0);
            return GetColorCode(otherColor, 255, otherColor);
        }

        public static string GetCyanColorCode(float value, float low, float high, bool colorHigh)
        {
            double percent = GetPercentFloat(value, low, high, colorHigh);
            int otherColor = (int)Math.Round(255 - percent * 0.8, 0);
            return GetColorCode(otherColor, 255, 255);
        }

        public static string GetBlueColorCode(float value, float low, float high, bool colorHigh)
        {
            double percent = GetPercentFloat(value, low, high, colorHigh);
            int red = (int)Math.Round(255 - (percent / 100.0) * 255, 0);
            int green = (int)Math.Round(255 - (percent / 100.0) * 204, 0);
            int blue = (int)Math.Round(255 - (percent / 100.0) * 51, 0);
            return GetColorCode(red, green, blue);
        }

        // Int version calling through
        public static string GetBlueColorCode(int value, int low, int high, bool colorHigh)
            => GetBlueColorCode((float)value, (float)low, (float)high, colorHigh);

        // Int versions (call through to float)
        public static string GetYellowColorCode(int value, int low, int high, bool colorHigh)
            => GetYellowColorCode((float)value, (float)low, (float)high, colorHigh);

        public static string GetRedColorCode(int value, int low, int high, bool colorHigh)
            => GetRedColorCode((float)value, (float)low, (float)high, colorHigh);

        public static string GetGreenColorCode(int value, int low, int high, bool colorHigh)
            => GetGreenColorCode((float)value, (float)low, (float)high, colorHigh);

        public static string GetCyanColorCode(int value, int low, int high, bool colorHigh)
            => GetCyanColorCode((float)value, (float)low, (float)high, colorHigh);

        // Configurable "freshness window" in seconds — 900 = 15 minutes
        public static int AgeShadeMaxSeconds = 900;

        public static string AgeRingStyle(string color)
        {
            return $"box-shadow:inset 0 0 0 var(--card-age-border-width) {color}, inset 0 0 0 calc(var(--card-age-border-width) + 1px) var(--color-border-strong, var(--color-border));";
        }

        public static string GetAgeShadeHex(TimeSpan? age, int? maxSecondsOverride = null)
        {
            if (!age.HasValue) return null;

            int maxSeconds = maxSecondsOverride ?? AgeShadeMaxSeconds;
            double elapsed = age.Value.TotalSeconds;

            if (elapsed < 0) elapsed = 0;
            if (elapsed >= maxSeconds) return null; // past the window, no shading

            return "#" + GetYellowColorCode((float)elapsed, 0f, (float)maxSeconds, false);
        }

        // Darkens a color if it's too light to read against a bright yellow background (like the age-shading).
        // Colors that are already dark enough (navy, maroon, black, etc.) are returned unchanged.
        public static string GetAutoColorForLightBackground(string hexColor, double maxLuminance = 0.55)
        {
            if (string.IsNullOrEmpty(hexColor)) return hexColor;

            var hex = hexColor.TrimStart('#');
            if (hex.Length != 6) return hexColor;

            int r = Convert.ToInt32(hex.Substring(0, 2), 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);

            double luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255.0;

            if (luminance <= maxLuminance)
                return "#" + hex;

            double darkenFactor = maxLuminance / luminance;
            r = (int)Math.Round(r * darkenFactor);
            g = (int)Math.Round(g * darkenFactor);
            b = (int)Math.Round(b * darkenFactor);

            return "#" + GetColorCode(r, g, b);
        }
    }
}