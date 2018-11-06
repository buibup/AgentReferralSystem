using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Query;
using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.ViewModels
{
    public class CustomerItemViewModel
    {
        public string ServiceName { get; set; }
        public decimal ServicePrice { get; set; }
        public decimal ServiceCommission { get; set; }
        public DateTime ServiceDate { get; set; }
        
        public CustomerItemViewModel(CommissionItem commissionItem)
        {
            this.ServiceName = commissionItem.Item_Desc;
            this.ServicePrice = commissionItem.Item_Total;
            this.ServiceCommission = commissionItem.Item_Commission;
            this.ServiceDate = commissionItem.Episode_Date;
        }
    }
}
