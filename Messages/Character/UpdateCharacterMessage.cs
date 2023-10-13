namespace Shinra.Messages.Character
{
    public class UpdateCharacterMessage
    {
        public string Realm { get; private set; }
        public string CharacterName { get; private set; }
        public int? Level { get; private set; }
        public string CharacterClass { get; private set; }
        public UpdateCharacterMessage(string realm, string characterName, int level, string characterClass) 
        {
            Realm = realm;
            CharacterName = characterName;
            Level = level;
            CharacterClass = characterClass;
        }
    }
}
