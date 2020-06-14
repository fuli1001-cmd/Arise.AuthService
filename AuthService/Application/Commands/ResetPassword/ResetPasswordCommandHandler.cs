using Arise.DDD.Domain.Exceptions;
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
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;
        private readonly IMediator _mediator;

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager,
            IMediator mediator,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var validateSecretQuestionCommand = new ValidateSecretQuestionCommand
            {
                UserName = request.UserName,
                SecretAnswer = request.SecretAnswer,
                SecretQuestion = request.SecretQuestion
            };
            var user = await _mediator.Send(validateSecretQuestionCommand);

            if (user == null)
                throw new ClientException("密保问题验证失败。");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var identityResult = await _userManager.ResetPasswordAsync(user, token, request.Password);

            if (!identityResult.Succeeded)
                throw new ClientException(HelperMethods.GetIdentityResultErrorString(identityResult));

            return true;
        }
    }
}
