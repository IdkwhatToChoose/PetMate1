namespace PetMate.Helpers
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication;
    using PetMate.Model;
    using PetMate.ViewModels;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc;
    using OpenAI.Chat;
    using System.ClientModel;

    public class UserAndShelterManager : IUserAndShelterManager
    {
        private string? apiKey = Environment.GetEnvironmentVariable("OpenAI-API-KEY");
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
        public string GetGPTResponse(string prompt)
        {
            string model = "gpt-3.5-turbo"; // Specify the model (e.g., gpt-4)
            ChatClient chatClient = new ChatClient(model, apiKey);

            // Create messages using the specific message types
            List<ChatMessage> messages = new List<ChatMessage>
            {
                 ChatMessage.CreateSystemMessage("You are a helpful assistant."),
                 ChatMessage.CreateUserMessage(prompt)
            };

            try
            {
                // Send chat request (synchronous)
                ClientResult<ChatCompletion> result = chatClient.CompleteChat(messages);

                if (result?.Value != null)
                {
                    // Access the response content directly through the flattened property
                    return result.Value.Content[0].Text;
                }
                else
                {
                    return "Error: The result value is null or the operation was unsuccessful.";
                }
            }
            catch (Exception ex)
            {
                return "An error occurred: " + ex.Message;
            }
        }


    }
}
