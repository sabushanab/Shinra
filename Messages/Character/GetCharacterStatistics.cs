using Shinra.Clients.Models;

namespace Shinra.Messages.Character
{
    public class GetCharacterStatistics
    {
        public CharacterStatistics Statistics { get; private set; }
        public GetCharacterStatistics(CharacterStatistics statistics)
        {
            Statistics = statistics;
        }
    }
}
