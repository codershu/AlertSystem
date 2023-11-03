using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.IService
{
    public interface IEmailHandler
    {
        Task<Response<bool>> SendEmailWithTemplate(string email, string name, string templateId, Dictionary<string, string> dynamicTemplateData = null, Dictionary<string, string> attachments = null);
        Task<Response<bool>> SendEmail(string email, string subject, string body);
    }
}
