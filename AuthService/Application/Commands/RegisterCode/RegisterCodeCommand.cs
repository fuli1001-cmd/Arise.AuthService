using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.RegisterCode
{
    [DataContract]
    public class RegisterCodeCommand : IRequest<bool>
    {
        [DataMember]
        [Required]
        public string Code { get; set; }
    }
}
