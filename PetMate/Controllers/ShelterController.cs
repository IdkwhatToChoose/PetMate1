using Microsoft.AspNetCore.Mvc;

namespace PetMate.Controllers
{
    public class ShelterController : Controller
    {
        public IActionResult ShelterHomePage()
        {
            return View();
        }
        public IActionResult PetRegistration()
        {
            return View();
        }
    }
}
