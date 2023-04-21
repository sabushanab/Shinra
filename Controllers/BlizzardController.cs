using Microsoft.AspNetCore.Mvc;
using Shinra.Clients;
using Shinra.Services;
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

        public async Task<ActionResult> GetCharacterStatistics(string realm, string characterName)
        {
            return Ok(await _client.GetCharacterStatistics(realm, characterName));
        }

        public async Task<ActionResult> GetCharacterProfile(string realm, string characterName)
        {
            return Ok(await _client.GetCharacterProfile(realm, characterName));
        }

        public async Task<ActionResult> GetCharacterPoints(string realm, string characterName)
        {
            var parsedCharacter = await _service.ParseCharacter(realm, characterName);
            if (parsedCharacter.Level == 0)
            {
                return Ok(parsedCharacter);
            }
            return Ok(await _db.SaveCharacterPoints(parsedCharacter));
        }

        public async Task<ActionResult> GetAllCharacters()
        {
            return Ok(await _db.GetAllCharacterPoints());
        }

        public async Task<ActionResult> UpdateAllCharacterPoints()
        {
            await _db.UpdateAllCharacterPoints();
            return Ok("done");
        }
    }
}
