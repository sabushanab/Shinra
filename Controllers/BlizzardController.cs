using Microsoft.AspNetCore.Mvc;
using Shinra.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Controllers
{
    public class BlizzardController : Controller
    {
        private IBlizzardClient _client;
        public BlizzardController(IBlizzardClient client)
        {
            _client = client;
        }

        public async Task<ActionResult> GetCharacterStatistics(string realm, string characterName)
        {
            return Ok(await _client.GetCharacterStatistics(realm, characterName));
        }
    }
}
