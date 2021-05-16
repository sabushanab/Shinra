using Microsoft.AspNetCore.Mvc;

namespace Shinra.Controllers
{
    public class PartyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
