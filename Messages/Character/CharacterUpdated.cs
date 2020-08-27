namespace Shinra.Messages.Character
{
    public class CharacterUpdated
    {
        public CharacterContainer Character { get; private set; }
        public CharacterUpdated(CharacterContainer character) 
        {
            Character = character;
        }
    }
}
