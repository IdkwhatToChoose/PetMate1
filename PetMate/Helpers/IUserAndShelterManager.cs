using PetMate.ViewModels;
using PetMate.Model;
namespace PetMate.Helpers
{
    public interface IUserAndShelterManager
    {
        public User UserRegister(UserViewModel userViewModel);
        public Shelter ShelterRegister(ShelterViewModel shelterViewModel);
    }
}
