using Microsoft.AspNetCore.Mvc;
using PetMate.Model;
using System.Security.Claims;

namespace PetMate.Controllers
{
    public class Meetings : Controller
    {
        private PetMateContext _db = new PetMateContext();

        public async Task Add(VirtualMeeting meeting)
        {
            int sid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            meeting.ShelterId = sid;

            await _db.VirtualMeetings.AddAsync(meeting);
            await _db.SaveChangesAsync();
         }
    }
}
