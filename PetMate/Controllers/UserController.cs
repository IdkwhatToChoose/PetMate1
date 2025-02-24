using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;
using PetMate.Model;
using PetMate.Helpers;
using PetMate.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;
using static OpenAI.GPT3.ObjectModels.SharedModels.IOpenAiModels;

namespace PetMate.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        PetMateContext db = new PetMateContext();

        private readonly IUserAndShelterManager manager;

        public UserController(IUserAndShelterManager _manager)
        {
            manager = _manager;
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            int uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User? user=await db.Users.FindAsync(uid);
            //user.Requests=await db.Requests.Where(r=>r.UserId== uid).ToListAsync();

            foreach(var req in user.Requests)
            {
                req.Shelter = await db.Shelters.FindAsync(req.ShelterId);
                req.Pet = await db.Pets.FindAsync(req.PetId);
            }

            return View(user);
        }
        
        [HttpGet]
        public async Task<IActionResult> UserHomePage()
        {
            int uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<Pet> pets = await db.Pets.Take(15).ToListAsync();
            UserViewModel userpage=new UserViewModel()
            {
                Pets= await PetMateModel.ToPetsVM(pets),
                Requests = await db.Requests.Where(r => r.UserId == uid).ToListAsync(),
            };
            foreach (var req in userpage.Requests)
            {
                req.Shelter = await db.Shelters.FindAsync(req.ShelterId);
                req.Pet = await db.Pets.FindAsync(req.PetId);
                req.User = await db.Users.FindAsync(uid);
            }

            userpage.Pets.Reverse();
            return View(userpage);
        }
        
        [HttpGet]
        public async Task<IActionResult> Gallery(int page)
        {
            page = page.Equals(0) ? 1 : page;
            var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            User? user = await db.Users.FindAsync(userID);

            List<Pet> pets = await db.Pets.Take(20).ToListAsync();
            pets = pets.Skip((page - 1) * 20).ToList();
            

            var petCharacteristics = pets.Select(pet => new
            {
                pet.Name,
                pet.Character

            }).ToList();

            string promptData = JsonSerializer.Serialize(petCharacteristics);

            var matchedPets = manager.GetGPTResponse($"Take a look at these pets:{promptData}.Order them based on how much the user will like each pet, starting from the most liked ones, to the least liked ones. Use the user's characteristics: [{user.Character}] and the pet's characteristics to order them. Return them in a JSON like format.Include all pets in the sorting, no exception.", true);
            var matchedPets_list = JsonSerializer.Deserialize<List<string>>(matchedPets)
            .Select(name => name.Trim().ToLower())
            .ToList();



            if (pets.Count > 1)
            {
                pets = pets.Where(pet => matchedPets_list.Contains(pet.Name.ToLower()))
                        .OrderBy(pet => matchedPets_list.IndexOf(pet.Name.ToLower()))
                        .ToList();
            }

            List<PetVM> photos = await PetMateModel.ToPetsVM(pets);

            ViewBag.TotalPages = Math.Ceiling((double)pets.Count / 20);
            ViewBag.CurrentPage = page; //Page ur currently on

            return View(photos);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1) // Set expiration to a past date
            };
            Response.Cookies.Append(".AspNetCore.Cookies_User", "", cookieOptions);
            return RedirectToAction("Index", "Home");

        }
        
    }
}
