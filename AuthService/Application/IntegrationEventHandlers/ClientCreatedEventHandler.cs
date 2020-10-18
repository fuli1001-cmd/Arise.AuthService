using Arise.DDD.Messages.Events;
using AuthService.Application.Commands.CreateClient;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Application.IntegrationEventHandlers
{
    public class ClientCreatedEventHandler : IHandleMessages<ClientCreatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClientCreatedEventHandler> _logger;

        public ClientCreatedEventHandler(IMediator mediator, ILogger<ClientCreatedEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ClientCreatedEvent message, IMessageHandlerContext context)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{message.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling ClientCreatedEvent: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", message.Id, Program.AppName, message);

                // create client
                var command = new CreateClientCommand
                {
                    ClientId = message.ClientId,
                    ClientName = message.Name,
                    Secret = message.Secret
                };

                await _mediator.Send(command);
            }
        }
    }
}
