using AuthService.Data;
using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.RegisterPhone
{
    public class RegisterPhoneCommandHandler : IRequestHandler<RegisterPhoneCommand, bool>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RegisterPhoneCommandHandler> _logger;

        public RegisterPhoneCommandHandler(ApplicationDbContext dbContext, 
            UserManager<ApplicationUser> userManager,
            IServiceProvider serviceProvider,
            ILogger<RegisterPhoneCommandHandler> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RegisterPhoneCommand request, CancellationToken cancellationToken)
        {
            IdentityResult identityResult = null;
            var user = _dbContext.Users.SingleOrDefault(u => u.PhoneNumber == request.PhoneNumber);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = request.PhoneNumber,
                    PhoneNumber = request.PhoneNumber,
                    Code = Path.GetRandomFileName().Replace(".", string.Empty).ToUpper()
                };

                identityResult = await _userManager.CreateAsync(user, request.VerifyCode);

                if (identityResult.Succeeded)
                    await SendUserRegisteredEventAsync(request.PhoneNumber);
            }
            else
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                identityResult = await _userManager.ResetPasswordAsync(user, token, request.VerifyCode);
            }

            if (!identityResult.Succeeded)
                throw new ApplicationException(HelperMethods.GetIdentityResultErrorString(identityResult));

            return true;
        }

        private async Task SendUserRegisteredEventAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await HelperMethods.SendUserRegisteredEventAsync(user, messageSession, _logger);
        }
    }
}
