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
using Microsoft.AspNetCore.Identity;
using PetMate.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace PetMate.Controllers
{
    public class HomeController : Controller
    {

        PetMateContext db = new PetMateContext();
        private readonly IFileManager filemanager;

        public HomeController(IFileManager _file)
        {
            filemanager = _file;
        }

        //public IActionResult Gallery()
        //{
        //    return View();
        //}
        public async Task<IActionResult> Index()
        {
            ModelServ model = new ModelServ(filemanager);

            List<Pet> pets = await db.Pets.ToListAsync();
            List<PetVM> petsVM = await model.ToPetVM(pets);
            return View(petsVM);
        }

        [HttpGet]
        public async Task<IActionResult> Gallery(int page)
        {
             ModelServ model = new ModelServ(filemanager);

            List<Pet> pets = await db.Pets.ToListAsync();
            var pagedPhotos = pets.
                Skip((page - 1) * 20).
                Take(20).
                ToList();

            List<PetVM> photos = await model.ToPetVM(pagedPhotos);

            ViewBag.TotalPages = Math.Ceiling((double)pets.Count / 20);
            ViewBag.CurrentPage = page; //Page ur currently on

            return View(photos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}