using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace PetMate.ViewModels
{
    public class SuccessStatus
    {

        public string Header { get; set; } = null!;

        public string Message { get; set; } = null!;

        public OType Operation { get; set; }

    }

    public enum OType
    {
        Email,
        Donation
    }
}
