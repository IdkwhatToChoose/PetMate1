using Google.Apis.Auth.OAuth2;
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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authorization;

namespace PetMate.Controllers
{
    public class ShelterController : Controller
    {
        PetMateContext db = new PetMateContext();
        Random random = new Random();

        private readonly IUserAndShelterManager usermanager;
        private string? apiKey = Environment.GetEnvironmentVariable("OpenAI-API-KEY");
        private readonly string questions = "Какво е нивото на активност на домашния любимец?, 2. Колко общителен е домашният любимец с хората?, 3. Как взаимодейства домашният любимец с други животни?, 4. Какво ниво на поддръжка изисква домашният любимец?, 5. Колко гласовит е домашният любимец?, 6. Обучен ли е домашният любимец?, 7. Има ли домашният любимец специални нужди или медицински изисквания?, 8. Какъв тип жилищна среда е най-подходяща за този домашен любимец?, 9. Колко независим е домашният любимец?, 10. До каква степен домашният любимец понася деца?, 11. Какъв тип климат предпочита домашният любимец?, 12. Какво е предпочитаното ниво на взаимодействие на домашния любимец?, 13. Какъв е темпераментът на домашния любимец?";
        private string explanation = "Правя уебсайт, където характеристиките на регистрирания домашен любимец ще се показват в неговия профил въз основа на отговорите на въпросите";
        private readonly string[] charactersitics = Enum.GetNames(typeof(Characteristics.Characteristics));
      
        private readonly IConfiguration _config;

