using Microsoft.AspNetCore.Mvc;

namespace PetMate.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Service()
        {
            return View();
        }
    }
}
