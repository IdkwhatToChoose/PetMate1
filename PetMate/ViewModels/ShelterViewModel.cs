namespace PetMate.ViewModels
{
    public class ShelterViewModel
    {
        public long PetCount { get; set; }

        public int Id { get; set; }

        public string Address { get; set; } = null!;

        public int Type { get; set; }

        public int WorkingTime { get; set; }

        public int VisitorsTime { get; set; }

        public string? ShelterName { get; set; }

        public string ShelterPassword { get; set; } = null!;
    }
}
