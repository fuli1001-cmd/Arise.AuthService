using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        public string UserName { get; set; }

        public string SecretQuestion { get; set; }

        public string SecretAnswer { get; set; }

        public string Password { get; set; }
    }
}
