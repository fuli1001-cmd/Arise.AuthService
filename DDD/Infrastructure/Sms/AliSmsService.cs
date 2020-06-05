using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.Infrastructure.Sms
{
    public class AliSmsService : ISmsService
    {
        private readonly IOptionsSnapshot<SmsSettings> _smsSettings;
        private readonly ILogger<AliSmsService> _logger;

        public AliSmsService(IOptionsSnapshot<SmsSettings> redisSettings, ILogger<AliSmsService> logger)
        {
            _smsSettings = redisSettings ?? throw new ArgumentNullException(nameof(redisSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string SendSms(string phonenumber)
        {
            var code = new Random().Next(1111, 9999).ToString();
            var msg = string.Format("您的验证码是:{0}", code);

            IClientProfile profile = DefaultProfile.GetProfile("ap-southeast-1", _smsSettings.Value.Key, _smsSettings.Value.Secrect);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            CommonRequest request = new CommonRequest
            {
                Method = MethodType.POST,
                Domain = _smsSettings.Value.Domain,
                Version = _smsSettings.Value.Version,
                Action = _smsSettings.Value.Action
            };

            request.AddQueryParameters("To", phonenumber);
            request.AddQueryParameters("Message", msg);
            try
            {
                CommonResponse response = client.GetCommonResponse(request);
                _logger.LogInformation(response.Data);
            }
            catch (ServerException ex)
            {
                _logger.LogError("SendSms ServerException: {@SendSmsException}", ex);
            }
            catch (ClientException ex)
            {
                _logger.LogError("SendSms ClientException: {@SendSmsException}", ex);
            }

            return code;
        }
    }
}
