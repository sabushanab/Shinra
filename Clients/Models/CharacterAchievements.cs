using System.Collections.Generic;

namespace Shinra.Clients.Models
{
    public class CharacterAchievements
    {
        public List<Achievement> achievements { get; set; }
    }

    public class Achievement
    {
        public int id { get; set; }
        public long completed_timestamp { get; set; }
        public AchievementInfo achievement { get; set; }
    }

    public class AchievementInfo
    {
        public string name { get; set; }
    }
}
