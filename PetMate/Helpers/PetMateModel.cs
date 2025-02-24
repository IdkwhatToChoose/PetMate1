using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetMate.Model;
using PetMate.ViewModels;

namespace PetMate.Helpers
{
    public static class PetMateModel
    {
        static PetMateContext db = new PetMateContext();

        public static async Task<List<PetVM>> ToPetsVM(List<Pet> pets)
        {
            List<PetVM> petvm = new List<PetVM>();
            foreach (Pet pet in pets)
            {
                var imageID = db.PhotoOfPets.FirstOrDefault(x => x.PetId == pet.Id).Id;
                
                PetVM vm = new PetVM
                {
                    Id = pet.Id,
                    Name = pet.Name,
                    Age=pet.Age.ToString(),
                    Image = await GetPhoto(imageID),
                    Castrated = pet.Castrated.ToString(),
                    Breed = pet.Breed,
                    Gender = pet.Gender,
                    Size = pet.Size,
                    Character = pet.Character,
                    ShelterId = pet.ShelterId,
                    AdopterId = pet.AdopterId
                };
                petvm.Add(vm);
            }
            return petvm;
        }
        public static async Task<PetVM> ToPetVM(int pid)
        {
                Pet pet=await db.Pets.FindAsync(pid);

                var imageID = db.PhotoOfPets.FirstOrDefault(x => x.PetId == pet.Id).Id;

                PetVM vm = new PetVM
                {
                    Id = pet.Id,
                    Name = pet.Name,
                    Age = pet.Age.ToString(),
                    Image = await GetPhoto(imageID),
                    Castrated = pet.Castrated.ToString(),
                    Breed = pet.Breed,
                    Gender = pet.Gender,
                    Size = pet.Size,
                    Character = pet.Character,
                    ShelterId = pet.ShelterId,
                    AdopterId = pet.AdopterId
                };
            return vm;
        }
        public static async Task<IFormFile> GetPhoto(int id)
        {
            var image = await db.PhotoOfPets.FindAsync(id);
            return ByteToFormFile(image.Image, image.ImageName, "image/jpeg");

        }

        public static async Task<User> GetUserById(int uid)
        {
            return await db.Users.FindAsync(uid);
        }

        public static IFormFile ByteToFormFile(byte[] fileBytes, string fileName, string contentType)
        {
            // Create a MemoryStream from the byte array
            var stream = new MemoryStream(fileBytes);

            // Create an IFormFile instance using FormFile
            var formFile = new FormFile(stream, 0, fileBytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return formFile;
        }
    }
}
