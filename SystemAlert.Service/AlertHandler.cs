using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SystemAlert.Data.Context.Models;
using SystemAlert.Data.Model.Common;
using SystemAlert.Data.Model.Items;
using SystemAlert.IService;

namespace SystemAlert.Service
{
    public class AlertHandler : IAlertHandler
    {
        private readonly ILogger<AlertHandler> _logger;
        private readonly AdminContext _context;
        private readonly IEmailHandler _email;
        private readonly IMapper _mapper;

        public AlertHandler(ILogger<AlertHandler> logger, AdminContext context, IEmailHandler email, IMapper mapper)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._email = email ?? throw new ArgumentNullException(nameof(email));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<bool>> CreateAlertConfiguration(Guid customerId, AlertConfigurationItem request)
        {
            var existingConfiguration = _context.AlertConfiguration.Where(x => x.CustomerId == request.CustomerId && x.AlertTypeId == request.AlertTypeId).FirstOrDefault();
            if (existingConfiguration != null)
                return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE006);

            var alertConfiguration = new AlertConfiguration()
            {
                CustomerId = request.CustomerId,
                AlertTypeId = request.AlertTypeId,
                Email = request.Email,
                CoolDown = request.CoolDown,
                SendTo = request.SendTo,
                CreatedBy = customerId,
                UpdatedBy = customerId
            };
            _context.Add(alertConfiguration);
            await _context.SaveChangesAsync();

