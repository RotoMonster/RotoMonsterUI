using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class ScheduleGridResult
    {
        public ScheduleGridSortBy? SortBy { get; set; }
        public ScheduleGridColorType? ColorType { get; set; }
        public int? StartSelectedPeriod { get; set; }
        public int? EndSelectedPeriod { get; set; }
        public string EasePositionFilterValue { get; set; }
        public bool? ShowQualityGames { get; set; }
        public int? ExpandedPeriodNumber { get; set; }
        public bool IsCollapseRequested { get; set; }
    }

    public class ScheduleGridService
    {
        public ScheduleGridResult Process(string controlId, Dictionary<string, string> formValues)
        {
            var result = new ScheduleGridResult();

            if (formValues.TryGetValue($"{controlId}-sortby", out var sortBy)
                && System.Enum.TryParse<ScheduleGridSortBy>(sortBy, true, out var parsedSortBy))
            {
                result.SortBy = parsedSortBy;
            }

            if (formValues.TryGetValue($"{controlId}-colortype", out var colorType)
                && System.Enum.TryParse<ScheduleGridColorType>(colorType, true, out var parsedColorType))
            {
                result.ColorType = parsedColorType;
            }

            // Baseline: carry forward whatever start/end were already selected
            if (formValues.TryGetValue($"{controlId}-current-start", out var curStartStr) && int.TryParse(curStartStr, out var curStart))
                result.StartSelectedPeriod = curStart;

            if (formValues.TryGetValue($"{controlId}-current-end", out var curEndStr) && int.TryParse(curEndStr, out var curEnd))
                result.EndSelectedPeriod = curEnd;

            // Up/down arrows on a period row override just the one that was clicked
            foreach (var kvp in formValues)
            {
                if (kvp.Key == $"{controlId}-set-start" && int.TryParse(kvp.Value, out var startPeriod))
                    result.StartSelectedPeriod = startPeriod;

                if (kvp.Key == $"{controlId}-set-end" && int.TryParse(kvp.Value, out var endPeriod))
                    result.EndSelectedPeriod = endPeriod;
            }

            // The toggle only submits a value when checked, so the hidden marker tells us it was rendered
            if (formValues.ContainsKey($"{controlId}-quality-present"))
                result.ShowQualityGames = formValues.ContainsKey($"{controlId}-showquality");

            if (formValues.TryGetValue($"{controlId}-ease-position", out var easePosition))
                result.EasePositionFilterValue = easePosition;

            if (formValues.TryGetValue($"{controlId}-expand", out var expandValue))
            {
                if (int.TryParse(expandValue, out var expandPeriod))
                    result.ExpandedPeriodNumber = expandPeriod;
                else
                    result.IsCollapseRequested = true;
            }

            return result;
        }
    }
}