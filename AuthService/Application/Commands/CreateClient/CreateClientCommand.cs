using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.CreateClient
{
    public class CreateClientCommand : IRequest<bool>
    {
        // 创建客户的用户id
        public string ClientId { get; set; }

        // 客户id
        public string ClientName { get; set; }

        public string Secret { get; set; }

        public string AppDomain { get; set; }
    }
}
