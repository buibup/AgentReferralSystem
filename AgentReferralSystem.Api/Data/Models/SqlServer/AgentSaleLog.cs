using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class AgentSaleLog
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public decimal AnnualSale { get; set; }
        public decimal AnnualCommission { get; set; }
        public int RewardPoint { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
