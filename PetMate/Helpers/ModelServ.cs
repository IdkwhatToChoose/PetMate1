using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetMate.Model;
using PetMate.ViewModels;

namespace PetMate.Helpers
{
    public  class ModelServ
    {
         PetMateContext db = new PetMateContext();
        private readonly IFileManager filemanager;

        public ModelServ(IFileManager _file)
        {
            filemanager = _file;
        }

        public async Task<List<PetVM>> ToPetVM(List<Pet> pets)
        {
            List<PetVM> petvm = new List<PetVM>();
            foreach (Pet pet in pets)
            {
                var imageID = db.PhotoOfPets.FirstOrDefault(x => x.PetId == pet.Id).Id;
                PetVM vm = new PetVM
                {
                    Id = pet.Id,
                    Name = pet.Name,
                    Image = await GetPhoto(imageID),
                    Castrated = pet.Castrated.ToString(),
                    Breed = pet.Breed,
                    Size = pet.Size,
                    Character = pet.Character,
                    ShelterId = pet.ShelterId,
                    AdopterId = pet.AdopterId
                };
                petvm.Add(vm);
            }
            return petvm;
        }
        public async Task<IFormFile> GetPhoto(int id)
        {

            var image = await db.PhotoOfPets.FindAsync(id);
            return filemanager.ByteToFormFile(image.Image, image.ImageName, "image/jpeg");

        }
        
    }
}