        public ShelterController(IUserAndShelterManager _userManager, IConfiguration config)
        {
            usermanager = _userManager;
            _config = config;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ShelterHomePage()
        {
           
            ViewBag.confirm_msg = TempData["msg"] as string;
            ViewBag.msg_type = TempData["msg_type"] as string;

            int shelter_id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Shelter curr_shelter = await db.Shelters.FindAsync(shelter_id);

            List<Pet> pets_in_shelter = await db.Pets.Where(x => x.ShelterId == shelter_id).ToListAsync();
            ICollection<Request> requests = await db.Requests.Where(x=>x.ShelterId == curr_shelter.Id).ToListAsync();

            foreach (var req in requests)
            {
                req.User = await db.Users.FindAsync(req.UserId);
                
            }

            ShelterViewModel shelter_vm = new ShelterViewModel()
            {
                Id = curr_shelter.Id,
                Address = curr_shelter.Address,
                ShelterName = curr_shelter.ShelterName,
                ShelterPassword = curr_shelter.ShelterPassword,
                WorkingTime = curr_shelter.WorkingTime,
                VisitorsTime = curr_shelter.VisitorsTime,
                Type = curr_shelter.Type,
                Pets = await PetMateModel.ToPetsVM(pets_in_shelter),
                PetCount = curr_shelter.PetCount,
                AdoptionRequests= requests,
            };
            return View(shelter_vm);
        }
        public IActionResult PetRegistration()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PetRegistration(PetVM petVM)
        {

            bool castrated = bool.TryParse(petVM.Castrated, out _);
            int age=int.Parse(petVM.Age);

            var shelterID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            List<PhotoOfPet> photos = new List<PhotoOfPet>();

            Pet newPet = new Pet();

            newPet.Name = petVM.Name;
            newPet.Size = petVM.Size;
            newPet.Age = age;
            newPet.Gender= petVM.Gender;
            newPet.Castrated = castrated;
            newPet.Breed = petVM.Breed;
            newPet.ShelterId = shelterID;
            newPet.Character = AnalyseAnswers(petVM.Answers);

            await db.Pets.AddAsync(newPet);
            await db.SaveChangesAsync();

            foreach (var image in petVM.Images)
            {
                PhotoOfPet photo = new PhotoOfPet();

                (byte[] imageBytes, string imageName) imageFile = usermanager.SetPhoto(image);
                photo.Image = imageFile.imageBytes;
                photo.ImageName = imageFile.imageName;
                photo.PetId = newPet.Id;

                photos.Add(photo);
            }

            await db.PhotoOfPets.AddRangeAsync(photos);
            await db.SaveChangesAsync();
            
            TempData["msg"] = "Успешно добавихте нов любимец!";

            return RedirectToAction("ShelterHomePage", "Shelter");
        }

        [HttpGet]
        
        public async Task<IActionResult> Profile()
        {
            int shelter_id=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Shelter curr_shelter = await db.Shelters.FindAsync(shelter_id);

            List<Pet> pets_in_shelter = await db.Pets.Where(x => x.ShelterId == shelter_id).ToListAsync();
            ShelterViewModel shelter_vm = new ShelterViewModel()
            {
                Id = curr_shelter.Id,
                Address = curr_shelter.Address,
                ShelterName = curr_shelter.ShelterName,
                ShelterPassword = curr_shelter.ShelterPassword,
                WorkingTime=curr_shelter.WorkingTime,
                VisitorsTime=curr_shelter.VisitorsTime,
                Type = curr_shelter.Type,
                Pets = await PetMateModel.ToPetsVM(pets_in_shelter),
                PetCount=curr_shelter.PetCount
            };
            return View(shelter_vm);

        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1) 
            };
            Response.Cookies.Append(".AspNetCore.Cookies", "", cookieOptions);
            return RedirectToAction("Index", "Home");

        }
        public string AnalyseAnswers(string answers)
        {
            string answer = usermanager.GetGPTResponse($"{explanation}. Върни някоя от тези характеристики за потребителя: {string.Join(", ", charactersitics)}, въз основа на тези въпроси: {questions} и техните отговори: {answers}. " + $"Последният въпрос е с възможност за многократен избор, така че числата 13 и нагоре са отговорите на този въпрос. Също така върни само избраните характеристики, нито повече, нито по-малко.", false); string[] userChar = answer.Split(':');
            string result = userChar[0].Replace("-", ", "); 
            return result;
        }


       
        public async Task<IActionResult> AcceptRequest(int rid)
        { 
             string accepted = RequestStatus.Приета.ToString();
             string rejected = RequestStatus.Отхвърлена.ToString();

            try
            {
                Request req = await db.Requests.FindAsync(rid);

                if(req.Status == accepted || req.Status == rejected)
                {
                    TempData["msg"] = $"Заявката за осиновяване вече е {req.Status.ToLower()}!";
                    TempData["msg_type"] = "error";
                    return RedirectToAction("ShelterHomePage", "Shelter");
                }

                req.Status = RequestStatus.Приета.ToString();

                db.Requests.Update(req);
                await db.SaveChangesAsync();
                TempData["msg"] = "Заявката за осиновяване бе приета.";
                TempData["msg_type"] = "success";
            }
            catch(Exception)
            {
                TempData["msg"] = "Приемането на заявка за осиновяване се правали. Пробвайте да обновите страницата или опитайте по-късно.";
                TempData["msg_type"] = "error";

                return RedirectToAction("ShelterHomePage", "Shelter");
            }


            return RedirectToAction("ShelterHomePage", "Shelter");
        }
        public async Task<IActionResult> RejectRequest(int rid)
        {

            string accepted = RequestStatus.Приета.ToString();
            string rejected = RequestStatus.Отхвърлена.ToString();

            try
            {
                Request req = await db.Requests.FindAsync(rid);

                if (req.Status == accepted || req.Status == rejected)
                {
                    TempData["msg"] = $"Заявката за осиновяване вече е {req.Status.ToLower()}!";
                    TempData["msg_type"] = "error";
                    return RedirectToAction("ShelterHomePage", "Shelter");
                }

                req.Status = RequestStatus.Отхвърлена.ToString();

                db.Requests.Update(req);
                await db.SaveChangesAsync();
                TempData["msg"] = "Заявката за осиновяване бе отхвърлена.";
            }
            catch
            {
                TempData["msg"] = "Приемането на заявка за осиновяване се правали. Пробвайте да обновите страницата или опитайте по-късно.";
                return RedirectToAction("ShelterHomePage", "Shelter");
            }


            return RedirectToAction("ShelterHomePage", "Shelter");
        }
    }

}