namespace PetMate.ViewModels
{
    public class ShelterViewModel
    {
        public long PetCount { get; set; }

        public int Id { get; set; }

        public string Address { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string WorkingTime { get; set; } = null!;

        public string VisitorsTime { get; set; } = null!;

        public string? ShelterName { get; set; }

        public string ShelterPassword { get; set; } = null!;
    }
}
