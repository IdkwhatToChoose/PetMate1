using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;
using PetMate.Model;

namespace PetMate.Controllers
{
    [Authorize]
    public class UserController : Controller
    {


        public IActionResult UserHomePage()
        {
            return View();
        }
        public IActionResult UserProfile()
        {

            return View();
        }


    }
}
