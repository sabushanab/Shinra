using System.Net.Http;
using System.Threading.Tasks;

namespace Shinra.Clients
{
    public interface IXIVAPIClient
    {
        Task<FreeCompanyMembersContainer> GetFreeCompanyMembers();                
        Task GetEachFreeCompanyMember();
        Task<CharacterContainer> GetCharacter(int id);
    }
}