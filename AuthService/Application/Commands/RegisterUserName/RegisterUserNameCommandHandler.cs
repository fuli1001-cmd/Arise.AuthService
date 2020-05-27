using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RegisterUserNameCommandHandler> _logger;

        public RegisterUserNameCommandHandler(UserManager<ApplicationUser> userManager,
            ILogger<RegisterUserNameCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
            return await _userManager.CreateAsync(user, request.Password);
        }
    }
}
