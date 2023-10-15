using Shinra.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shinra.Services
{
    public interface IBlizzardDataAccess
    {
        Task<LeaderboardResult> GetAllCharacterPoints(int page, int pageSize = 50);
        Task<PointContainer> SaveCharacterPoints(PointContainer container);
        Task UpdateAllCharacterPoints();
    }
}
