using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //[PersonalData]
        //public string Nickname { get; set; }

        //[PersonalData]
        //public string Avatar { get; set; }

        //[PersonalData]
        //public int Points { get; set; }
    }
}
