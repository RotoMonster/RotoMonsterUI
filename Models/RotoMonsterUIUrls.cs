using System;

namespace RotoMonsterUI
{
    /// <summary>
    /// Where the components link to. Defaults are Basketball Monster's urls,
    /// so BM needs no changes. Other sites override these once at startup:
    ///
    ///     RotoMonsterUIUrls.PlayerUrl = id => $"/Players?playerId={id}";
    /// </summary>
    public static class RotoMonsterUIUrls
    {
        public static Func<int, string> PlayerUrl { get; set; }
            = id => "/playerInfo.aspx?i=" + id;

        public static Func<int, string> PlayerCommentsUrl { get; set; }
            = id => "/usercomments.aspx?i=" + id;

        public static string UserSettingsUrl { get; set; }
            = "/usersettings.aspx";
    }
}
