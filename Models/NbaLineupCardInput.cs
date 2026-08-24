using System.Collections.Generic;

namespace RotoMonsterUI
{
    public class NbaLineupCardInput
    {
        public string Id { get; set; }

        public NbaLineupTeamInput AwayTeam { get; set; }
        public NbaLineupTeamInput HomeTeam { get; set; }

        public string TipTime { get; set; }


        public string OddsLine { get; set; }

        public bool ShowProjectedMinutes { get; set; } = true;

        public bool ShowBench { get; set; } = false;

        public string BenchLabel { get; set; } = "Bench";


        public bool BenchCollapsed { get; set; } = false;

        public bool IsGameFinished { get; set; }
    }

    public class NbaLineupTeamInput
    {
        public string TeamCode { get; set; }

        public float? ProjectedPoints { get; set; }

        public bool IsVerified { get; set; }

        public bool IsBackToBack { get; set; }

        public List<NbaLineupPlayer> Players { get; set; } = new List<NbaLineupPlayer>();

        public List<NbaLineupPlayer> BenchPlayers { get; set; } = new List<NbaLineupPlayer>();
    }

    public class NbaLineupPlayer
    {

        public string Slot { get; set; }

        public DisplayPlayerInput Player { get; set; }

        public double? ProjectedMinutes { get; set; }

        public bool IsOwned { get; set; }

        public InjuryBadgeInput InjuryBadge { get; set; }
    }
}