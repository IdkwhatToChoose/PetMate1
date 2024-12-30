using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetMate.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Azure.AI.OpenAI;
using System.Text.Json.Serialization;
using OpenAI;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using OpenAI.GPT3.ObjectModels.ResponseModels;
using OpenAI.GPT3.ObjectModels.SharedModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PetMate.Model;
using PetMate.ViewModels;

namespace PetMate.Controllers
{
    public class HomeController : Controller
    {
        PetMateContext db = new PetMateContext();

        public IActionResult Index()
        {
            //List<Pet> pets=db.Pets.Take(9).ToList();
            //List<PetVM> petsVM;
            //foreach (Pet pet in pets)
            //{ var imageID = db.PhotoOfPets.FirstOrDefault(x => x.PetId == pet.Id).Id;
            //    PetVM vm = new PetVM
            //    {
            //        Id = pet.Id,
            //        Name = pet.Name,
            //       Image=GetPhoto(imageID),

            //    }

            //}
            List<PhotoOfPet> images = db.PhotoOfPets.Take(9).ToList();
            return View(images);
        }

        public IActionResult Privacy()
        {
            return View();
        }
       //public async Task<IFormFile> GetPhoto(int id)
       //{

       // var image = await db.PhotoOfPets.FindAsync(id);
       //     return image.Image;

       //}
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
}
}