using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetMate.ViewModels
{
    public class PetVM
    {
        public string Breed { get; set; } = null!;

        public string Size { get; set; } = null!;

        public string? Castrated { get; set; }
        public string? Gender { get; set; }
        
        public int Id { get; set; }

        public int ShelterId { get; set; }

        public int? AdopterId { get; set; }

        public string? Character { get; set; }

        public string? Age { get; set; }

        public string Name { get; set; } = null!;

        public IFormFile? Image { get; set; }

        public string Answers { get; set; }
    }
    public static class SelectOptionsPet
    {
      public static List<SelectListItem> genders = new List<SelectListItem>
        {
            new SelectListItem { Value = "Male", Text = "Male" },
            new SelectListItem { Value = "Female", Text = "Female" },
        };
       public static List<SelectListItem> pet_sizes = new List<SelectListItem>
        {
            new SelectListItem { Value = "Big", Text = "Big" },
            new SelectListItem { Value = "Average", Text = "Average" },
            new SelectListItem { Value = "Small", Text = "Small"}
        };
    }
}
