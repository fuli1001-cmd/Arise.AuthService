using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.ResetPassword
{
    public class ValidateSecretQuestionCommandHandler : IRequestHandler<ValidateSecretQuestionCommand, ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ValidateSecretQuestionCommandHandler> _logger;

        public ValidateSecretQuestionCommandHandler(UserManager<ApplicationUser> userManager,
            ILogger<ValidateSecretQuestionCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApplicationUser> Handle(ValidateSecretQuestionCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user != null && user.SecretAnswer == request.SecretAnswer && user.SecretQuestion == request.SecretQuestion)
                return user;

            return null;
        }
    }
}
