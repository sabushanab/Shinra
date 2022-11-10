using System.Collections.Generic;
using System.Web;

namespace Shinra.Services.Models
{
    public class PointContainer
    {
        public string Realm { get; set; }
        public string CharacterName { get; set; }
        public int Level { get; set; }
        public string CharacterClass { get; set; }
        public double TotalPoints { get; set; }
        public List<PointCategory> Categories { get; set; } = new List<PointCategory>();
        public PointContainer(string realm, string characterName, int level, string characterClass) 
        {
            Realm = realm;
            CharacterName = characterName;
            Level = level;
            CharacterClass = characterClass;
        }
    }
}
