using Arise.DDD.Domain.Exceptions;
using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.ValidateUserName
{
    public class ValidateUserNameCommandHandler : IRequestHandler<ValidateUserNameCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ValidateUserNameCommandHandler> _logger;

        public ValidateUserNameCommandHandler(UserManager<ApplicationUser> userManager,
            ILogger<ValidateUserNameCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ValidateUserNameCommand request, CancellationToken cancellationToken)
        {
            Regex regexChars = new Regex("[a-zA-Z]");
            Regex regexNumbers = new Regex("[0-9]");
            if (!regexChars.IsMatch(request.UserName) || !regexNumbers.IsMatch(request.UserName) || request.UserName.Length < 6 || request.UserName.Length > 16)
                throw new ClientException("用户名须包含字母和数字，且至少为6位，最多为16位");

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
                throw new ClientException("用户名已存在");

            return true;
        }
    }
}
