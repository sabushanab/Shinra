using Shinra.Clients.Models;

namespace Shinra.Messages.Character
{
    public class CharacterUpdated
    {
        public CharacterStatistics Statistics { get; private set; }
        public CharacterUpdated(CharacterStatistics statistics) 
        {
            Statistics = statistics;
        }
    }
}
