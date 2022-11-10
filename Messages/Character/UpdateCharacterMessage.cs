namespace Shinra.Messages.Character
{
    public class UpdateCharacterMessage
    {
        public string Realm { get; private set; }
        public string CharacterName { get; private set; }
        public UpdateCharacterMessage(string realm, string characterName) 
        {
            Realm = realm;
            CharacterName = characterName;
        }
    }
}
