namespace PetMate.Helpers
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication;
    using PetMate.Model;
    using PetMate.ViewModels;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc;

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
        public (byte[] imageBytes,string imageName) SetPhoto(IFormFile imageFile)
        {

            if (imageFile == null || imageFile.Length == 0)
                return (null,null);

            using (var memoryStream = new MemoryStream())
            {
                imageFile.CopyTo(memoryStream);
                return (memoryStream.ToArray(), imageFile.FileName);
            }
        }



    }
}
