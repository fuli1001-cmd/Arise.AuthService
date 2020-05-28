using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.ValidateUserName
{
    public class ValidateUserNameCommand : IRequest<bool>
    {
        public string UserName { get; set; }
    }
}
