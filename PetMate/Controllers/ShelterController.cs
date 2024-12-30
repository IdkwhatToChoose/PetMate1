using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using PetMate.Controllers;
using PetMate.Helpers;
using PetMate.Model;
using PetMate.ViewModels;
using System.ClientModel;
using System.Security.Claims;
using System.Xml.Linq;

namespace PetMate.Controllers
{
    public class ShelterController : Controller
    {
        PetMateContext db = new PetMateContext();
        private readonly IUserAndShelterManager usermanager;
        private readonly string questions = "What is the pet's activity level?, 2. How sociable is the pet with people?, 3. How does the pet interact with other animals?, 4. What level of grooming does the pet require?, 5. How vocal is the pet?, 6. Is the pet trained?, 7. Does the pet have special needs or medical requirements?, 8. What type of living environment is best for this pet?, 9. How independent is the pet?, 10. How well does the pet tolerate children?, 11. What type of climate does the pet prefer?, 12. What is the pet's preferred level of interaction?, 13. What is the pet's temperament?";
        private string? apiKey = Environment.GetEnvironmentVariable("OpenAI-API-KEY");
        private string explanation = "Im making a website where characteristics of the registered pet will be displyed in its profile based on the answers on questions";
        private readonly string[] charactersitics = Enum.GetNames(typeof(Characteristics.Characteristics));

        public ShelterController(IUserAndShelterManager _userManager)
        {
            usermanager = _userManager;
        }

        public IActionResult ShelterHomePage()
        {
            return View();
        }
        public IActionResult PetRegistration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PetRegistration(PetVM petVM)
        {
            var shelterID = int.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
            PhotoOfPet photo = new PhotoOfPet();
            Pet newPet = new Pet();

            newPet.Name = petVM.Name;
            newPet.Size = petVM.Size;
            newPet.Age = petVM.Age;
            newPet.Castrated = bool.Parse(petVM.Castrated);
            newPet.Breed = petVM.Breed;
            newPet.ShelterId = shelterID;

            (byte[] imageBytes, string imageName) imageFile = usermanager.SetPhoto(petVM.Image);
            photo.Image = imageFile.imageBytes;
            photo.ImageName = imageFile.imageName;
            newPet.Character = AnalyseAnswers(petVM.Answers);
            try
            {
                  db.Pets.Add(newPet);
                  db.SaveChanges();
                  photo.PetId = newPet.Id;
                  db.PhotoOfPets.Add(photo);
                  db.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);

            }

            return RedirectToAction("Index", "Home");
        }
        public string AnalyseAnswers(string answers)
        {
            string answer = GetGPTResponse($"{explanation}.Return any of these characteristics for the user: {string.Join(", ", charactersitics)}, based on these questions: {questions} and their answers: {answers}. " + $"The last question is multiple choice, so the numbers 13 and above are the answers to the question.Also only type the chosen characteristics, nothing else.");
            string[] userChar = answer.Split(':');
            string result = userChar[0].Replace("-", ", ");
            return result;
        }
        
        private string GetGPTResponse(string prompt)
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
