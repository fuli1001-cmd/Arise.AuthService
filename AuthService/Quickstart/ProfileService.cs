using AuthService.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Quickstart
{
    public class ProfileService : IProfileService
    {
        private UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            Console.WriteLine("--------------GetProfileDataAsync---------------------");
            var user = await _userManager.GetUserAsync(context.Subject);
            if (user != null)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Role, user.Role ?? string.Empty) };
                context.IssuedClaims.AddRange(claims);
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            Console.WriteLine("--------------IsActiveAsync---------------------");
            //var user = await _userManager.GetUserAsync(context.Subject);
            //context.IsActive = (user != null) && user.IsActive;
            context.IsActive = true;
        }
    }
}
