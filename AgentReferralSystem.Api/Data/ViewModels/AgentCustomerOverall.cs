using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Query;
using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace AgentReferralSystem.Api.Data.ViewModels
{
    public class AgentCustomerOverall
    {
        public DateTime CustomerDate { get; set; }
        public List<AgentCustomerViewModel> CustomerList { get; set; }

        public AgentCustomerOverall(DateTime CustomerDate, List<AgentCustomerViewModel> CustomerList)
        {
            this.CustomerDate = CustomerDate;
            this.CustomerList = CustomerList;
        }

        public static List<AgentCustomerOverall> GenerateCustomerOverall(List<CommissionItem> commissionItemList)
        {
            List<AgentCustomerOverall> result = new List<AgentCustomerOverall>();

            List<DateTime> dateList = commissionItemList.Select(x => x.Episode_Date).Distinct().ToList();
            List<DateTime> filterDate = new List<DateTime>();
            foreach(DateTime date in dateList)
            {
                DateTime itemD = DateTime.ParseExact(date.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                filterDate.Add(itemD);
            }
            foreach (DateTime date in filterDate)
            {
                List<CommissionItem> filteredList = commissionItemList.Where
                    (x => x.Episode_Date.Day == date.Day && x.Episode_Date.Month == date.Month && x.Episode_Date.Year == date.Year).ToList();
                List<AgentCustomerViewModel> ACVMList = AgentCustomerViewModel.GenByCustomerList(filteredList);
                AgentCustomerOverall item = new AgentCustomerOverall(date, ACVMList);
                result.Add(item);
            }
            return result;
        }
    }
}
