using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using IdentityServer4.Services;

namespace AuthService.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return await OnPost();
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            var logoutId = this.Request.Query["logoutId"].ToString();

            return Redirect($"~/Account/Logout?logoutId={logoutId}");

            //if (returnUrl != null)
            //{
            //    return LocalRedirect(returnUrl);
            //}
            //else if (!string.IsNullOrEmpty(logoutId))
            //{
            //    var logoutContext = await _interaction.GetLogoutContextAsync(logoutId);
            //    returnUrl = logoutContext.PostLogoutRedirectUri;

            //    if (!string.IsNullOrEmpty(returnUrl))
            //    {
            //        return Redirect(returnUrl);
            //    }
            //    else
            //    {
            //        return Page();
            //    }
            //}
            //else
            //{
            //    return Page();
            //}
        }
    }
}
