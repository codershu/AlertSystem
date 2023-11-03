using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.Data.Model.Items
{
    public class AlertItem
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string AlertTypeId { get; set; }
        public string Email { get; set; }
        public int CoolDown { get; set; }
        public string ResourceId { get; set; }
        public string Content { get; set; }
        public DateTime? SentOn { get; set; }
        public Guid? ResolvedBy { get; set; }
        public DateTime? ResolvedOn { get; set; }
    }
}
