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

namespace PetMate.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        PetMateContext db = new PetMateContext();

        private readonly IFileManager filemanager;
        private readonly IUserAndShelterManager manager;

        public UserController(IFileManager _file, IUserAndShelterManager _manager)
        {
            filemanager = _file;
            manager = _manager;
        }

        public IActionResult UserHomePage()
        {
            return View();
        }
        public IActionResult UserProfile()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Gallery(int page)
        {
            var userID = int.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
            User? user = await db.Users.FindAsync(userID);
            ModelServ model = new ModelServ(filemanager);

            List<Pet> pets = await db.Pets.ToListAsync();
            var pagedPhotos = pets.
                Skip((page - 1) * 20).
                Take(20).
                ToList();

            var petCharacteristics = pagedPhotos.Select(pet => new
            {
                pet.Name,
                pet.Character

            }).ToList();

            string promptData = JsonSerializer.Serialize(petCharacteristics);

            var matchedPets = manager.GetGPTResponse($"Take a look at these pets:{promptData}.Order them based on how much the user will like each pet, starting from the most liked ones, to the least liked ones. Use the user's characteristics: [{user.Character}] and the pet's characteristics to order them. Return them in a JSON like format.Return all pets, no exception.", true);
            var matchedPets_list = JsonSerializer.Deserialize<List<string>>(matchedPets)
            .Select(name => name.Trim().ToLower())
            .ToList();
            

            pagedPhotos = pets.Where(pet => matchedPets_list.Contains(pet.Name.ToLower()))
                      .OrderBy(pet => matchedPets_list.IndexOf(pet.Name.ToLower()))
                      .ToList();
            List<PetVM> photos = await model.ToPetVM(pagedPhotos);

            ViewBag.TotalPages = Math.Ceiling((double)pets.Count / 20);
            ViewBag.CurrentPage = page; //Page ur currently on

            return View(photos);
        }
    }
}
