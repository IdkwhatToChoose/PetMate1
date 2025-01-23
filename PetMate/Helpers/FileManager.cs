namespace PetMate.Helpers
{
    public class FileManager:IFileManager
    {
        public IFormFile ByteToFormFile(byte[] fileBytes, string fileName, string contentType)
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
