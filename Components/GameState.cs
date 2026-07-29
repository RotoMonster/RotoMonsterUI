using HtmlTags;
using System;

namespace RotoMonsterUI
{
    public class GameState
    {
        private readonly GameStateInput _input;

        public GameState(GameStateInput input)
        {
            _input = input;
        }

        private string GetPostponementColor(string chance)
        {
            if (string.IsNullOrEmpty(chance)) return null;
            switch (chance.ToLower())
            {
                case "medium": return "#FBBF24";
                case "high": return "#FB7185";
                case "confirmed": return "#dc2626";
                default: return null;
            }
        }

        private double GetBasketballElapsedFraction()
        {
            var totalMinutes = _input.TotalQuarters * _input.QuarterLengthMinutes;
            if (totalMinutes <= 0) return 0;
            var elapsedMinutes = (_input.CurrentQuarter - 1) * _input.QuarterLengthMinutes
                + (_input.QuarterLengthMinutes - _input.QuarterMinutesRemaining);
            return Math.Max(0, Math.Min(1, elapsedMinutes / totalMinutes));
        }

        public string Render()
        {
            var wrapper = new HtmlTag("div").AddClass("game-state-wrapper");

            if (_input.IsGameLive && !_input.IsGameFinished)
            {
                double progressPercent = _input.Sport == GameSport.Basketball
                    ? Math.Min(100, GetBasketballElapsedFraction() * 100)
                    : Math.Min(100, _input.CurrentOuts / 54.0 * 100);
                wrapper.Attr("style", $"background: linear-gradient(to right, rgba(128,128,128,0.35) {progressPercent:0.0}%, transparent {progressPercent:0.0}%); border-radius: 6px; padding: 3px 6px;");
            }

            if (_input.IsGameFinished)
            {
                var final = new HtmlTag("div").AddClass("game-state-final");
                final.Text("Final");
                wrapper.Append(final);
            }
            else if (_input.IsGameLive && _input.Sport == GameSport.Basketball)
            {
                var quarterLabel = _input.IsOvertime ? "OT" : OrdinalHelper.GetOrdinal(_input.CurrentQuarter);
                var minutes = (int)_input.QuarterMinutesRemaining;
                var seconds = (int)Math.Round((_input.QuarterMinutesRemaining - minutes) * 60);
                var clockLabel = $"{minutes}:{seconds:00}";

                var quarterClock = new HtmlTag("div").AddClass("game-state-quarter-clock");
                quarterClock.Append(new HtmlTag("span").AddClass("game-state-quarter-label").Text(quarterLabel));
                quarterClock.Append(new HtmlTag("span").AddClass("game-state-clock-label").Text(clockLabel));
                wrapper.Append(quarterClock);
            }
            else if (_input.IsGameLive)
            {
                int inning = _input.CurrentOuts / 6 + 1;
                int halfInningPos = _input.CurrentOuts % 6;
                bool isBottom = halfInningPos >= 3;
                int topOuts = isBottom ? 3 : halfInningPos;
                int bottomOuts = isBottom ? halfInningPos - 3 : 0;
                string inningLabel = OrdinalHelper.GetOrdinal(inning);

                var outsWrapper = new HtmlTag("div").AddClass("game-state-outs");

                var topHalf = new HtmlTag("div").AddClass("game-state-outs-half");
                for (int i = 0; i < 3; i++)
                {
                    var circle = new HtmlTag("span").AddClass(i < topOuts ? "out-circle out-circle--filled" : "out-circle");
                    topHalf.Append(circle);
                }

                var inningTag = new HtmlTag("span").AddClass("game-state-inning-label").Text(inningLabel);

                var bottomHalf = new HtmlTag("div").AddClass("game-state-outs-half");
                for (int i = 0; i < 3; i++)
                {
                    var circle = new HtmlTag("span").AddClass(i < bottomOuts ? "out-circle out-circle--filled" : "out-circle");
                    bottomHalf.Append(circle);
                }

                outsWrapper.Append(topHalf);
                outsWrapper.Append(inningTag);
                outsWrapper.Append(bottomHalf);
                wrapper.Append(outsWrapper);
            }
            else
            {
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(_input.GameTimeUtc, _input.DisplayTimezone);
                var timeStr = localTime.ToString("h:mmtt").ToLower();
                var until = _input.GameTimeUtc - DateTime.UtcNow;
                var untilStr = until.TotalMinutes < 0
                    ? ""
                    : until.TotalHours >= 1
                        ? $"in {Math.Round(until.TotalHours, 1)}h"
                        : $"in {(int)until.TotalMinutes}m";

                var upcoming = new HtmlTag("div").AddClass("game-state-upcoming");

                if (until.TotalHours <= 4 && until.TotalHours >= 0)
                {
                    var yellowPercent = (float)(1.0 - until.TotalHours / 4.0);
                    var bgColor = ColorHelper.GetYellowColorCode(yellowPercent * 100f, 0f, 100f, true);
                    upcoming.Attr("style", $"background-color:#{bgColor}; border: 1px solid #d1d5db; text-align:center; border-radius:4px; padding: 0.25rem 0.5rem;");
                }
                else
                {
                    upcoming.Attr("style", "border: 1px solid #d1d5db; text-align:center; border-radius:4px; padding: 0.25rem 0.5rem; color: inherit;");
                }

                upcoming.Text($"{timeStr} {untilStr}");

                if (_input.Weather != null)
                {
                    var postponeColor = GetPostponementColor(_input.Weather?.PostponementFactor);
                    if (postponeColor != null)
                    {
                        var postponeIcon = new Icon(new IconInput { Type = IconType.PostponementChanceWarning, Color = postponeColor, Fill = postponeColor, Size = 18 }).Render();
                        var postponeLabel = string.IsNullOrEmpty(_input.Weather.PostponementFactor) ? ""
                            : char.ToUpper(_input.Weather.PostponementFactor[0]) + _input.Weather.PostponementFactor.Substring(1).ToLower();
                        upcoming.AppendHtml("&nbsp;");
                        var postponeTooltip = !string.IsNullOrEmpty(_input.Weather.PostponementReason)
                            ? $"Postponement: {postponeLabel} — {_input.Weather.PostponementReason}"
                            : $"Postponement: {postponeLabel}";
                        upcoming.AppendHtml(new CustomTooltip(postponeIcon, postponeTooltip).Render());
                    }
                }

                wrapper.Append(upcoming);
            }

            return wrapper.ToString();
        }
    }
}