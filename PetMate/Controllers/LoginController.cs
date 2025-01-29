using Microsoft.AspNetCore.Mvc;
using PetMate.ViewModels;
using PetMate.Model;
using PetMate.Helpers;
using System.Runtime.InteropServices;
using OpenAI.Chat;
using System.ClientModel;
using PetMate.Characteristics;
using System.Reflection.PortableExecutable;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.SqlServer.Server;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PetMate.Controllers
{
    public class LoginController : Controller
    {
        private PetMateContext db = new PetMateContext();
        private readonly IUserAndShelterManager usermanager;
        private readonly string questions = "1.How active are you on a daily basis?, 2. Do you have experience with pet ownership?, 3. What is your living situation?, 4. How often are you at home during the day?, 5. Do you have other pets?, 6. Are there children in your household?, 7. How much time can you commit to pet care daily?, 8. What’s your preferred level of pet maintenance?, 9. Do you prefer a pet that is independent or interactive?, 10. Are you comfortable with pets that require regular grooming?, 11. What’s your tolerance for noise or vocalizations (barking, meowing)?, 12. How would you rate your tolerance for pet-related cleaning?, 13. Are you comfortable with pets that may have special needs?, 14. Do you travel frequently, and would your pet travel with you?, 15. What qualities are most important in your ideal pet?";
        private string? apiKey = Environment.GetEnvironmentVariable("OpenAI-API-KEY");
        private string explanation = "Im making a website where characteristics of the user will be displyed in his profile based on his answers on questions";
        private readonly string[] charactersitics = Enum.GetNames(typeof(Characteristics.Characteristics));

        public LoginController(IUserAndShelterManager _userManager)
        {
            usermanager = _userManager;
        }

        public IActionResult LoginShelter()
        {

            return View();
        }
        public IActionResult Registration()
        {
            //UserViewModel uvm=new UserViewModel();
            return View();
        }
        public IActionResult HabitForm()
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
            bool validForm=bool.Parse(userVM.Valid);

            if (validForm)
            {
                User user = usermanager.UserRegister(userVM);//Sets user body, except characteristics
                user.Character = AnalyseAnswers(userVM.Answers);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Registration", "Login");
        }
        public string AnalyseAnswers(string answers)
        {
            string answer = GetGPTResponse($"{explanation}.Return any of these characteristics for the user: {string.Join(", ", charactersitics)}, based on these questions: {questions} and their answers: {answers}. " + $"The last question is multiple choice, so the numbers 15 and above are the answers to the question.Also only print the chosen characteristics.");
            string[] userChar = answer.Split(':');
            string result = userChar[1].Replace("-", ", ");
            return result;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel userVM)
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
                await usermanager.SetUserCookie(HttpContext, user.Id, user.Email);
                
                return RedirectToAction("UserHomePage","User");

            }
            return View();
        }

        [HttpPost]
      async public Task<IActionResult> LoginShelter(ShelterViewModel shelterVM)
        {
            Shelter? shelter = await db.Shelters.FirstOrDefaultAsync(x => x.ShelterName == shelterVM.ShelterName);

            string textPass = shelterVM.ShelterPassword;
            shelterVM.ShelterPassword = BCrypt.Net.BCrypt.HashPassword(shelterVM.ShelterPassword);


            using (StreamWriter writer = System.IO.File.CreateText($"log-{DateTime.Now.ToLongTimeString()}.txt".Replace(":", "_").Replace(" ", "_")))
            {
                await writer.WriteLineAsync($"{shelter?.Id} - {shelter?.ShelterName}");
            }

            if (shelter == null)
            {
                return View();
            }
            else if (BCrypt.Net.BCrypt.Verify(textPass, shelterVM.ShelterPassword) == true)
            {
                await usermanager.SetShelterCookie(HttpContext, shelter.Id);
                return RedirectToAction("ShelterHomePage", "Shelter");
            }
            return View();
        }
        private string GetGPTResponse(string prompt)
        {
            string model = "gpt-3.5-turbo"; // Specify the model (e.g., gpt-4)
            ChatClient chatClient = new ChatClient(model, apiKey);

            // Create messages using the specific message types
            List<ChatMessage> messages = new List<ChatMessage>
            {
                 ChatMessage.CreateSystemMessage("You are a helpful assistant."),
                 ChatMessage.CreateUserMessage(prompt)
            };

            try
            {
                // Send chat request (synchronous)
                ClientResult<ChatCompletion> result = chatClient.CompleteChat(messages);

                if (result?.Value != null)
                {
                    // Access the response content directly through the flattened property
                    return result.Value.Content[0].Text;
                }
                else
                {
                    return "Error: The result value is null or the operation was unsuccessful.";
                }
            }
            catch (Exception ex)
            {
                return "An error occurred: " + ex.Message;
            }
        }


   
    }
}
