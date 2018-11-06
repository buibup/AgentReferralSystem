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
    public class AgentCommissionDetailViewModel
    {
        public int AgentId { get; set; }
        public string CustomerName { get; set; }
        public decimal CustomerSpend { get; set; }
        public decimal TotalSpend { get; set; }
        public decimal CurrentPercent { get; set; }
        public decimal TotalCommission { get; set; }

        public AgentCommissionDetailViewModel(int AgentId, string CustomerName, decimal CustomerSpend, decimal TotalSpend, decimal CurrentPercent, decimal TotalCommission)
        {
            this.AgentId = AgentId;
            this.CustomerName = CustomerName;
            this.CustomerSpend = CustomerSpend;
            this.TotalSpend = TotalSpend;
            this.CurrentPercent = CurrentPercent;
            this.TotalCommission = TotalCommission;
        }
        public static List<AgentCommissionDetailViewModel> GenerateAgentCDVMList(List<CommissionItem> ItemList, List<PercentConfig> ConfigList)
        {
            List<AgentCommissionDetailViewModel> result = new List<AgentCommissionDetailViewModel>();
            ItemList = ItemList.OrderBy(x => x.Episode_Date).ToList();
            decimal TotalSpend = 0;
            foreach(CommissionItem item in ItemList)
            {
                TotalSpend += item.Item_Total;
                //Check CurrentPercent
                var CurrentConfig = ConfigList.Where(x => decimal.Parse(x.value2 ?? "0") <= item.Item_Total).Last();
                if (CurrentConfig == null) CurrentConfig = ConfigList.First();
                var CurrentPercent = CurrentConfig.value1;

                //CalTotalCommission
                var TotalCommission = decimal.Divide(decimal.Multiply(TotalSpend, CurrentPercent), 100);

                AgentCommissionDetailViewModel itemModel = new AgentCommissionDetailViewModel(item.Agent_Id, item.Patient_Name, item.Item_Total, TotalSpend, CurrentPercent, TotalCommission);
                result.Add(itemModel);
            }
            return result;
        }
    }
}
