namespace Shinra.Messages.Character
{
    public class UpdateCharacterMessage
    {
        public int CharacterID { get; private set; }
        public UpdateCharacterMessage(int characterID) 
        {
            CharacterID = characterID;
        }
    }
}
