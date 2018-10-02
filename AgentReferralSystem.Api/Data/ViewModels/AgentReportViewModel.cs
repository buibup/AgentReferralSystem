using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.ViewModels
{
    public class AgentReportViewModel
    {
        public int year { get; set;  }
        public decimal TotalSaleAgent { get; set; }
        public decimal TotalCommission { get; set; }
        public List<ComBillViewModel> CommissionBillList { get; set; } 
    }
}
