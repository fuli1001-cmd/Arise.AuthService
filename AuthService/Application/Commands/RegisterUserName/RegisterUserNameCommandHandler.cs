using Arise.Messages.Events;
using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.RegisterUserName
{
    public class RegisterUserNameCommandHandler : IRequestHandler<RegisterUserNameCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RegisterUserNameCommandHandler> _logger;

        private IMessageSession _messageSession;

        public RegisterUserNameCommandHandler(UserManager<ApplicationUser> userManager,
            IServiceProvider serviceProvider,
            ILogger<RegisterUserNameCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IdentityResult> Handle(RegisterUserNameCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                SecretQuestion = request.SecretQuestion,
                SecretAnswer = request.SecretAnswer
            };
            var identity = await _userManager.CreateAsync(user, request.Password);

            await SendUserRegisteredEventAsync(request.UserName);

            return identity;
        }

        private async Task SendUserRegisteredEventAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var @event = new UserRegisteredEvent 
            { 
                Id = user.Id,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                MyCode = user.MyCode,
                InvitingUserCode = user.InvitingUserCode,
                SecretQuestion = user.SecretQuestion,
                SecretAnswer = user.SecretAnswer
            };
            _messageSession = (IMessageSession)_serviceProvider.GetService(typeof(IMessageSession));
            await _messageSession.Publish(@event);
            _logger.LogInformation("----- Published UserRegisteredEvent: from {AppName} - ({@IntegrationEvent})", Program.AppName, @event);
        }
    }
}
