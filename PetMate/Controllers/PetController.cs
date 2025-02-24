using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PetMate.Helpers;
using PetMate.Model;
using PetMate.ViewModels;
using System.Security.Claims;

namespace PetMate.Controllers
{
    public class PetController : Controller
    {
        PetMateContext db=new PetMateContext();

        [HttpGet]
        public async Task<IActionResult> PetProfile(int id)
        {
            PetVM petVM = await PetMateModel.ToPetVM(id);
            ViewBag.request_msg = TempData["request_msg"];

            return View(petVM);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendAdoptionRequest(int pid)
        {
            string link = "<а href='@Url.Action(\"Login\",\"Login\")'>Тук</а>";

            DateTime date = DateTime.Now;
            try
            {
                Pet petdtls = await db.Pets.FindAsync(pid);
                
                var cookie = User.FindFirst(ClaimTypes.NameIdentifier);
                
                int userID = int.Parse(cookie.Value);

                
                Shelter shelter = await db.Shelters.FindAsync(petdtls.ShelterId);
                User user = await db.Users.FindAsync(userID);

                Request request = new Request()
                {
                    PetId = pid,
                    UserId = user.Id,
                    ShelterId = shelter.Id,
                    Status = RequestStatus.в_очакване.ToString(),
                    Datetime = date,
                    Shelter = shelter,
                    Pet = petdtls,
                    User = user,
                };
                

                shelter.Requests.Add(request);
                user.Requests.Add(request);
                petdtls.Requests.Add(request);
                

                await db.Requests.AddAsync(request);
                await db.SaveChangesAsync();
                TempData["request_msg"] = $"Заявка за осиновяване бе успешно изпратена до приютът на любимец {petdtls.Name}";
            }
            catch(NullReferenceException)
            {
                TempData["request_msg"] = $"Пращането на заявка за осиновяване се провали. Ако не сте влезли в профила си направете го {link} или опитайте по-късно";
                return RedirectToAction("PetProfile", "Pet", new { id = pid });
            }
            
            

            return RedirectToAction("PetProfile", "Pet", new {id=pid});
        }
        public IActionResult PetRegistration()
        {
            return View();
        }
    }
}
