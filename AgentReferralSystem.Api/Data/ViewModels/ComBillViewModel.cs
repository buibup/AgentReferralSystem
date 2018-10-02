using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentReferralSystem.Api.Data.Models.SqlServer;

namespace AgentReferralSystem.Api.Data.ViewModels
{
    public class ComBillViewModel
    {
        public int month { get; set; }
        public decimal sumItmValue { get; set; }
        public decimal sumCommiss { get; set; }
        public List<CommissionItem> ItemList { get; set; }
    }
}
