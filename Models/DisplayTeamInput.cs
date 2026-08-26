namespace RotoMonsterUI
{
    public class DisplayTeamInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }

        /// <summary>Team abbreviation. Lets ColorCode be left blank and filled from the team palette.</summary>
        public string TeamCode { get; set; }
    }
}