using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.Messages.Events
{
    public class ClientCreatedEvent : BaseEvent
    {
        public string ClientId { get; set; }

        public string Secret { get; set; }

        public string Name { get; set; }
    }
}
