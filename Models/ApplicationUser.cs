using Microsoft.AspNetCore.Identity;

namespace BookingRoomCampus.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}