using Microsoft.AspNetCore.Mvc;

namespace PetMate.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult about()
        {
            return View();
        }
    }
}
