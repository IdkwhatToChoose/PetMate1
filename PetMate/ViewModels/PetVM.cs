namespace PetMate.ViewModels
{
    public class PetVM
    {
        public string Breed { get; set; } = null!;

        public string Size { get; set; } = null!;

        public bool Castrated { get; set; }

        public int Id { get; set; }

        public int ShelterId { get; set; }

        public int? AdopterId { get; set; }

        public string? Character { get; set; }

        public int Age { get; set; }

        public string Name { get; set; } = null!;

        public IFormFile? Image { get; set; }

        public string Answers { get; set; }
    }
}
