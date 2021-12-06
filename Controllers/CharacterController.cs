using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Shinra.Actors;
using Shinra.Clients;
using Shinra.Messages;
using Shinra.Services;
using System.Threading.Tasks;

namespace Shinra.Controllers
{
    public class CharacterController : Controller
    {
        private IXIVAPIClient _client;
        private LodestoneService _service;
        public CharacterController(IXIVAPIClient client, LodestoneService service)
        {
            _client = client;
            _service = service;
        }
        public async Task<string> Queue()
        {
            return await ActorService.CharacterSupervisor.Ask<string>(new GetQueueLength());
        }

        public async Task<ActionResult> GetFreeCompanyMembers()
        {
            return Json(await CacheService.GetAndSetAsync<FreeCompanyMembersContainer>(nameof(XIVAPIClient.GetFreeCompanyMembers), _client.GetFreeCompanyMembers));
        }

        public async Task<ActionResult> GetCharacter(int id)
        {
            return Json(await CacheService.GetAndSetAsync<CharacterContainer>(id.ToString(), () => _service.GetCharacter(id)));
        }

        public async Task<ActionResult> Update()
        {
            await _client.GetEachFreeCompanyMember();
            return Json("Character Update In Progress");
        }
    }
}
