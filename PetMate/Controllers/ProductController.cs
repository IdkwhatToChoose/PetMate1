using Microsoft.AspNetCore.Mvc;

namespace PetMate.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Product()
        {
            return View();
        }
    }
}
