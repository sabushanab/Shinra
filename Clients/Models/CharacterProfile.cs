namespace Shinra.Clients.Models
{
    public class CharacterProfile
    {
        public string name { get; set; }
        public CharacterClass character_class { get; set; }
        public int level { get; set; }
    }

    public class CharacterClass
    {
        public string name { get; set; }
    }
}
