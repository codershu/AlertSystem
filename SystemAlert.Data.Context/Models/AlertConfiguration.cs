using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.Data.Context.Models
{
    public class AlertConfiguration
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string AlertTypeId { get; set; }
        public string SendTo { get; set; }
        public string Email { get; set; }
        public int CoolDown { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
