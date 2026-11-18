using System;
using HtmlTags;

namespace RotoMonsterUI
{
    public class TimeSince
    {
        private TimeSpan _timeSpan;

        public TimeSince(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        public string Render()
        {
            string display;

            if (_timeSpan.TotalSeconds < 60)
                display = $"{(int)_timeSpan.TotalSeconds}s";
            else if (_timeSpan.TotalMinutes < 60)
                display = $"{(int)_timeSpan.TotalMinutes}m";
            else if (_timeSpan.TotalHours < 24)
                display = $"{Math.Round(_timeSpan.TotalHours, 1)}h";
            else if (_timeSpan.TotalDays <= 30)
                display = $"{Math.Round(_timeSpan.TotalDays, 1)}d";
            else
            {
                var when = DateTime.Now.Subtract(_timeSpan);
                display = new DisplayDate(new DisplayDateInput { Date = when, Format = when.Year == DateTime.Now.Year ? "M/d" : "M/d/yy" }).Render();
            }

            var tag = new HtmlTag("span")
                .AddClass("time-since")
                .Text(display);

            return tag.ToString();
        }
    }
}