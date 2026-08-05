using Microsoft.AspNetCore.Mvc;

namespace Shared.Security.API.System.Controllers
{
    public class SystemUsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
