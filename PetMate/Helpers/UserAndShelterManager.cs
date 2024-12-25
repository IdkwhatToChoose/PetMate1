namespace PetMate.Helpers
{
  using Microsoft.AspNetCore.Authentication.Cookies;
  using Microsoft.AspNetCore.Authentication;
  using PetMate.Model;
  using PetMate.ViewModels;
  using System.Security.Claims;

  public class UserAndShelterManager : IUserAndShelterManager
  {
        private readonly IWebHostEnvironment _environment;

        public UserAndShelterManager()
        {

        }

        public UserAndShelterManager(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public User UserRegister(UserViewModel uvm)
        {
            User newUser = new User();
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
            newShelter.Type = svm.Type;

            return newShelter;
        }
        public string SetPetPhoto(PetVM pet)
        {
            var path = _environment.WebRootPath;
            IFormFile? image = pet.Image;
            if (image != null && image.Length > 0)
            {
                var filePath = Path.Combine(path, "uploads", image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyToAsync(stream);
                }

                return image.FileName;

            }
            return null;

        }

  }
}
