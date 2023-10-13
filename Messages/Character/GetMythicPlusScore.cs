using Shinra.Clients.Models;

namespace Shinra.Messages.Character
{
    public class GetMythicPlusScore
    {
        public CharacterProfile Profile { get; set; }
        public CharacterStatistics Statistics { get; private set; }
        public CharacterMythicPlusSeasonDetails MythicScore { get; private set; }
        public GetMythicPlusScore(CharacterMythicPlusSeasonDetails mythicScore, CharacterStatistics statistics, CharacterProfile profile)
        {
            MythicScore = mythicScore;
            Statistics = statistics;
            Profile = profile;
        }
    }
}
