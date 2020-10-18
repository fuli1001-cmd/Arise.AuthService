using Arise.DDD.Messages.Events;
using AuthService.Data;
using AuthService.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.CreateClient
{
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, bool>
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly ILogger<CreateClientCommandHandler> _logger;

        public CreateClientCommandHandler(ConfigurationDbContext configurationDbContext, ILogger<CreateClientCommandHandler> logger)
        {
            _configurationDbContext = configurationDbContext ?? throw new ArgumentNullException(nameof(configurationDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var lifetime = 604800;

            var client = new Client
            {
                ClientId = request.ClientId,
                ClientName = request.ClientName,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret(request.Secret.Sha256()) },
                AccessTokenLifetime = lifetime,
                IdentityTokenLifetime = lifetime,
                AllowedScopes = new List<string> { "Recharge.API" }
            };

            _configurationDbContext.Clients.Add(client.ToEntity());

            await _configurationDbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
