using Microsoft.AspNetCore.Mvc;

namespace PetMate.Helpers
{
    public interface IFileManager
    {
        public IFormFile ByteToFormFile(byte[] fileBytes, string fileName, string contentType);


        public async Task<string> DisplayImage(IFormFile imageFile)
        {
          string base64Image = string.Empty;

          if (imageFile != null){
            // Convert IFormFile to a Base64 string
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                base64Image = Convert.ToBase64String(imageBytes);
            }

          }

            // Pass the base64 string to the view via ViewBag or model
            return base64Image;
    }

}
}
