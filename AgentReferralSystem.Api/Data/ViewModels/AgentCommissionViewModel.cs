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
    public class AgentCommissionViewModel
    {
        public DateTime CommissionDate { get; set; }
        public List<AgentCommissionDetailViewModel> DetailList { get; set; }

        public AgentCommissionViewModel(DateTime CommissionDate, List<AgentCommissionDetailViewModel> DetailList)
        {
            this.CommissionDate = CommissionDate;
            this.DetailList = DetailList;
        }

        public static List<AgentCommissionViewModel> GenerateAgentComVMList(List<CommissionItem> itemList, List<PercentConfig> configList)
        {
            var resultList = new List<AgentCommissionViewModel>();
            var dateList = itemList.Select(x => x.Episode_Date).Distinct().ToList();
            foreach (DateTime date in dateList)
            {
                List<CommissionItem> filteredList = itemList.Where(x => x.Episode_Date == date).ToList();
                List<AgentCommissionDetailViewModel> detailList = AgentCommissionDetailViewModel.GenerateAgentCDVMList(filteredList, configList);
                var item = new AgentCommissionViewModel(date, detailList);
                resultList.Add(item);
            }
            return resultList;
        }
    }
}
