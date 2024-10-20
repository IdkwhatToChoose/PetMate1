using Microsoft.AspNetCore.Mvc;
using PetMate.ViewModels;
using PetMate.Model;
using PetMate.Helpers;
using System.Runtime.InteropServices;

namespace PetMate.Controllers
{
    public class LoginController : Controller
    {
        private PetMateContext db=new PetMateContext();
        private readonly IUserAndShelterManager usermanager;
        public LoginController(IUserAndShelterManager _userManager) { 
              usermanager=_userManager;
        }
       
        public IActionResult LoginShelter()
        {

            return View();
        }
        public IActionResult Registration()
        {
            return View();
        }
        public IActionResult RegistrationShelter()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegistrationShelter(ShelterViewModel svm)
        {
            Shelter shelter = usermanager.ShelterRegister(svm);
            db.Shelters.AddAsync(shelter);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult Register(UserViewModel userVM)
        {
            User user=usermanager.UserRegister(userVM);//Adds to database 
            
            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Login(UserViewModel userVM)
        {
            User? user = db.Users.FirstOrDefault(x => x.Email == userVM.Email);

            string textPass = userVM.Password;
            userVM.Password = BCrypt.Net.BCrypt.HashPassword(userVM.Password);
            if (user == null)
            {
                return View();
            }
            else if (BCrypt.Net.BCrypt.Verify(textPass, userVM.Password) == true)
            {

                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
