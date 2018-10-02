using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class RewardHistory
    {
        public int Id { get; set; }
        public int RewardItemId { get; set; }
        public string ItemName { get; set; }
        public int PointValue { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ItemPrice { get; set; }
        public DateTime ItemCreateDate { get; set; }
        public DateTime HistoryCreateDate { get; set; }
        public bool isDelete { get; set; }
    }
}
