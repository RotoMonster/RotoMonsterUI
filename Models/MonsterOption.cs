using System.Collections.Generic;

namespace RotoMonsterUI
{
    /// <summary>
    /// One choice in a dropdown or a button group. Shared rather than each
    /// component declaring its own, so a page can build a list once and pass
    /// it to whichever components need it.
    /// </summary>
    public class MonsterOption
    {
        public string Value { get; set; }
        public string Text { get; set; }

        public MonsterOption() { }

        public MonsterOption(string value, string text)
        {
            Value = value;
            Text = text;
        }
    }

    /// <summary>
    /// A scoring category that can be punted, with an optional weight. The
    /// weight is a string so a half-typed value survives a postback.
    /// </summary>
    public class MonsterPuntCategory
    {
        public string CategoryId { get; set; }
        public string Abbreviation { get; set; }
        public bool IsSelected { get; set; }
        public string Weight { get; set; }
    }

    /// <summary>
    /// A position in a filter. ColorCSS goes straight into a css custom
    /// property, so it needs to be a usable value - "var(--pos-pg)" or a hex,
    /// not a bare variable name.
    /// </summary>
    public class MonsterPosition
    {
        public string PositionId { get; set; }
        public string Abbreviation { get; set; }
        public string ColorCSS { get; set; }
        public bool IsSelected { get; set; }
    }
}