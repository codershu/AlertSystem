using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.Data.Context.Models
{
    public class AlertType
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string ConfigurationTitle { get; set; }
        public string ConfigurationSubTitle { get; set; }
        public string EmailTemplateId { get; set; }
        public string Group { get; set; }
    }
}
