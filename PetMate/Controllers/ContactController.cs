using Microsoft.AspNetCore.Mvc;
using PetMate.ViewModels;

namespace PetMate.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Success(SuccessStatus status)
        {
            return View(status);
        }
    }
}
