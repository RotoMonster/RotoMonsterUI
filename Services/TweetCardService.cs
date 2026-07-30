using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class TweetCardService
    {

        private static readonly string[] ActionPrefixes =
        {
            "tweetpost_", "tweetsettag_", "tweetcancel_", "tweetautofill_", "tweetaitext_"
        };


        private static readonly string[] FieldPrefixes =
        {
            "tweetplayer_", "tweetteam_",
            "tweetstatus_", "tweettag_", "tweetnewstitle_", "tweetsource_",
            "tweetnewsdetails_", "tweetunofficial_", "tweetlevel_", "tweetnewstag_"
        };

        public static long? GetActiveTweetId(Dictionary<string, string> params_)
        {
            string eventTarget;
            if (params_.TryGetValue("__EVENTTARGET", out eventTarget) && eventTarget != null)
            {
                var fromTarget = MatchId(eventTarget, ActionPrefixes) ?? MatchId(eventTarget, FieldPrefixes);
                if (fromTarget.HasValue) return fromTarget;
            }

            foreach (var key in params_.Keys)
            {
                var id = MatchId(key, ActionPrefixes);
                if (id.HasValue) return id;
            }

            foreach (var key in params_.Keys)
            {
                var id = MatchId(key, FieldPrefixes);
                if (id.HasValue) return id;
            }

            return null;
        }

        private static long? MatchId(string key, string[] prefixes)
        {
            foreach (var prefix in prefixes)
            {
                if (!key.StartsWith(prefix)) continue;
                long id;
                if (long.TryParse(TrimToId(key.Substring(prefix.Length)), out id))
                    return id;
            }
            return null;
        }

        private static string TrimToId(string rest)
        {
            if (string.IsNullOrEmpty(rest)) return rest;
            var underscore = rest.IndexOf('_');
            return underscore < 0 ? rest : rest.Substring(0, underscore);
        }

        public TweetCardResult Process(Dictionary<string, string> params_)
        {
            var tweetId = GetActiveTweetId(params_);
            return tweetId.HasValue ? Process(tweetId.Value, params_) : new TweetCardResult();
        }

        public TweetCardResult Process(long tweetId, Dictionary<string, string> params_)
        {
            var result = new TweetCardResult();
            result.TweetId = tweetId;

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);

            var postKey = "tweetpost_" + tweetId;
            if (params_.ContainsKey(postKey) || eventTarget == postKey)
                result.PostPressed = true;

            var setTagKey = "tweetsettag_" + tweetId;
            if (params_.ContainsKey(setTagKey) || eventTarget == setTagKey)
                result.SetTagPressed = true;

            var cancelKey = "tweetcancel_" + tweetId;
            if (params_.ContainsKey(cancelKey) || eventTarget == cancelKey)
                result.CancelPressed = true;

            
            var autoFillKey = "tweetautofill_" + tweetId;
            if (params_.ContainsKey(autoFillKey) || eventTarget == autoFillKey)
                result.AutoFillPressed = true;

            var aiTextKey = "tweetaitext_" + tweetId;
            if (params_.ContainsKey(aiTextKey) || eventTarget == aiTextKey)
            {
                result.AiTextPressed = true;
                result.AutoFillPressed = true;
            }

            // Player selection. Empty value means the user cleared it, which is still a change.
            var playerKey = "tweetplayer_" + tweetId;

            string selectedPlayer;
            if (params_.TryGetValue(playerKey, out selectedPlayer))
            {
                int playerId;
                if (int.TryParse(selectedPlayer, out playerId))
                    result.SelectedPlayerId = playerId;
            }

            if (eventTarget == playerKey)
                result.PlayerSelectionChanged = true;

            if (result.CancelPressed)
            {
                result.PlayerSelectionChanged = true;
                result.SelectedPlayerId = null;
            }

            string selectedTeam;
            if (params_.TryGetValue("tweetteam_" + tweetId, out selectedTeam))
                result.SelectedTeamCode = selectedTeam;

            string status;
            if (params_.TryGetValue("tweetstatus_" + tweetId, out status))
                result.StatusTypeText = status;

            string tag;
            if (params_.TryGetValue("tweettag_" + tweetId, out tag))
                result.StatusTypeTag = tag;

            string title;
            if (params_.TryGetValue("tweetnewstitle_" + tweetId, out title))
                result.NewsTitle = title;

            string source;
            if (params_.TryGetValue("tweetsource_" + tweetId, out source))
                result.SourceURL = source;

            string details;
            if (params_.TryGetValue("tweetnewsdetails_" + tweetId, out details))
                result.NewsDetails = details;

            result.IsUnofficial = params_.ContainsKey("tweetunofficial_" + tweetId);

            string level;
            NewsLevel parsedLevel;
            if (params_.TryGetValue("tweetlevel_" + tweetId, out level) &&
                System.Enum.TryParse<NewsLevel>(level, true, out parsedLevel))
            {
                result.NewsLevel = parsedLevel;
            }

            var tagPrefix = "tweetnewstag_" + tweetId + "_";
            result.CheckedNewsTagIds = params_.Keys
                .Where(k => k.StartsWith(tagPrefix))
                .Select(k =>
                {
                    int id;
                    return int.TryParse(k.Substring(tagPrefix.Length), out id) ? id : -1;
                })
                .Where(id => id >= 0)
                .ToList();

            return result;
        }
    }
}