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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RegisterPhoneCommandHandler> _logger;

        public RegisterPhoneCommandHandler(UserManager<ApplicationUser> userManager,
            IServiceProvider serviceProvider,
            ILogger<RegisterPhoneCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RegisterPhoneCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.PhoneNumber,
                PhoneNumber = request.PhoneNumber,
                Code = Path.GetRandomFileName().Replace(".", string.Empty).ToUpper()
            };
            var identityResult = await _userManager.CreateAsync(user);

            if (!identityResult.Succeeded)
                throw new ApplicationException(HelperMethods.GetIdentityResultErrorString(identityResult));

            await SendUserRegisteredEventAsync(request.PhoneNumber);

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
