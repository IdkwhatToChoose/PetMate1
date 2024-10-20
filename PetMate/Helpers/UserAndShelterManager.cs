namespace PetMate.Helpers

{
    using PetMate.Model;
    using PetMate.ViewModels;

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

    }
}
