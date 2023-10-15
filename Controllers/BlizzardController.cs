using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Shinra.Actors;
using Shinra.Clients;
using Shinra.Messages.Character;
using Shinra.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Controllers
{
    public class BlizzardController : Controller
    {
        private IBlizzardClient _client;
        private BlizzardParserService _service;
        private IBlizzardDataAccess _db;
        public BlizzardController(IBlizzardClient client, BlizzardParserService service, IBlizzardDataAccess db)
        {
            _client = client;
            _service = service;
            _db = db;
        }

        public async Task<ActionResult> GetRealms(string region)
        {
            return Ok(await CacheService.GetAndSetAsync($"{region.ToLower()}-realms", async () =>
            {
                var realms = new List<string>();
                var result = await _client.GetRealms(region);
                foreach(var realm in result.realms.OrderBy(x => x.name))
                {
                    realms.Add(realm.name);
                }
                return realms;
            }));
        }

        public async Task<ActionResult> GetCharacterStatistics(string region, string realm, string characterName)
        {
            return Ok(await _client.GetCharacterStatistics(region, realm, characterName));
        }

        public async Task<ActionResult> GetMythicPlusScore(string region, string realm, string characterName)
        {
            return Ok(await _client.GetMythicPlusScore(region, realm, characterName));
        }

        public async Task<ActionResult> GetMythicPlusSeasonDetails(string region, string realm, string characterName)
        {
            return Ok(await _client.GetMythicPlusSeasonDetails(region, realm, characterName));
        }

        public async Task<ActionResult> GetCharacterProfile(string region, string realm, string characterName)
        {
            return Ok(await _client.GetCharacterProfile(region, realm, characterName));
        }

        public async Task<ActionResult> GetCharacterPoints(string region, string realm, string characterName)
        {
            var parsedCharacter = await _service.ParseCharacter(region, realm, characterName);
            if (parsedCharacter.Level == 0)
            {
                return Ok(parsedCharacter);
            }
            return Ok(await _db.SaveCharacterPoints(parsedCharacter));
        }

        public async Task<ActionResult> GetAllCharacters(int page = 1)
        {
            return Ok(await _db.GetAllCharacterPoints(page));
        }

        public async Task<ActionResult> UpdateAllCharacterPoints()
        {
            await _db.UpdateAllCharacterPoints();
            return Ok("done");
        }

        public async Task<ActionResult> GetCharacterMetrics()
        {
            return Ok(await ActorService.CharacterSupervisor.Ask<CharacterMetricsResponse>(new CharacterMetricsRequest()));
        }
    }
}
