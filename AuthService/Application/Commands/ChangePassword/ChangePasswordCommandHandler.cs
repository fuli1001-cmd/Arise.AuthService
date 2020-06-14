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

namespace AuthService.Application.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        private readonly IMediator _mediator;

        public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager,
            IMediator mediator,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
                throw new ClientException("操作失败。", new List<string> { $"User {request.UserName} does not exist."});

            var identityResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!identityResult.Succeeded)
                throw new ClientException(HelperMethods.GetIdentityResultErrorString(identityResult));

            return true;
        }
    }
}
