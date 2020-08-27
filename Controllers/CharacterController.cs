using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Shinra.Actors;
using Shinra.Clients;
using Shinra.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shinra.Controllers
{
    public class CharacterController : Controller
    {
        private IXIVAPIClient _client;
        public CharacterController(IXIVAPIClient client)
        {
            _client = client;
        }
        public async Task<string> Queue()
        {
            return await ActorService.CharacterSupervisor.Ask<string>(new GetQueueLength());
        }

        public async Task<ActionResult> Update()
        {
            await _client.GetEachFreeCompanyMember();
            return Json("Character Update In Progress");
        }
    }
}
