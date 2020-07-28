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
        private readonly SmsSettings _smsSettings;
        private readonly ILogger<AliSmsService> _logger;

        public AliSmsService(IOptionsSnapshot<SmsSettings> smsOptions, ILogger<AliSmsService> logger)
        {
            _smsSettings = smsOptions?.Value ?? throw new ArgumentNullException(nameof(smsOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string SendSms(string phonenumber)
        {
            var code = new Random().Next(1111, 9999).ToString();

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", _smsSettings.Key, _smsSettings.Secrect);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            CommonRequest request = new CommonRequest
            {
                Method = MethodType.POST,
                Domain = _smsSettings.Domain,
                Version = _smsSettings.Version,
                Action = _smsSettings.Action
            };

            request.AddQueryParameters("PhoneNumbers", phonenumber);
            request.AddQueryParameters("SignName", _smsSettings.SignName);
            request.AddQueryParameters("TemplateCode", _smsSettings.TemplateCode);
            request.AddQueryParameters("TemplateParam", "{\"code\":\"" + code + "\"}");

            _logger.LogInformation("SmsSettings: {@SmsSettings}", _smsSettings);
            _logger.LogInformation("SendSms request: {@request}", request);

            try
            {
                CommonResponse response = client.GetCommonResponse(request);
                _logger.LogInformation("SendSms response: {response}", response.Data);
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
