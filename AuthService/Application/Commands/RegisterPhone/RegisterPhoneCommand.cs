using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.RegisterPhone
{
    [DataContract]
    public class RegisterPhoneCommand : IRequest<bool>
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [DataMember]
        [Required]
        public string Code { get; set; }
    }
}
