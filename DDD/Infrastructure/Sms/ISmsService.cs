using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.Infrastructure.Sms
{
    public interface ISmsService
    {
        string SendSms(string Phonenumber);
    }
}
