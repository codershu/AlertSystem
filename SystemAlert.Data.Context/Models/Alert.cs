using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.Data.Context.Models
{
    public class Alert
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string AlertTypeId { get; set; }
        public string ResourceId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? SentOn { get; set; }
        public Guid? ResolvedBy { get; set; }
        public DateTime? ResolvedOn { get; set; }
    }
}
