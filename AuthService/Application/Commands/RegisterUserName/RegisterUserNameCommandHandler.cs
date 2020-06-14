using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Messages.Events;
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

namespace AuthService.Application.Commands.RegisterUserName
{
    public class RegisterUserNameCommandHandler : IRequestHandler<RegisterUserNameCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RegisterUserNameCommandHandler> _logger;

        public RegisterUserNameCommandHandler(UserManager<ApplicationUser> userManager,
            IServiceProvider serviceProvider,
            ILogger<RegisterUserNameCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RegisterUserNameCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                SecretQuestion = request.SecretQuestion,
                SecretAnswer = request.SecretAnswer,
                Code = Path.GetRandomFileName().Replace(".", string.Empty).ToUpper()
            };
            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (!identityResult.Succeeded)
                throw new ClientException(HelperMethods.GetIdentityResultErrorString(identityResult));

            await SendUserRegisteredEventAsync(request.UserName);

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
