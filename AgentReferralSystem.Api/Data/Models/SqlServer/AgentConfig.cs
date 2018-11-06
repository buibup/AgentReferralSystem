using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class AgentConfig
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public decimal CommissionAmount { get; set; }
        public string AmountType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
    }
}
