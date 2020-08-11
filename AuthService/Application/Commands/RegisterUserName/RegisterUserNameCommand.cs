using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.RegisterUserName
{
    public class RegisterUserNameCommand : IRequest<bool>
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string SecretQuestion { get; set; }

        public string SecretAnswer { get; set; }

    }
}
