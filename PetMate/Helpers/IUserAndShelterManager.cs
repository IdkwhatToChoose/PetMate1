using PetMate.ViewModels;
using PetMate.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PetMate.Helpers
{
    public interface IUserAndShelterManager
    {
        public User UserRegister(UserViewModel userViewModel);
        public Shelter ShelterRegister(ShelterViewModel shelterViewModel);
        public async Task SetUserCookie(HttpContext httpContext, int id, string email)
        {
            // Create user claims
            var claims = new List<Claim>
        {
             new Claim(ClaimTypes.Sid, id.ToString()),
             new Claim(ClaimTypes.Role, "User"),
             new Claim(ClaimTypes.Email,email),
            // Add roles if needed
        };

            // Create the identity and principal
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
                });
        }
        public async Task SetShelterCookie(HttpContext httpContext, int id)
        {
            // Create user claims
            var claims = new List<Claim>
        {
             new Claim(ClaimTypes.Sid, id.ToString()),
             new Claim(ClaimTypes.Role, "Shelter"),
            // Add roles if needed
        };
            //using (StreamWriter writer = System.IO.File.CreateText($"log-{DateTime.Now.ToLongTimeString()}.txt".Replace(":", "_").Replace(" ", "_")))
            //{
            //    foreach (var claim in claims)
            //    {
            //        await writer.WriteLineAsync($"{claim} - {claim.Value}");
            //    }
            //}

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
                });
        }

        public (byte[] imageBytes, string imageName) SetPhoto(IFormFile imageFile);
        public string GetGPTResponse(string prompt, bool strict_match);

    }
}
