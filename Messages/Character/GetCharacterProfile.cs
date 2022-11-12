using Shinra.Clients.Models;

namespace Shinra.Messages.Character
{
    public class GetCharacterProfile
    {
        public CharacterStatistics Statistics { get; private set; }
        public CharacterProfile Profile { get; private set; }
        public GetCharacterProfile(CharacterStatistics statistics, CharacterProfile profile)
        {
            Statistics = statistics;
            Profile = profile;
        }
    }
}
