using System.Collections.Generic;

namespace RotoMonsterUI
{
    public enum LeagueScoringType
    {
        Roto,
        H2H
    }

    public class LeagueSettingsCategory
    {
        public string Abbreviation { get; set; }
        public bool IsActive { get; set; } = true;
        public double PointsPerStat { get; set; }
    }

    public class LeagueSettingsInput
    {
        public LeagueScoringType ScoringType { get; set; } = LeagueScoringType.Roto;
        public int NumTeams { get; set; }
        public int PlayersPerTeam { get; set; }
        public List<LeagueSettingsCategory> Categories { get; set; } = new List<LeagueSettingsCategory>();
    }
}