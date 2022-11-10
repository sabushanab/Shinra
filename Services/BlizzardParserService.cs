using Shinra.Clients;
using Shinra.Clients.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Services
{
    public class BlizzardParserService
    {
        private IBlizzardClient _client;
        public BlizzardParserService(IBlizzardClient client)
        {
            _client = client;
        }
        public async Task<CharacterStatistics> ParseCharacter(string realm, string characterName)
        {
            
            return await _client.GetCharacterStatistics(realm, characterName); ;
        }
    }
}
