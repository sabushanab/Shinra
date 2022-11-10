using Shinra.Clients.Models;
using System.Threading.Tasks;

namespace Shinra.Clients
{
    public interface IBlizzardClient
    {
        Task<CharacterStatistics> GetCharacterStatistics(string realm, string characterName);
    }
}
