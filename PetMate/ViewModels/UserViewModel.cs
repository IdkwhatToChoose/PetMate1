using PetMate.Model;

namespace PetMate.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string Answers { get; set; }

        public string? Valid { get; set; }

        public ICollection<PetVM> Pets { get; set;}

        public ICollection<Request>? Requests { get; set; }

        public ICollection<Donation>? Donations { get; set; }

        public string? CalendarUrl { get; set; }

        public string? TwofaCode { get; set; }
    }
}