            return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS006, true);
        }

        public async Task<Response<bool>> DeleteAlertConfiguration(Guid alertConfigurationId)
        {
            var alertConfiguration = await _context.AlertConfiguration.FirstOrDefaultAsync(x => x.Id == alertConfigurationId);
            if (alertConfiguration == null)
                return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE008);

            _context.Remove(alertConfiguration);
            await _context.SaveChangesAsync();

            return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS008, true);
        }

        public async Task<Response<List<AlertConfigurationItem>>> GetAlertConfigurations(Guid customerId)
        {
            var configurations = await _context.AlertConfiguration.Where(x => x.CustomerId == customerId).ToListAsync();
            var result = _mapper.Map<List<AlertConfigurationItem>>(configurations);

            return Response<List<AlertConfigurationItem>>.Ok(ResponseCode.CodeNumbers.ALTS005, result);
        }

        public async Task<Response<bool>> UpdateAlertConfiguration(Guid customerId, AlertConfigurationItem request)
        {
            var alertConfiguration = await _context.AlertConfiguration.FirstOrDefaultAsync(x => x.CustomerId == request.CustomerId && x.AlertTypeId == request.AlertTypeId);
            if (alertConfiguration == null)
                return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE007);

            alertConfiguration.CoolDown = request.CoolDown;
            alertConfiguration.Email = request.Email;
            alertConfiguration.SendTo = request.SendTo;
            alertConfiguration.UpdatedBy = customerId;

            _context.Update(alertConfiguration);
            await _context.SaveChangesAsync();

            return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS007, true);
        }

        public async Task<Response<List<AlertTypeItem>>> GetAlertTypes()
        {
            var alertTypes = await _context.AlertType.ToArrayAsync();
            var result = _mapper.Map<List<AlertTypeItem>>(alertTypes);

            return Response<List<AlertTypeItem>>.Ok(ResponseCode.CodeNumbers.ALTS011, result);
        }

        public async Task<Response<bool>> ProcessAlertMessage(AlertView alertView)
        {
            var alertTypeConfiguration = _context.AlertType.FirstOrDefault(x => x.Id == alertView.AlertTypeId);
            if (alertTypeConfiguration == null || String.IsNullOrWhiteSpace(alertTypeConfiguration.EmailTemplateId))
                return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS016, false);

            var configuration = _context.AlertConfiguration.FirstOrDefault(x => x.CustomerId == alertView.CustomerId && x.AlertTypeId == alertView.AlertTypeId);
            if (configuration == null)
                return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS016, false);

            if (configuration.SendTo == AlertSendToTypes.Off)
                return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS016, false);

            var customer = _context.Customer.FirstOrDefault(x => x.Id == alertView.CustomerId && x.IsActive);
            if (customer == null)
                return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE018);

            var sendEmail = false;
            var targetAlert = _context.Alert.OrderByDescending(x => x.SentOn).FirstOrDefault(x => x.AlertTypeId == alertView.AlertTypeId && x.CustomerId == alertView.CustomerId && x.ResourceId == alertView.ResourceId && x.ResolvedOn == null);
            if (targetAlert == null)
            {
                sendEmail = true;
                var alert = new Alert()
                {
                    CustomerId = alertView.CustomerId,
                    AlertTypeId = alertView.AlertTypeId,
                    ResourceId = alertView.ResourceId,
                    Content = alertView.Content,
                    CreatedOn = alertView.CreatedOn
                };
                _context.Add(alert);
                await _context.SaveChangesAsync();

                targetAlert = alert;
            }
            else
            {
                sendEmail = targetAlert.SentOn == null || targetAlert.SentOn < DateTime.UtcNow.AddMinutes(-configuration.CoolDown);
            }

            var jsonContent = JsonDocument.Parse(alertView.Content);
            if (jsonContent.RootElement.TryGetProperty("collectionSilenceAlert", out var collectionSilenceAlert) && collectionSilenceAlert.GetBoolean())
            {
                sendEmail = false;
            }

            if (sendEmail)
            {
                var emailResponse = await SendAlertEmail(targetAlert, configuration, alertTypeConfiguration.EmailTemplateId, customer.Name);
                if (emailResponse == null || !emailResponse.IsSuccess)
                    return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE015);

                return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS015, true);
            }
            else
            {
                return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS016, false);
            }
        }

        public async Task<Response<bool>> SendAlertEmail(Alert alert, AlertConfiguration configuration, string emailTemplateId, string customerName)
        {
            var globalConfiguration = _context.AlertConfiguration.FirstOrDefault(x => x.CustomerId == alert.CustomerId && x.AlertTypeId == AlertTypes.Global);
            var globalEmailList = globalConfiguration.Email.Split(";").ToList();
            var customEmailList = configuration.Email.Split(";").ToList();
            var emailList = new List<string>();
            if (configuration.SendTo == AlertSendToTypes.Global)
                emailList = globalEmailList;
            else if (configuration.SendTo == AlertSendToTypes.Custom)
                emailList = customEmailList;
            else
            {
                emailList.AddRange(globalEmailList);
                emailList.AddRange(customEmailList);
            }

            var dynamicalData = new Dictionary<string, string>();
            dynamicalData.Add("CustomerName", customerName);
            dynamicalData.Add("CustomerId", alert.CustomerId.ToString());
            if (!String.IsNullOrWhiteSpace(alert.Content))
            {
                var temp = JsonSerializer.Deserialize<Dictionary<string, object>>(alert.Content, JsonOpts.Web);
                foreach (var item in temp)
                {
                    dynamicalData.Add(char.ToUpper(item.Key[0]) + item.Key.Substring(1), item.Value.ToString());
                }
            }

            var anySent = false;
            foreach (var email in emailList)
            {
                var emailResponse = await _email.SendEmailWithTemplate(email, customerName, emailTemplateId, dynamicalData, null);
                if (emailResponse == null || !emailResponse.IsSuccess)
                    continue;
                else
                    anySent = true;
            }

            if (anySent)
            {
                alert.SentOn = DateTime.UtcNow;

                _context.Update(alert);
                await _context.SaveChangesAsync();

                return Response<bool>.Ok(ResponseCode.CodeNumbers.ALTS015, true);
            }
            return Response<bool>.Error(ResponseCode.CodeNumbers.ALTE015);
        }

        public Task SendAlertAsync(AlertView alert)
        {
            alert.CustomerId = alert.CustomerId == Guid.Empty ? _userContext.CustomerId : alert.CustomerId;
            if (Guid.Empty == alert.CustomerId)
            {
                _logger.LogError("Error sending Alert: AlertTypeId:{0} ResourceId:{1} Content:{2}", alert.AlertTypeId, alert.ResourceId, alert.Content);
                return Task.CompletedTask;
            }
            else
            {
                var queuename = _config["ServiceBus:AlertQueue"];
                return _servicebus.SendMessageToQueueAsync(alert, queuename);
            }
        }
    }
}
