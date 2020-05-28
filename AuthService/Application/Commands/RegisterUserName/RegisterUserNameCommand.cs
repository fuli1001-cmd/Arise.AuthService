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
    [DataContract]
    public class RegisterUserNameCommand : IRequest<bool>
    {
        [DataMember]
        [Required]
        public string UserName { get; set; }

        [DataMember]
        [Required]
        public string Password { get; set; }

        [DataMember]
        [Required]
        public string SecretQuestion { get; set; }

        [DataMember]
        [Required]
        public string SecretAnswer { get; set; }

    }
}
