using Microsoft.AspNetCore.Mvc;
using PetMate.ViewModels;
using PetMate.Model;

namespace PetMate.Controllers
{
    public class LoginController : Controller
    {
        private PetMateContext db=new PetMateContext();
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserViewModel userVM)
        {
            User user=new User();
            userVM.Username=user.Username;
            userVM.Email = user.Email;
            userVM.Password=user.Password;
           
           db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("Index","Home");
        }
    }
}
