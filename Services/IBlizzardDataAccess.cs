using Shinra.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shinra.Services
{
    public interface IBlizzardDataAccess
    {
        Task<List<PointContainer>> GetAllCharacterPoints();
        Task<PointContainer> SaveCharacterPoints(PointContainer container);
        Task UpdateAllCharacterPoints();
    }
}
