using Microsoft.AspNetCore.Mvc;
using Shinra.Clients;
using Shinra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Controllers
{
    public class BlizzardController : Controller
    {
        private IBlizzardClient _client;
        private BlizzardParserService _service;
        public BlizzardController(IBlizzardClient client, BlizzardParserService service)
        {
            _client = client;
            _service = service;
        }

        public async Task<ActionResult> GetCharacterStatistics(string realm, string characterName)
        {
            return Ok(await _client.GetCharacterStatistics(realm, characterName));
        }

        public async Task<ActionResult> GetCharacterPoints(string realm, string characterName)
        {
            return Ok(await _service.ParseCharacter(realm, characterName));
        }
    }
}
