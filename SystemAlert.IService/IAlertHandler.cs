using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.IService
{
    public interface IAlertHandler
    {
        Task<Response<List<AlertConfigurationItem>>> GetAlertConfigurations(Guid customerId);
        Task<Response<bool>> CreateAlertConfiguration(Guid customerId, AlertConfigurationItem request);
        Task<Response<bool>> UpdateAlertConfiguration(Guid customerId, AlertConfigurationItem request);
        Task<Response<bool>> DeleteAlertConfiguration(Guid alertConfigurationId);
        Task<Response<List<AlertTypeItem>>> GetAlertTypes();
        Task<Response<bool>> ProcessAlertMessage(AlertView alert);
        Task SendAlertAsync(AlertView alert);
    }
}
