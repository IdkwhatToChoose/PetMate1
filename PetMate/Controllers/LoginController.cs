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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Html;

namespace PetMate.Controllers
{
    public class LoginController : Controller
    {
        private PetMateContext db = new PetMateContext();
        private readonly IUserAndShelterManager usermanager;
        private readonly string questions = "1. Колко активни сте в ежедневието си?, 2. Имате ли опит с отглеждане на домашни любимци?, 3. Каква е вашата житейска ситуация?, 4. Колко често сте у дома през деня?, 5. Имате ли други домашни любимци?, 6. Има ли деца във вашето домакинство?, 7. Колко време можете да отделяте ежедневно за грижа за домашен любимец?, 8. Какво ниво на поддръжка на домашен любимец предпочитате?, 9. Предпочитате ли домашен любимец, който е независим или интерактивен?, 10. Чувствате ли се комфортно с домашни любимци, които изискват редовно подстригване?, 11. Каква е вашата поносимост към шум или вокализации (лаене, мяукане)?, 12. Как бихте оценили вашата поносимост към почистване, свързано с домашния любимец?, 13. Чувствате ли се комфортно с домашни любимци, които може да имат специални нужди?, 14. Пътувате ли често и би ли пътувал вашият домашен любимец с вас?, 15. Кои качества са най-важни за вашия идеален домашен любимец?";
        private string? apiKey = Environment.GetEnvironmentVariable("OpenAI-API-KEY");
        private string explanation = "Правя уебсайт, където характеристиките на регистрирания домашен любимец ще се показват в неговия профил въз основа на отговорите на въпросите";
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
            //ViewBag.err_msg_reg = TempData["err_msg_reg"] as string; 
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
        public async Task<IActionResult> RegistrationShelter(ShelterViewModel svm)
        {
            Shelter shelter = usermanager.ShelterRegister(svm);
           
            await db.Shelters.AddAsync(shelter);
            await db.SaveChangesAsync();
            await usermanager.SetShelterCookie(HttpContext, shelter.Id,Response);
            

            return RedirectToAction("ShelterHomePage", "Shelter");
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

                usermanager.SetUserCookie(HttpContext, user.Id,Response);

                return RedirectToAction("UserHomePage", "User");
            }
            TempData["err_msg_reg"] = "Моля попълнете всички въпроси!";
            return RedirectToAction("Registration", "Login");
        }
        public string AnalyseAnswers(string answers)
        {
            string answer = usermanager.GetGPTResponse($"{explanation}. Върни някоя от тези характеристики за потребителя: {string.Join(", ", charactersitics)}, въз основа на тези въпроси: {questions} и техните отговори: {answers}. " + $"Последният въпрос е с множество избори, така че числата 15 и нагоре са отговори на този въпрос. Също така, отпечатай само избраните характеристики.", false);
            return answer;
            
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
          
            User? user = await db.Users.FirstOrDefaultAsync(x => x.Username == userVM.Username);
            string textPass = userVM.Password;
            userVM.Password = BCrypt.Net.BCrypt.HashPassword(userVM.Password);
            if (user == null)
            {
                ViewBag.Error_msg = "Потребителят не е намерен.";
                return View();
            }
            else if (BCrypt.Net.BCrypt.Verify(textPass, user.Password)==true)
            {
                await usermanager.SetUserCookie(HttpContext, user.Id,Response);
                
                return RedirectToAction("UserHomePage","User");

            }
            ViewBag.Error_msg = "Грешно потребителско име или парола. Моля опитайте отново";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<IActionResult> LoginShelter(ShelterViewModel shelterVM)
        {
            
            Shelter? shelter = await db.Shelters.FirstOrDefaultAsync(x => x.ShelterName == shelterVM.ShelterName);
            try {
                string textPass = shelterVM.ShelterPassword;
                shelterVM.ShelterPassword = BCrypt.Net.BCrypt.HashPassword(shelterVM.ShelterPassword);

                if (shelter == null)
                {
                    ViewBag.Error_msg = "Приютът не е намерен.";
                    return View();
                }
                else if (BCrypt.Net.BCrypt.Verify(textPass, shelter.ShelterPassword) == true)
                {
                    await usermanager.SetShelterCookie(HttpContext, shelter.Id, Response);
                    return RedirectToAction("ShelterHomePage", "Shelter");
                }
            }
            catch(Exception)
            {
                ViewBag.Error_msg = "Грешно потребителско име или парола. Моля опитайте отново.";
                return View();
            }
            ViewBag.Error_msg = "Грешно потребителско име или парола. Моля опитайте отново.";
            return View();
        }
        private string GetGPTResponse(string prompt)
        {
            string model = "gpt-3.5-turbo"; // Specify the model (e.g., gpt-4)
            ChatClient chatClient = new ChatClient(model, apiKey);

            // Create messages using the specific message types
            List<ChatMessage> messages = new List<ChatMessage>
            {
                 ChatMessage.CreateSystemMessage("You are a helpful assistant. Respond in bulgarian."),
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
