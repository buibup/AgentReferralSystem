using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class RewardItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int PointValue { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ItemPrice { get; set; }
        public DateTime CreateDate { get; set; }
        public bool isDelete { get; set; }
    }
}
