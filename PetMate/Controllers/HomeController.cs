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

namespace PetMate.Controllers
{
    public class HomeController : Controller
    {


        public IActionResult Index(bool LoggedIn)
        {
            ViewBag.LoggedIn = LoggedIn;
            return View();
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