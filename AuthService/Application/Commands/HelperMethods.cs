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
                UserId = user.Id,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Code = user.Code,
                InvitingUserCode = user.InvitingUserCode
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

            var message = messageBuilder.ToString();
            if (message.EndsWith("\r\n"))
                message = message.Substring(0, message.Length - 2);

            return message;
        }

        private static string GetCnError(string enError)
        {
            var cnError = enError;

            if (enError.Contains("Incorrect password"))
                cnError = "密码错误";
            else if (enError.Contains("Passwords must have at least one digit"))
                cnError = "密码至少包含一位数字";
            else if (enError.Contains("Passwords must be at least 6 characters. Passwords must have at least one lowercase ('a'-'z')."))
                cnError = "密码至少6位，且至少包含一个小写字母('a'-'z')";

            return cnError;
        }
    }
}
