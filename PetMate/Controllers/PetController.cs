using Microsoft.AspNetCore.Mvc;
using PetMate.Model;
using PetMate.ViewModels;

namespace PetMate.Controllers
{
    public class PetController : Controller
    {
        PetMateContext db=new PetMateContext();

        public async Task<IActionResult> PetProfile(int id,IFormFile img)
        {
            Pet? pet = await db.Pets.FindAsync(id);
            PetVM petVM = new PetVM()
            {
                Id = pet.Id,
                Name= pet.Name,
                Age=pet.Age,
                Breed=pet.Breed,
                Size=pet.Size,
                Castrated=pet.Castrated.ToString(),
                ShelterId=pet.ShelterId,
                AdopterId=pet.AdopterId,
                Character=pet.Character,
                Image=img
            };

            return View(petVM);
        }
        public IActionResult PetRegistration()
        {
            return View();
        }
    }
}
