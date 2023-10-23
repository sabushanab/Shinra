using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shinra.Actors;
using Shinra.Clients;
using Shinra.Messages.Character;
using Shinra.Services;
using Shinra.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Controllers
{
    public class BlizzardController : Controller
    {
        private readonly IBlizzardClient _client;
        private readonly BlizzardParserService _service;
        private readonly IBlizzardDataAccess _db;
        private readonly IConfiguration _configuration;
        public BlizzardController(IBlizzardClient client, BlizzardParserService service, IBlizzardDataAccess db, IConfiguration configuration)
        {
            _client = client;
            _service = service;
            _db = db;
            _configuration = configuration;
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

        public async Task<ActionResult> GetCharacterPoints(string region, string realm, string characterName)
        {
            if (await _db.DoesCharacterExist($"{region}-{characterName}-{realm}"))
            {
                return Ok(new PointContainer()
                {
                    CharacterName = characterName,
                    Realm = realm,
                    _notAdded = true
                });
            }
            var parsedCharacter = await _service.ParseCharacter(region, realm, characterName);
            if (parsedCharacter.Level == -1)
            {
                return Ok(parsedCharacter);
            }
            return Ok(await _db.SaveCharacterPoints(parsedCharacter));
        }

        public async Task<ActionResult> GetAllCharacters(int page = 1)
        {
            return Ok(await _db.GetAllCharacterPoints(page));
        }

        public async Task<ActionResult> UpdateAllCharacterPoints(string adminPassword)
        {
            if (adminPassword != _configuration.GetValue("adminPassword", ""))
            {
                return BadRequest();
            }
            await _db.UpdateAllCharacterPoints();
            return Ok("done");
        }

        public async Task<ActionResult> GetCharacterMetrics()
        {
            return Ok(await ActorService.CharacterSupervisor.Ask<CharacterMetricsResponse>(new CharacterMetricsRequest()));
        }
    }
}
