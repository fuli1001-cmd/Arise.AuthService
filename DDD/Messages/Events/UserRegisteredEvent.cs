using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.Messages.Events
{
    public class UserRegisteredEvent : BaseEvent
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
        public string InvitingUserCode { get; set; }
    }
}
