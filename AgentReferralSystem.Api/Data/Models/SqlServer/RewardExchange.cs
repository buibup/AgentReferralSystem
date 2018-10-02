using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class RewardExchange
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int ItemId { get; set; }
        public int PointUsed { get; set; }
        public DateTime CreateDate { get; set; }
        public bool isDelete { get; set; }
    }
}
