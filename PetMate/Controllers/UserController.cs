using Microsoft.AspNetCore.Mvc;

namespace PetMate.Controllers
{
    public class UserController : Controller
    {
        public IActionResult UserHomePage()
        {
            return View();
        }
    }
}
