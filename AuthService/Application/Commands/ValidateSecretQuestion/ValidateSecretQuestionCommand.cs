using AuthService.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.ResetPassword
{
    public class ValidateSecretQuestionCommand : IRequest<ApplicationUser>
    {
        public string UserName { get; set; }

        public string SecretQuestion { get; set; }

        public string SecretAnswer { get; set; }
    }
}
