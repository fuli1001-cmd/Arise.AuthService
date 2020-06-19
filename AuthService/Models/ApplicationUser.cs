using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string Code { get; set; }

        [PersonalData]
        public string InvitingUserCode { get; set; }

        [PersonalData]
        public string SecretQuestion { get; set; }

        [PersonalData]
        public string SecretAnswer { get; set; }

        [PersonalData]
        public string Role { get; set; }
    }
}
