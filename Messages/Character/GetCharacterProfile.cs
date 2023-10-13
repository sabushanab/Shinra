using Shinra.Clients.Models;

namespace Shinra.Messages.Character
{
    public class GetCharacterProfile
    {
        public CharacterProfile Profile { get; private set; }
        public GetCharacterProfile(CharacterProfile profile)
        {
            Profile = profile;
        }
    }
}
