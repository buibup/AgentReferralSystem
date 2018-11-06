using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class AgentRewardLog
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int RewardPoint { get; set; }
        public decimal CommissionAmount { get; set; }
        public DateTime ProcessDate { get; set; }
        public int ProcessMonth { get; set; }
        public int ProcessYear { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
