namespace RotoMonsterUI
{
    public enum IconType
    {
        Settings,
        RefreshRosters,
        PostponementChanceWarning,
        Info,
        MainValue,
        Dome,
        RetractableDome,
        Rain,
        Trash,
        Next,
        Previous,
        LineupConfirmed,
        LineupNotConfirmed,
        Weather,
        LineupCard,
        ExportCSV,
        ExportExcel,
        Save,
        Calendar,
        PersonSimple,
        PersonAlert,
        PersonConfirmed,
        PersonArmsDown,
        PersonArmsUp,
        Injury,
        Illness,
        Rest,
        Personal,
        CoachsDecision,
        Dental,
        PossibleSuspension,
        Other,
        TradePending,
        Contract,
        InjuryMaintenance,
        Warning,
        Error,
        GameTimeDecision,
        Practiced,
        MissedPractice,
        MadeShootaround,
        MissedShootaround,
        Injured,
        Suspended,
        OutForSeason,
        NewContract,
        FreeAgent,
        Note,
        NewTeam,
        BreakoutCandidate,
        BustCandidate,
        PositionBattle,
        TwoWayPlayer,
        NoBackToBack,
        TankCandidate,
        Sleeper,
        ManifestoArticlePlayer,
        WaiverWire,
        SpotStart,
        LimitedMinutes,
        IsTeamUpdate,
        Edit,
        Kebab,
        ExternalLink,
        Close,
        ChatBubble,
        ArrowUp,
        ArrowDown,
        Filter,
        UnofficialTag,
        Success,
        Plus,
        Moon,
        Sun,
        Ai,
        Grok,
        Gemini,
        Favorite,
        FavoriteOutline,
        DragHandle,
        Basketball,
        Robot,
        WandMagic,
        Lock,
        Envelope,
        Twitter,
        UserGear,
        Box,
        Newspaper,
        Baseball,
        Football,
        HockeyPuck,
        PersonMinus,
        Verified,

        ChromeExtension,

        // Lineup state. Play for a confirmed starter, pause for a benched
        // player, play with a question badge for a probable one.
        Play,
        Pause,
        PlayQuestion,
        LockOpen, 
        Question,

        // Megaphone. Marks a message from the site managers.
        ManagerMessage

    }

    public class IconInput
    {
        public IconType Type { get; set; }
        public int Size { get; set; } = 20;
        public string Color { get; set; } = "currentColor";
        public string Fill { get; set; } = "none";

        /// <summary>
        /// Only used by PlayQuestion. That icon's mark has to contrast with
        /// the play shape rather than match it, so it cannot take Color.
        /// Leave empty for the default amber.
        /// </summary>
        public string QuestionColor { get; set; } = "";
    }
}