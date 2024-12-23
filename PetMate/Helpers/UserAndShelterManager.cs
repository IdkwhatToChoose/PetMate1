namespace PetMate.Helpers

{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication;
    using PetMate.Model;
    using PetMate.ViewModels;
    using System.Security.Claims;

    public class UserAndShelterManager : IUserAndShelterManager
    {
        public User UserRegister(UserViewModel uvm)
        {
            User newUser=new User();
            newUser.Email = uvm.Email;
            newUser.Username = uvm.Username;
            uvm.Password = BCrypt.Net.BCrypt.HashPassword(uvm.Password);
            newUser.Password = uvm.Password;
            return newUser;
        }
         public Shelter ShelterRegister(ShelterViewModel svm)
        {

            Shelter newShelter = new Shelter();
            newShelter.Address = svm.Address;
            newShelter.ShelterName = svm.ShelterName;

            svm.ShelterPassword = BCrypt.Net.BCrypt.HashPassword(svm.ShelterPassword);
            newShelter.ShelterPassword = svm.ShelterPassword;
            newShelter.WorkingTime = svm.WorkingTime;
            newShelter.VisitorsTime = svm.VisitorsTime;
            newShelter.PetCount = svm.PetCount;
            newShelter.Type= svm.Type;

            return newShelter;
        }
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
        public async Task SetShelterCookie(HttpContext httpContext, int id, string email)
        {
            // Create user claims
            var claims = new List<Claim>
        {
             new Claim(ClaimTypes.Sid, id.ToString()),
             new Claim(ClaimTypes.Role, "Shelter"),
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

    }
}
