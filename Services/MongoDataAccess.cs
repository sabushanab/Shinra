using Akka.Actor;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Shinra.Actors;
using Shinra.Messages.Character;
using Shinra.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Services
{
    public class MongoDataAccess : IBlizzardDataAccess
    {
        private readonly IMongoCollection<PointContainer> _pointCollection;
        public MongoDataAccess(IConfiguration config)
        {
            var reachDB = new MongoClient(config["MongoConnectionString"]).GetDatabase("reach");
            _pointCollection = reachDB.GetCollection<PointContainer>("characterPoints");
            var indexModels = new List<CreateIndexModel<PointContainer>>()
            {
                new CreateIndexModel<PointContainer>(Builders<PointContainer>.IndexKeys.Descending(x => x.TotalPoints)),
                new CreateIndexModel<PointContainer>(Builders<PointContainer>.IndexKeys.Ascending(x => x._id)),
            };
            _pointCollection.Indexes.CreateMany(indexModels);
            
        }

        public async Task<LeaderboardResult> GetAllCharacterPoints(int page, int pageSize = 1)
        {
            var filter = Builders<PointContainer>.Filter.Empty;
            var findOptions = new FindOptions<PointContainer, PointContainer>()
            {
                Sort = Builders<PointContainer>.Sort.Descending(x => x.TotalPoints),
                Skip = (page - 1) * pageSize,
                Limit = pageSize
            };
            var leaderboardResult = new LeaderboardResult();
            leaderboardResult.Characters = (await _pointCollection.FindAsync(filter, findOptions)).ToList();
            leaderboardResult.TotalCount = await _pointCollection.EstimatedDocumentCountAsync();
            return leaderboardResult;
        }

        public async Task<PointContainer> GetCharacterPoints(string realm, string characterName)
        {
            var filter = Builders<PointContainer>.Filter.Eq(x => x._id, $"{characterName}-{realm}");
            return (await _pointCollection.FindAsync(filter)).SingleOrDefault();
        }

        public async Task<PointContainer> SaveCharacterPoints(PointContainer container)
        {
            var filter = Builders<PointContainer>.Filter.Eq(x => x._id, container._id);
            if (_pointCollection.CountDocuments(filter) > 0 && container.HasDied)
            {
                container._notAdded = true;
                return container;
            }
            container.LastUpdated = DateTime.UtcNow;
            await _pointCollection.ReplaceOneAsync<PointContainer>(x => x._id == container._id, container, new ReplaceOptions() { IsUpsert = true });

            return container;
        }

        public async Task UpdateAllCharacterPoints()
        {
            var filter = Builders<PointContainer>.Filter.Eq(x => x.HasDied, false);
            var results =  (await _pointCollection.FindAsync(filter)).ToList();

            foreach (var result in results)
            {
                ActorService.CharacterSupervisor.Tell(new UpdateCharacterMessage(result.Region.ToLower(), result.Realm.ToLower(), result.CharacterName.ToLower(), result.Level, result.CharacterClass));
            }
        }
    }
}
