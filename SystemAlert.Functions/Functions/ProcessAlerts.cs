using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonOpts = Common.Json.SerializerDefaults;

namespace SystemAlert.Functions.Functions
{
    public class ProcessAlerts
    {
        private readonly ILogger<ProcessAlerts> _logger;
        private readonly IAlertHandler _handler;

        public ProcessAlerts(ILogger<ProcessAlerts> logger, IAlertHandler handler)
        {
            this._logger = logger;
            this._handler = handler;
        }

        [FunctionName(nameof(ProcessAlerts))]
        public async Task Run([ServiceBusTrigger("%AlertQueue%", Connection = "SERVICE_BUS_CONNECTION")] Azure.Messaging.ServiceBus.ServiceBusReceivedMessage message)
        {
            try
            {
                var alertView = JsonSerializer.Deserialize<AlertView>(Encoding.UTF8.GetString(message.Body), JsonOpts.Web);
                _ = await _handler.ProcessAlertMessage(alertView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
