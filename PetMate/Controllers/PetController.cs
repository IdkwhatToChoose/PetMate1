using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using PetMate.Helpers;
using PetMate.Model;
using PetMate.ViewModels;
using Stripe;
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
            ViewBag.msg_type= TempData["msg_type"];

            return View(petVM);
        }

        [HttpPost]
        public async Task<IActionResult> Donation(int pid)
        {
            return View(await PetMateModel.ToPetVM(pid));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendAdoptionRequest(int pid)
        {
            //string link = "<а href='@Url.Action(\"Login\",\"Login\")'>Тук</а>";

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
                
                if(await RequestDuplicated(request))
                {
                    TempData["request_msg"] = "Вече сте пратили завка за осиновяване към този любимец !";
                    TempData["msg_type"] = "error";

                    return RedirectToAction("PetProfile", "Pet", new { id = pid });
                }

                shelter.Requests.Add(request);
                user.Requests.Add(request);
                petdtls.Requests.Add(request);
                
                await db.Requests.AddAsync(request);
                await db.SaveChangesAsync();

                TempData["request_msg"] = $"Заявка за осиновяване бе успешно изпратена до приютът на любимец {petdtls.Name}";
                TempData["msg_type"] = "success";
            }
            catch(Exception)
            {
                TempData["request_msg"] = $"Пращането на заявка за осиновяване се провали. Може вече да сте пратили зявка, или не сте влезли в профила си.";
                TempData["msg_type"] = "error";

                return RedirectToAction("PetProfile", "Pet", new { id = pid });
            }
            
            

            return RedirectToAction("PetProfile", "Pet", new {id=pid});
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Donate(Donation donation)
        {
            Charge payment;
            var pet = await db.Pets.FindAsync(donation.PetId);

            try
            {
                ChargeCreateOptions options = new ChargeCreateOptions()
                {
                    Amount = long.Parse(donation.Amount+"00"),
                    Currency = "bgn",
                    Description = $"Дарение от {donation.Username} на {pet.Name}",
                    Source = donation.StripeToken,
                };

                var service = new ChargeService();
                payment = await service.CreateAsync(options);

                await db.Sponsorships.AddAsync(new Sponsorship()
                {
                    Amount = long.Parse(donation.Amount),
                    PetId = pet.Id,
                    UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    KeepUpdated = donation.SendUpdates
                });

                await db.SaveChangesAsync();

            }catch (StripeException) {

                ViewBag.Message = "Карта ви е невалидна или не съществува. Опитайте с друг номер";
                return RedirectToAction("Donation", new { pid = pet.Id });
            }
            

            return RedirectToAction("Success","Contact",new SuccessStatus()
            {
                Header = $"Дарението ви бе успешно",
                Message = $"Ние искрено оценяваме вашето щедро дарение. Вашата подкрепа ще бъде от пряка полза за {pet.Name} осигурявайки основни грижи, храна и медицинско обслужване. Вашата доброта играе жизненоважна роля за подобряването на живота на животните в нужда и ние сме наистина благодарни за вашето състрадание.",
                Operation = OType.Donation
            });
        }
        public IActionResult PetRegistration()
        {
            return View();
        }
        public async Task<bool> RequestDuplicated(Request request)
        {
            return await db.Requests
                .AnyAsync(r =>
                    r.UserId == request.UserId &&
                    r.PetId == request.PetId &&
                    r.ShelterId == request.ShelterId);
        }
    }
}
