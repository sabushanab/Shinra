using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace Shinra.Services.Models
{
    public class PointContainer
    {
        public string _id {  get
            {
                return $"{Region}-{CharacterName.ToLower()}-{Realm.ToLower()}";
            } 
        }
        public bool _notAdded { get; set; }
        public string Region { get; set; }
        public string Realm { get; set; }
        public bool Boosted { get; set; }
        public bool NewCharacter { get; set; }
        public string CharacterName { get; set; }
        public int Level { get; set; }
        public string CharacterClass { get; set; }
        public double TotalPoints { get; set; }
        public double MythicPlusScore { get; set; }
        public bool HasDied { get; set; }
        public DateTime LastUpdated { get; set; }

        public List<PointCategory> Categories { get; set; } = new List<PointCategory>();
        public PointContainer() { }
        public PointContainer(string region, string realm, string characterName, int level, string characterClass) 
        {
            Region = region;
            Realm = realm;
            CharacterName = characterName;
            Level = level;
            CharacterClass = characterClass;
        }
    }
}
