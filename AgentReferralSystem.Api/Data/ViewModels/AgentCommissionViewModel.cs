using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Query;
using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using AgentReferralSystem.Api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.ViewModels
{
    public class AgentCommissionViewModel
    {
        public DateTime CommissionDate { get; set; }
        public decimal currentCommission { get; set; }
        public decimal TotalSale { get; set; }
        public string CustomerName { get; set; }
        public decimal CustomerSpend { get; set; }

        public AgentCommissionViewModel(DateTime CommissionDate, decimal currentCommission, decimal TotalSale, string CustomerName, decimal CustomerSpend)
        {
            this.CommissionDate = CommissionDate;
            this.CustomerName = CustomerName;
            this.CustomerSpend = CustomerSpend;
            this.currentCommission = currentCommission;
            this.TotalSale = TotalSale;
        }

        public static async Task<List<AgentDateCommissionViewModel>> GenerateAgentComVMList(List<CommissionItem> itemsList, List<PercentConfig> configList)
        {
            List<AgentDateCommissionViewModel> result = new List<AgentDateCommissionViewModel>();
            await Task.Run(() =>
            {
                decimal TotalSale = 0;
                var dateList = itemsList.Select(x => x.Episode_Date).Distinct().ToList();
                foreach (DateTime date in dateList)
                {
                    List<AgentCommissionViewModel> detailList = new List<AgentCommissionViewModel>();
                    var datefilterList = itemsList.Where(x => (x.Episode_Date.Day == date.Day && x.Episode_Date.Month == date.Month 
                        && x.Episode_Date.Year == date.Year)).ToList();
                    var customerList = datefilterList.Select(x => x.Patient_Name).Distinct().ToList();
                    foreach (string customer in customerList)
                    {
                        //Check CurrentPercent
                        var CurrentConfig = configList.Where(x => decimal.Parse(x.value2 ?? "0") <= TotalSale).Last();
                        if (CurrentConfig == null) CurrentConfig = configList.First();
                        var CurrentPercent = CurrentConfig.value1;

                        var customerfilterList = datefilterList.Where(x => x.Patient_Name == customer).ToList();
                        var customerTotal = customerfilterList.Sum(x => x.Item_Total);
                        TotalSale = decimal.Add(customerTotal, TotalSale);
                        var commission = decimal.Divide(decimal.Multiply(TotalSale, CurrentPercent), 100);
                        AgentCommissionViewModel item = new AgentCommissionViewModel(date,commission,TotalSale, customer, customerTotal);
                        detailList.Add(item);
                    }
                    AgentDateCommissionViewModel itm = new AgentDateCommissionViewModel(date, detailList);
                    result.Add(itm);
                }

                //    List<int> yearList = dateComList.OrderBy(x => x.CommissionDate).Select(x => x.CommissionDate.Year).Distinct().ToList();
                //    foreach (int year in yearList)
                //    {
                //        for (int i = 1; i <= 12; i++)
                //        {
                //            var filterList = dateComList.Where(x => x.CommissionDate.Month == i && x.CommissionDate.Year == year).ToList();
                //            AgentMonthCommissionViewModel monthItem = new AgentMonthCommissionViewModel(year, i, filterList);
                //            MonthList.Add(monthItem);
                //        }
                //    }
            });

            return result;
        }
    }
}
