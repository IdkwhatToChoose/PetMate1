using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetMate.Model;
using PetMate.ViewModels;
using System.Collections;

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

        public static ICollection<Donation> Donations(List<Sponsorship> sponsorships)
        {
            var donations = new List<Donation>();
            foreach(var sponsor in sponsorships)
            {
                donations.Add(new Donation(sponsor));
            }
            return donations;
        }

        public static async Task<PetVM> ToPetVM(int pid)
        {
                Pet pet=await db.Pets.FindAsync(pid);

                var imageID = db.PhotoOfPets.FirstOrDefault(x => x.PetId == pet.Id).Id;
                var petphotos = await db.PhotoOfPets.Where(p=>p.PetId == pid).ToListAsync();

                PetVM vm = new PetVM
                {
                    Id = pet.Id,
                    Name = pet.Name,
                    Age = pet.Age.ToString(),
                    Image = await GetPhoto(imageID),
                    Images = await ToListFormFile(petphotos),
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

        public static async Task<List<IFormFile>> ToListFormFile(List<PhotoOfPet> photos)
        {
            List<IFormFile> files = new List<IFormFile>();
            
            foreach (PhotoOfPet photo in photos)
            {
                var file = ByteToFormFile(photo.Image, photo.ImageName, "image/jpeg");
                files.Add(file);
            }
            return files;
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
