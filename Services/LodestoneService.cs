using Shinra.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Services
{
    public class LodestoneService
    {
        private LodestoneParserService _service { get; set; }
        private ILodestoneClient _client { get; set; }
        public LodestoneService(LodestoneParserService parserService, ILodestoneClient client)
        {
            _service = parserService;
            _client = client;
        }

        public async Task<CharacterContainer> GetCharacter(long characterID)
        {
            var characterDoc = await _client.GetCharacter(characterID);
            CharacterContainer container = _service.ParseCharacter(characterDoc);

            var classJobDoc = await _client.GetCharacterClassJob(characterID);
            container = _service.ParseCharacterClassJob(container, classJobDoc);

            var mountDoc = await _client.GetCharacterMounts(characterID);
            container = _service.ParseCharacterMounts(container, mountDoc);

            var minionDoc = await _client.GetCharacterMinions(characterID);
            container = _service.ParseCharacterMinions(container, minionDoc);

            return container;
        }
    }
}
