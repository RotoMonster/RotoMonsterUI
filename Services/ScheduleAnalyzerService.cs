using System;
using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class ScheduleAnalyzerResult
    {
        public string SelectedRangeKey { get; set; }
        public string SelectedTeamValue { get; set; }
        public bool IsAnalyzeRequested { get; set; }
    }

    public class ScheduleAnalyzerService
    {
        public ScheduleAnalyzerResult Process(string controlId, Dictionary<string, string> formValues)
        {
            var result = new ScheduleAnalyzerResult();

            if (formValues == null) return result;

            if (formValues.TryGetValue("sarange_" + controlId, out var currentRange))
                result.SelectedRangeKey = currentRange;

            if (formValues.TryGetValue("sateam_" + controlId, out var team))
                result.SelectedTeamValue = team;

            if (formValues.ContainsKey("saanalyze_" + controlId))
                result.IsAnalyzeRequested = true;

            var rangePrefix = "sarangepick_" + controlId + "_";

            foreach (var key in formValues.Keys)
            {
                if (key.StartsWith(rangePrefix, StringComparison.Ordinal))
                    result.SelectedRangeKey = key.Substring(rangePrefix.Length);
            }

            return result;
        }
    }
}