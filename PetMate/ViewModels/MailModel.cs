namespace PetMate.ViewModels
{
    public class MailModel
    {
        public string? Name { get; set; }
        public string? SenderEmail { get; set; }
        public string Subject { get; set; } = null!;
        public string Message { get; set;} = null!;
        public string? RevieverEmail { get; set; }
    }
}
