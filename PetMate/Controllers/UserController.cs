using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;
using PetMate.Model;
using PetMate.Helpers;
using PetMate.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System.Configuration;
using Microsoft.AspNetCore.Html;
using Google;

namespace PetMate.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        PetMateContext db = new PetMateContext();
        private readonly IUserAndShelterManager manager;
        private IConfiguration _configuration;

        private string redirectUri = "https://localhost:7169/User/Callback";
        private string clientId;
        private string clientSecret;



        public UserController(IUserAndShelterManager _manager, IConfiguration configuration)
        {
            manager = _manager;
            _configuration = configuration;
            clientId = _configuration["ZoomCliid"];
            clientSecret = _configuration["ZoomCliSecret"];
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            int uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User? user = await db.Users.FindAsync(uid);
            //user.Requests=await db.Requests.Where(r=>r.UserId== uid).ToListAsync();

            foreach (var req in user.Requests)
            {
                req.Shelter = await db.Shelters.FindAsync(req.ShelterId);
                req.Pet = await db.Pets.FindAsync(req.PetId);
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> UserHomePage()
        {
            ViewBag.confirm_msg = TempData["msg"] as string;
            ViewBag.msg_type = TempData["msg_type"] as string;
            string embedUrl = "";

            string? token = User.FindFirstValue(ClaimTypes.Authentication);
            CalendarService service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromAccessToken(token),
                ApplicationName = "Petmate"
            });
            try
            {
                 var cal = await service.Calendars.Get("primary").ExecuteAsync();
                embedUrl = $"https://calendar.google.com/calendar/embed?src={cal.Id}&ctz={cal.TimeZone}";
            }
            catch (Exception)
            {
                embedUrl = "";
            }
            int uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<Pet> pets = await db.Pets.Take(15).ToListAsync();
            UserViewModel userpage = new UserViewModel()
            {
                Pets = await PetMateModel.ToPetsVM(pets),
                Requests = await db.Requests.Where(r => r.UserId == uid).ToListAsync(),
                Donations = PetMateModel.Donations(await db.Sponsorships.Where(s => s.UserId == uid).ToListAsync()),
                CalendarUrl = embedUrl,
            };
            foreach (var req in userpage.Requests)
            {
                req.Shelter = await db.Shelters.FindAsync(req.ShelterId);
                req.Pet = await db.Pets.FindAsync(req.PetId);
                req.User = await db.Users.FindAsync(uid);
            }

            userpage.Pets.Reverse();
            return View(userpage);



        }

        [HttpGet]
        public async Task<IActionResult> Gallery(int page)
        {
            page = page.Equals(0) ? 1 : page;
            var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            User? user = await db.Users.FindAsync(userID);

            List<Pet> pets = await db.Pets.Take(20).ToListAsync();
            pets = pets.Skip((page - 1) * 20).ToList();


            var petCharacteristics = pets.Select(pet => new
            {
                pet.Name,
                pet.Character

            }).ToList();

            string promptData = System.Text.Json.JsonSerializer.Serialize(petCharacteristics);

            var matchedPets = manager.GetGPTResponse($"Take a look at these pets:{promptData}.Order them based on how much the user will like each pet, starting from the most liked ones, to the least liked ones. Use the user's characteristics: [{user.Character}] and the pet's characteristics to order them. Return them in a JSON like format.Include all pets in the sorting, no exception.", true);
            var matchedPets_list = System.Text.Json.JsonSerializer.Deserialize<List<string>>(matchedPets)
            .Select(name => name.Trim().ToLower())
            .ToList();



            if (pets.Count > 1)
            {
                pets = pets.Where(pet => matchedPets_list.Contains(pet.Name.ToLower()))
                        .OrderBy(pet => matchedPets_list.IndexOf(pet.Name.ToLower()))
                        .ToList();
            }

            List<PetVM> photos = await PetMateModel.ToPetsVM(pets);

            ViewBag.TotalPages = Math.Ceiling((double)pets.Count / 20);
            ViewBag.CurrentPage = page; //Page ur currently on

            return View(photos);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1) // Set expiration to a past date
            };
            Response.Cookies.Append(".AspNetCore.Cookies_User", "", cookieOptions);
            return RedirectToAction("Index", "Home");

        }

        //[HttpPost]
        //public async Task<IActionResult> CreateMeeting()
        //{


        //    HttpClient httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    // Generate JWT Token
        //    var jwtToken = GenerateJWTToken(zoomApiKey,apiSecret);

        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        //    string apiUrl = "https://api.zoom.us/v2/users/me/meetings";

        //    object rbody = new
        //    {
        //        topic = "Yes",
        //        type = 2,
        //        start_time = DateTime.Now,
        //        duration = 60,
        //        timezone = "UTC",
        //        settings = new
        //        {
        //            host_video = true,
        //            participant_video = true
        //        }
        //    };

        //    var reqContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(rbody),Encoding.UTF8,"application/json");
        //    var response = await httpClient.PostAsync(apiUrl, reqContent);

        //    var responseContent = await response.Content.ReadAsStringAsync();

        //    return Content(responseContent);
        //}

        public ActionResult Authorize()
        {
            var authorizeUrl = $"https://zoom.us/oauth/authorize?response_type=code&client_id={clientId}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}";
            return Redirect(authorizeUrl);
        }


        public async Task<ActionResult> Callback(string code)
        {
            //<iframe src="https://calendar.google.com/calendar/embed?src=stasi20101%40gmail.com&ctz=Europe%2FSofia" style="border: 0" width="800" height="600" frameborder="0" scrolling="no"></iframe>
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://zoom.us/oauth/token");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));

                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "authorization_code"},
            {"code", code},
            {"redirect_uri", redirectUri}
        });

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return Content("OAuth error: " + responseBody);

                var tokenData = JsonConvert.DeserializeObject<ZoomTokenResponse>(responseBody);

                // Store tokenData.access_token for the current user (cookie, session, or DB)
                TempData["ZoomAccessToken"] = tokenData.access_token;

                return RedirectToAction("CreateMeeting");
            }
        }


        public async Task<ActionResult> CreateMeeting()
        {
            var accessToken = TempData["ZoomAccessToken"]?.ToString();

            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Authorize");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var startTime = DateTime.UtcNow.AddDays(1); // Meeting scheduled for tomorrow

                var meetingData = new
                {
                    topic = "Pet Adoption Interview",
                    type = 2, // Scheduled meeting (not recurring)
                    start_time = startTime.ToString("yyyy-MM-ddTHH:mm:ssZ"), // ISO 8601 format
                    duration = 60, // Duration in minutes
                    timezone = "UTC", // Specify timezone
                    settings = new
                    {
                        join_before_host = false,
                        host_video = true,
                        participant_video = true,
                        mute_upon_entry = true,
                        approval_type = 1,
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(meetingData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.zoom.us/v2/users/me/meetings", content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return Content("Meeting creation failed: " + json);

                //var meeting = JsonConvert.DeserializeObject<ZoomMeetingResponse>(json);

                //ViewBag.JoinUrl = meeting.join_url;
                //ViewBag.StartUrl = meeting.start_url;

                return Content(json);
            }
        }





        [HttpPost]
        public async Task<IActionResult> CancelRequest(int rid)
        {
            string accepted = RequestStatus.Приета.ToString();
            string rejected = RequestStatus.Приета.ToString();

            try
            {
                Request req = await db.Requests.FindAsync(rid);

                if (req.Status == accepted || req.Status == rejected)
                {
                    TempData["msg"] = $"Заявката не може да се отхвърли, тъй като вече е {req.Status.ToLower()}";
                    TempData["msg_type"] = "error";
                    return RedirectToAction("UserHomePage", "User");
                }

                db.Requests.Remove(req);

                await db.SaveChangesAsync();
                TempData["msg"] = "Успешно отхвърлихте заявката за осиновяване !";
                TempData["msg_type"] = "success";
            }
            catch (Exception)
            {
                TempData["msg"] = "Отхвърлянето на заявка за осиновяване се правали. Пробвайте да обновите страницата или опитайте по-късно.";
                TempData["msg_type"] = "error";

                return RedirectToAction("UserHomePage", "User");
            }


            return RedirectToAction("UserHomePage", "User");
        }

        //private static string GenerateJWTToken(string apiKey, string apiSecret)
        //{
        //    var payload = new
        //    {
        //        iss = apiKey,
        //        exp = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds()
        //    };

        //    var header = new { alg = "HS256", typ = "JWT" };
        //    string base64UrlEncodedHeader = Base64UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(header));
        //    string base64UrlEncodedPayload = Base64UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(payload));

        //    string signature = ComputeHMACSHA256($"{base64UrlEncodedHeader}.{base64UrlEncodedPayload}", apiSecret);

        //    return $"{base64UrlEncodedHeader}.{base64UrlEncodedPayload}.{signature}";
        //}

        //private static string Base64UrlEncode(string input)
        //{
        //    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        //    string base64String = Convert.ToBase64String(inputBytes);
        //    return base64String.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        //}

        //private static string ComputeHMACSHA256(string input, string key)
        //{
        //    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        //    using (var hmac = new System.Security.Cryptography.HMACSHA256(keyBytes))
        //    {
        //        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        //        byte[] hashBytes = hmac.ComputeHash(inputBytes);
        //        return Convert.ToBase64String(hashBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        //    }
        //}
    }
}
public class ZoomTokenResponse
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string scope { get; set; }
}
