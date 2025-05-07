
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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
        public byte[] IncreaseQuality(byte[] image)
        {
            Bitmap bitmap;
            using (Stream stream = new MemoryStream(image))
            {
                bitmap = new Bitmap(stream);
            }
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Set high-quality rendering options
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            }
            using(MemoryStream ms = new MemoryStream()) { 
               bitmap.Save(ms,ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
