using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.Messages.Events
{
    public class UserRegisteredEvent : IEvent
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string MyCode { get; set; }
        public string InvitingUserCode { get; set; }
        public string SecretQuestion { get; set; }
        public string SecretAnswer { get; set; }
    }
}
