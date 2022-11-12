using LiteDB;
using System;
using System.Collections.Generic;

namespace Shinra.Services.Models
{
    public class PointContainer
    {
        public string _id {  get
            {
                return $"{CharacterName}-{Realm}";
            } 
        }
        [BsonIgnore]
        public bool _notAdded { get; set; }
        public string Realm { get; set; }
        public string CharacterName { get; set; }
        public int Level { get; set; }
        public string CharacterClass { get; set; }
        public double TotalPoints { get; set; }
        public bool HasDied { get; set; }
        public DateTime LastUpdated { get; set; }

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
