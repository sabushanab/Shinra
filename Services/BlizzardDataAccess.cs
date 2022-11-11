using Akka.Actor;
using LiteDB.Async;
using Microsoft.Extensions.Configuration;
using Shinra.Actors;
using Shinra.Messages.Character;
using Shinra.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shinra.Services
{
    public class BlizzardDataAccess : IBlizzardDataAccess
    {
        private readonly IConfiguration _config;
        public BlizzardDataAccess(IConfiguration config)
        {
            _config = config;
        }
        public async Task<List<PointContainer>> GetAllCharacterPoints()
        {
            using (var db = new LiteDatabaseAsync(_config["DatabaseLocation"]))
            {
                var col = db.GetCollection<PointContainer>("character_points");
                var results = await col.Query().ToListAsync();

                return results;
            }
        }

        public async Task<PointContainer> GetCharacterPoints(string realm, string characterName)
        {
            using (var db = new LiteDatabaseAsync(_config["DatabaseLocation"]))
            {
                var col = db.GetCollection<PointContainer>("character_points");
                var result = await col.Query().Where(x => x._id == $"{characterName}-{realm}").SingleOrDefaultAsync();

                return result;
            }
        }

        public async Task<PointContainer> SaveCharacterPoints(PointContainer container)
        {
            using (var db = new LiteDatabaseAsync(_config["DatabaseLocation"]))
            {
                var col = db.GetCollection<PointContainer>("character_points");
                await col.EnsureIndexAsync(x => x._id);
                var characterExists = await col.ExistsAsync(x => x._id == container._id);
                if (!characterExists && container.HasDied)
                {
                    container._notAdded = true;
                    return container;
                }
                var upsertResult = await col.UpsertAsync(container);

                return container;
            }
        }

        public async Task UpdateAllCharacterPoints()
        {
            using (var db = new LiteDatabaseAsync(_config["DatabaseLocation"]))
            {
                var col = db.GetCollection<PointContainer>("character_points");
                var results = await col.Query().Where(x => !x.HasDied).ToListAsync();
                foreach(var result in results)
                {
                    ActorService.CharacterSupervisor.Tell(new UpdateCharacterMessage(result.Realm, result.CharacterName));
                }
            }
        }
    }
}
