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
using Microsoft.AspNetCore.Authentication.Google;
using Google.Authentication;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace PetMate.Controllers
{
    public class LoginController : Controller
    {
        private PetMateContext db = new PetMateContext();
        private readonly IUserAndShelterManager usermanager;
        IConfiguration config;


        private readonly string questions = "1. Колко активни сте в ежедневието си?, 2. Имате ли опит с отглеждане на домашни любимци?, 3. Каква е вашата житейска ситуация?, 4. Колко често сте у дома през деня?, 5. Имате ли други домашни любимци?, 6. Има ли деца във вашето домакинство?, 7. Колко време можете да отделяте ежедневно за грижа за домашен любимец?, 8. Какво ниво на поддръжка на домашен любимец предпочитате?, 9. Предпочитате ли домашен любимец, който е независим или интерактивен?, 10. Чувствате ли се комфортно с домашни любимци, които изискват редовно подстригване?, 11. Каква е вашата поносимост към шум или вокализации (лаене, мяукане)?, 12. Как бихте оценили вашата поносимост към почистване, свързано с домашния любимец?, 13. Чувствате ли се комфортно с домашни любимци, които може да имат специални нужди?, 14. Пътувате ли често и би ли пътувал вашият домашен любимец с вас?, 15. Кои качества са най-важни за вашия идеален домашен любимец?";
        private string? apiKey = Environment.GetEnvironmentVariable("OpenAI-API-KEY");
        private string explanation = "Правя уебсайт, където характеристиките на регистрирания домашен любимец ще се показват в неговия профил въз основа на отговорите на въпросите";
        private readonly string[] charactersitics = Enum.GetNames(typeof(Characteristics.Characteristics));

        const string googleRedirectUri = $"https://localhost:7169/Login/GoogleResponse";

        public LoginController(IUserAndShelterManager _userManager, IConfiguration configuration/*,SignInManager<User> signInManager*/
   )
        {
            config = configuration; 
            usermanager = _userManager;
            //_sim = signInManager;

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
        //public IActionResult TwoFactorAuth(UserViewModel uvm)
        //{
        //    return View(uvm);
        //}

        public async Task<IActionResult> TwoFactorAuth(UserViewModel uvm,string? one)
        {
            MailService service = new MailService(config);
            HttpContext.Session.SetInt32("2faCode", await service.Send2faCode(uvm.Email));
            return View(uvm);
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
        public async Task<IActionResult> Register(UserViewModel userVM)
        {
            bool validForm=bool.Parse(userVM.Valid);
     
            if (validForm)
            {

                User user = usermanager.UserRegister(userVM); //Sets user body, except characteristics
                user.Character = AnalyseAnswers(userVM.Answers);
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();

                await usermanager.SetUserCookie(HttpContext, user.Id,"k");

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
            ViewBag.Error_msg = TempData["Error_msg"];
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
            else if (BCrypt.Net.BCrypt.Verify(textPass, user.Password))
            {
                //var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                //string token = result.Properties?.GetTokenValue("access_token");
                await usermanager.SetUserCookie(HttpContext, user.Id,"");
                
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
     
        public IActionResult GoogleLogin()
        {

            var properties = new AuthenticationProperties { RedirectUri = googleRedirectUri};
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var tokens = result.Properties.GetTokens();

            var claims = result.Principal.Identities.FirstOrDefault()
                .Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

            User? u = await db.Users.FirstOrDefaultAsync(u => u.Username == result.Principal.FindFirstValue(ClaimTypes.Name));            
            

            if (u == null)
            {
                TempData["Error_msg"] = "Потребителят не е намерен.";
                return RedirectToAction("Login","Login");
            }
            else if (u.Email == result.Principal.FindFirstValue(ClaimTypes.Email))
            {
                await usermanager.SetUserCookie(HttpContext, u.Id, result.Properties?.GetTokenValue("access_token"));
                return RedirectToAction("UserHomePage", "User");

            }
            TempData["Error_msg"] = "Грешно потребителско име или парола. Моля опитайте отново";
            return RedirectToAction("Login", "Login");

        }

        [HttpPost]
        public IActionResult RegisterUserInfo(UserViewModel uvm)
        {
            if(uvm.TwofaCode != HttpContext.Session.GetInt32("2faCode").ToString())
            {
                TempData["Message"] = "Грешен код за валидация на имейл. Опитайте отново или изпратете нов код.";
                return View("TwoFactorAuth",uvm);
            }
            return View("HabitForm",uvm);
        }
    }
}
