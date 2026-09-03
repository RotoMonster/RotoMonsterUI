using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class PlayerProfileService
    {
        public PlayerProfileResult Process(string id, Dictionary<string, string> params_)
        {
            return Process(id, params_, null);
        }

        public PlayerProfileResult Process(string id, Dictionary<string, string> params_,
            PlayerProfileInput input)
        {
            var result = new PlayerProfileResult();
            if (params_ == null) return result;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);
            if (eventTarget == null) eventTarget = "";

            var suffix = "_" + id;

            var commentsId = "ppcomments" + suffix;

            result.CommentsExpanded = Flag(commentsId + "-toggle", params_);
            result.CommentsPinned = Flag(commentsId + "-lock", params_);

            if (eventTarget == commentsId + "-pin-btn")
                result.CommentsPinned = !result.CommentsPinned;

            result.AddCommentPressed = Pressed("ppaddcomment" + suffix, params_, eventTarget);

            var trackedId = input != null && input.TrackedLeagues != null
                && !string.IsNullOrEmpty(input.TrackedLeagues.Id)
                    ? input.TrackedLeagues.Id
                    : "pptracked" + suffix;

            result.TrackedLeaguesExpanded = Flag(trackedId + "-toggle", params_);
            result.TrackedLeaguesPinned = Flag(trackedId + "-lock", params_);

            if (eventTarget == trackedId + "-pin-btn")
                result.TrackedLeaguesPinned = !result.TrackedLeaguesPinned;

            return result;
        }

        private static bool Flag(string key, Dictionary<string, string> params_)
        {
            string value;
            if (!params_.TryGetValue(key, out value)) return false;
            return value == "1";
        }

        private static bool Pressed(string key, Dictionary<string, string> params_, string eventTarget)
        {
            return eventTarget == key || params_.ContainsKey(key);
        }
    }
}