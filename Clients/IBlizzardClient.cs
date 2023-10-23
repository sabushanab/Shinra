using Shinra.Clients.Models;
using System.Threading.Tasks;

namespace Shinra.Clients
{
    public interface IBlizzardClient
    {
        Task<RealmContainer> GetRealms(string region);
        Task<CharacterStatistics> GetCharacterStatistics(string region, string realm, string characterName);
        Task<CharacterAchievements> GetCharacterAchievements(string region, string realm, string characterName);
        Task<CharacterMythicPlusScore> GetMythicPlusScore(string region, string realm, string characterName);
        Task<CharacterMythicPlusSeasonDetails> GetMythicPlusSeasonDetails(string region, string realm, string characterName);
        
        Task<CharacterProfile> GetCharacterProfile(string region, string realm, string characterName);
    }
}
