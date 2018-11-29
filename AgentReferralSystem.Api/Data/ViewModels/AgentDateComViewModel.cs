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
    public class AgentDateCommissionViewModel
    {
        public DateTime CommissionDate { get; set; }
        public List<AgentCommissionViewModel> detailList { get; set; }

        public AgentDateCommissionViewModel(DateTime CommissionDate, List<AgentCommissionViewModel> detailList)
        {
            this.CommissionDate = CommissionDate;
            this.detailList = detailList;
        }
    }
}
