using Arise.DDD.Messages.Events;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Commands
{
    public class HelperMethods
    {
        public static async Task SendUserRegisteredEventAsync(ApplicationUser user, IMessageSession messageSession, ILogger logger)
        {
            var @event = new UserRegisteredEvent
            {
                Id = user.Id,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Code = user.Code,
                InvitingUserCode = user.InvitingUserCode,
                SecretQuestion = user.SecretQuestion,
                SecretAnswer = user.SecretAnswer
            };
            await messageSession.Publish(@event);
            
            logger.LogInformation("----- Published UserRegisteredEvent: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
        }

        public static string GetIdentityResultErrorString(IdentityResult identityResult)
        {
            var messageBuilder = new StringBuilder();

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                    messageBuilder = messageBuilder.Append(GetCnError(error.Description)).Append("\r\n");
            }

            return messageBuilder.ToString();
        }

        private static string GetCnError(string enError)
        {
            var cnError = enError;

            if (enError.Contains("Incorrect password"))
                cnError = "密码错误";

            return cnError;
        }
    }
}
