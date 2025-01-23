using Microsoft.AspNetCore.Mvc;

namespace PetMate.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult MailSent()
        {
            return View();
        }
    }
}
