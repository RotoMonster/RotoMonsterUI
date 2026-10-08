using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RotoMonsterUI
{
    public class SupportFormService
    {
        private static readonly Regex EmailPattern = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public static List<string> DefaultCategories()
        {
            return new List<string>
            {
                "Account or login",
                "Membership or billing",
                "League import",
                "Rankings or data",
                "Something is broken",
                "Other"
            };
        }

        public SupportFormResult Process(string id, Dictionary<string, string> params_, IEnumerable<string> categories = null)
        {
            var result = new SupportFormResult();
            if (params_ == null || !params_.ContainsKey(id + "_submit")) return result;

            result.SubmitPressed = true;
            result.Name = Read(params_, id + "_name");
            result.Email = Read(params_, id + "_email");
            result.Category = Read(params_, id + "_category");
            result.Subject = Read(params_, id + "_subject");
            result.Description = Read(params_, id + "_description");

            if (!string.IsNullOrEmpty(Read(params_, id + "_website")))
            {
                result.IsSpam = true;
                return result;
            }

            var allowed = (categories ?? DefaultCategories()).ToList();

            if (result.Name.Length == 0) result.Errors["name"] = "Enter your name.";
            else if (result.Name.Length > 80) result.Errors["name"] = "Keep your name under 80 characters.";

            if (result.Email.Length == 0) result.Errors["email"] = "Enter your email.";
            else if (result.Email.Length > 256 || !EmailPattern.IsMatch(result.Email)) result.Errors["email"] = "Enter a valid email address.";

            if (!allowed.Contains(result.Category)) result.Errors["category"] = "Pick a category.";

            var subjectShown = params_.ContainsKey(id + "_subject");
            if (subjectShown)
            {
                if (result.Subject.Length == 0) result.Errors["subject"] = "Enter a subject.";
                else if (result.Subject.Length > 150) result.Errors["subject"] = "Keep the subject under 150 characters.";
            }
            else
            {
                result.Subject = BuildSubject(result.Category, result.Description);
            }

            if (result.Description.Length < 10) result.Errors["description"] = "Tell us a little more (at least 10 characters).";
            else if (result.Description.Length > 5000) result.Errors["description"] = "Keep the details under 5000 characters.";

            result.IsValid = result.Errors.Count == 0;
            return result;
        }

        public static string BuildSubject(string category, string description)
        {
            var text = Regex.Replace(description ?? "", @"\s+", " ").Trim();
            if (text.Length > 80)
            {
                var cut = text.LastIndexOf(' ', 80);
                text = (cut > 40 ? text.Substring(0, cut) : text.Substring(0, 80)).TrimEnd(',', '.', ';', ':', ' ') + "…";
            }
            var subject = string.IsNullOrEmpty(category) ? text : category + ": " + text;
            return subject.Length > 150 ? subject.Substring(0, 150) : subject;
        }

        private static string Read(Dictionary<string, string> params_, string key)
        {
            return params_.TryGetValue(key, out var value) ? (value ?? "").Trim() : "";
        }
    }
}
