using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shinra.Clients;
using Shinra.Services;
using System.Threading.Tasks;

namespace Shinra.Controllers
{
    public class XIVAPIController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IXIVAPIClient _client;

        public XIVAPIController(ILogger<HomeController> logger, IXIVAPIClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<ActionResult> GetFreeCompanyMembers() 
        {
            return Json(await CacheService.GetAndSetAsync<FreeCompanyMembersContainer>(nameof(XIVAPIClient.GetFreeCompanyMembers), _client.GetFreeCompanyMembers));
        }

        public async Task<ActionResult> GetCharacter(int id) 
        {
            return Json(await CacheService.GetAndSetAsync<CharacterContainer>(id.ToString(), () => _client.GetCharacter(id)));
        }
    }
}
