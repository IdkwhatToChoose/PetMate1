using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using PetMate.Controllers;
using PetMate.Helpers;
using PetMate.Model;
using PetMate.ViewModels;
using System.ClientModel;
using System.Security.Claims;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Hosting;
using Google.Apis.Auth.OAuth2.Flows;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace PetMate.Controllers
{
    public class ShelterController : Controller
    {
        PetMateContext db = new PetMateContext();
        private readonly IUserAndShelterManager usermanager;
        private string? apiKey = Environment.GetEnvironmentVariable("OpenAI-API-KEY");
        private readonly string questions = "What is the pet's activity level?, 2. How sociable is the pet with people?, 3. How does the pet interact with other animals?, 4. What level of grooming does the pet require?, 5. How vocal is the pet?, 6. Is the pet trained?, 7. Does the pet have special needs or medical requirements?, 8. What type of living environment is best for this pet?, 9. How independent is the pet?, 10. How well does the pet tolerate children?, 11. What type of climate does the pet prefer?, 12. What is the pet's preferred level of interaction?, 13. What is the pet's temperament?";
        private string explanation = "Im making a website where characteristics of the registered pet will be displyed in its profile based on the answers on questions";
        private readonly string[] charactersitics = Enum.GetNames(typeof(Characteristics.Characteristics));

        private readonly IConfiguration _config;

        public ShelterController(IUserAndShelterManager _userManager, IConfiguration config)
        {
            usermanager = _userManager;
            _config = config;
        }

        public IActionResult ShelterHomePage()
        {
            return View();
        }
        public IActionResult PetRegistration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PetRegistration(PetVM petVM)
        {
            using (StreamWriter writer = System.IO.File.CreateText($"log-{DateTime.Now.ToLongTimeString()}.txt".Replace(":", "_").Replace(" ", "_")))
            {
                foreach (var claim in User.Claims)
                {
                    await writer.WriteLineAsync($"{claim} - {claim.Value}");
                }
                await writer.WriteLineAsync($"{User.Identity.Name}");
            }

            bool castrated = bool.TryParse(petVM.Castrated, out _);
            

            var shelterID = int.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
            PhotoOfPet photo = new PhotoOfPet();
            Pet newPet = new Pet();

            newPet.Name = petVM.Name;
            newPet.Size = petVM.Size;
            newPet.Age = petVM.Age;
            newPet.Castrated = castrated;
            newPet.Breed = petVM.Breed;
            newPet.ShelterId = shelterID;

            (byte[] imageBytes, string imageName) imageFile =usermanager.SetPhoto(petVM.Image);
            photo.Image = imageFile.imageBytes;
            photo.ImageName = imageFile.imageName;
            newPet.Character = AnalyseAnswers(petVM.Answers);
            try
            {
               await db.Pets.AddAsync(newPet);
               await db.SaveChangesAsync();

                photo.PetId = newPet.Id;
               await db.PhotoOfPets.AddAsync(photo);
               await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {   
                return BadRequest(ex);

            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Logout()
        {
            Claim cookie = User.FindFirst(ClaimTypes.Sid);
            cookie.Properties.Remove(ClaimTypes.Sid);

            return RedirectToAction("Index", "Home");
        }
        public string AnalyseAnswers(string answers)
        {
            string answer = usermanager.GetGPTResponse($"{explanation}.Return any of these characteristics for the user: {string.Join(", ", charactersitics)}, based on these questions: {questions} and their answers: {answers}. " + $"The last question is multiple choice, so the numbers 13 and above are the answers to the question.Also only return the chosen characteristics, nothing more, nothing less.",false);
            string[] userChar = answer.Split(':');
            string result = userChar[0].Replace("-", ", ");
            return result;
        }

        public CalendarService GetCalendarService()
        {
            try
            {
                // Define the required scopes
                string[] scopes = { CalendarService.Scope.Calendar };

                // Load the credentials file
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "credentionals.json");

                GoogleClientSecrets clientSecrets;
                using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
                {
                    clientSecrets = GoogleClientSecrets.FromStream(stream);
                }

                // Use the Google Authorization Code Flow
                var userId = "your-user-id-or-email"; // Replace with a meaningful identifier
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets.Secrets,
                    scopes,
                    userId,
                    CancellationToken.None
                ).Result;

                // Initialize the Calendar service
                return new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "PetMate"
                });
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }


        }

        public IActionResult SendMail(MailModel mailModel)
        {
            MailService service= new MailService(_config);
            service.SendEmail(mailModel.Subject, mailModel.Client_name, mailModel.Client_email, mailModel.Client_message);
            return RedirectToAction("MailSent", "Contact");
        }
    }

}