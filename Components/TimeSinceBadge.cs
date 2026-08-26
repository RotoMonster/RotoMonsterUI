using System;
using HtmlTags;

namespace RotoMonsterUI
{
    public class TimeSinceBadge
    {
        private readonly TimeSpan _timeSpan;

        public TimeSinceBadge(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        public string Render()
        {
            // Age is conveyed by the card border now, so the badge no longer shades.
            var badge = new HtmlTag("span").AddClass("time-since-badge");
            badge.AppendHtml(new TimeSince(_timeSpan).Render());
            return badge.ToString();
        }
    }
}