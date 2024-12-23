using Microsoft.AspNetCore.Mvc;

namespace PetMate.Controllers
{
    public class PetController : Controller
    {
        public IActionResult PetProfile()
        {
            return View();
        }
        public IActionResult PetRegistration()
        {
            return View();
        }
    }
}
