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

namespace AuthService.Application.Commands.RegisterCode
{
    public class RegisterCodeCommandHandler : IRequestHandler<RegisterCodeCommand, bool>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RegisterCodeCommandHandler> _logger;

        private IMessageSession _messageSession;

        public RegisterCodeCommandHandler(ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            IServiceProvider serviceProvider,
            ILogger<RegisterCodeCommandHandler> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> Handle(RegisterCodeCommand request, CancellationToken cancellationToken)
        {
            //var user = _dbContext.Users.SingleOrDefault(u => u.Code.ToLower() == request.Code.ToLower());

            //if (user == null)
            //    throw new ApplicationException("邀请码错误。");

            ////Path.GetRandomFileName()
            //var user = new ApplicationUser
            //{
            //    UserName = Path.GetRandomFileName(),
            //    Code = Path.GetRandomFileName()
            //};
            //var identity = await _userManager.CreateAsync(user, request.Password);

            throw new NotImplementedException();
        }
    }
}
