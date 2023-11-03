using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SystemAlert.Data.Model.Common;
using SystemAlert.IService;

namespace SystemAlert.Service
{
    public class EmailHandler : IEmailHandler
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private SendGridClient _client;
        private readonly string _alertEmail;
        private readonly string _alertEmailName;

        public EmailHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["McitSendGridAppSecret"];
            _client = new SendGridClient(_apiKey);
            _alertEmail = _configuration["Alert:SystemAlertEmail"];
            _alertEmailName = _configuration["Alert:SystemAlertEmailName"];
        }

        public async Task<Response<bool>> SendEmail(string email, string subject, string body)
        {
            try
            {
                var message = MailHelper.CreateSingleEmail(
                    new EmailAddress(_alertEmail, _alertEmailName),
                    new EmailAddress(email),
                    subject,
                    body,
                    null);

                var response = await _client.SendEmailAsync(message);
                if (!response.IsSuccessStatusCode)
                    return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE017);

                return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE015, ex.Message);
            }
        }

        public async Task<Response<bool>> SendEmailWithTemplate(string email, string name, string templateId, Dictionary<string, string> dynamicTemplateData = null, Dictionary<string, string> attachments = null)
        {
            try
            {
                var message = MailHelper.CreateSingleTemplateEmail(
                    new EmailAddress(_alertEmail, _alertEmailName),
                    new EmailAddress(email, name),
                    templateId,
                    dynamicTemplateData);

                if (attachments != null)
                {
                    foreach (var filename in attachments.Keys)
                    {
                        message.AddAttachment(filename, attachments[filename]);
                    }
                }

                var response = await _client.SendEmailAsync(message);
                if (!response.IsSuccessStatusCode)
                    return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE017);

                return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS017, true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE015, ex.Message);
            }
        }
    }
}
