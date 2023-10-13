using System.Collections.Generic;

namespace Shinra.Clients.Models
{
    public class CharacterMythicPlusSeasonDetails
    {
        public CurrentMythicRating mythic_rating { get; set; }
        public List<BestMythicPlusRun> best_runs { get; set; }
    }

    public class BestMythicPlusRun
    {
        public long completed_timestamp { get; set; }
        public int duration { get; set; }
        public int keystone_level { get; set; }
        public Dungeon dungeon { get; set; }
    }

    public class Dungeon
    {
        public string name { get; set; }
    }
}
