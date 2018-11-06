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
    public class AgentCustomerViewModel
    {
        public int AgentId { get; set; }
        public string CustomerId { get; set; } //patientId
        public string CustomerName { get; set; }
        public decimal CustomerTotalSpend { get; set; }

        public List<CustomerItemViewModel> ItemList { get; set; }

        public AgentCustomerViewModel(List<CommissionItem> commissionItemList)
        {
            var defaultItem = commissionItemList.First();
            this.AgentId = defaultItem.Agent_Id;
            this.CustomerId = defaultItem.PatientId;
            this.CustomerName = defaultItem.Patient_Name;
            this.CustomerTotalSpend = commissionItemList.Sum(x => x.Item_Total);
            this.ItemList = new List<CustomerItemViewModel>();

            ConvertToCustomerItemViewModel(commissionItemList);
        }

        private AgentCustomerViewModel(CommissionItem item)
        {
            this.AgentId = item.Agent_Id;
            this.CustomerId = item.PatientId;
            this.CustomerName = item.Patient_Name;
            this.CustomerTotalSpend = item.Item_Total;
            this.ItemList = new List<CustomerItemViewModel>();
        }

        private void ConvertToCustomerItemViewModel(List<CommissionItem> commissionItemList)
        {
            foreach(CommissionItem commissionItem in commissionItemList)
            {
                CustomerItemViewModel item = new CustomerItemViewModel(commissionItem);
                this.ItemList.Add(item);
            }
        }

        public static List<AgentCustomerViewModel> GenerateACVMList(List<CommissionItem> commissionItemList)
        {
            List<AgentCustomerViewModel> resultList = new List<AgentCustomerViewModel>();

            var _comList = commissionItemList.OrderByDescending(x => x.Episode_Date).ToList();
            foreach (CommissionItem item in _comList)
            {
                var resultItem = new AgentCustomerViewModel(item);
                resultList.Add(resultItem);
            }

            return resultList;
        }

        public static List<AgentCustomerViewModel> GenByCustomerList(List<CommissionItem> commissionItemList)
        {
            List<AgentCustomerViewModel> result = new List<AgentCustomerViewModel>();
            List<string> customerList = commissionItemList.Select(x => x.Patient_Name).Distinct().ToList();
            foreach(string customer in customerList)
            {
                List<CommissionItem> filteredList = commissionItemList.Where(x => x.Patient_Name == customer).ToList();
                AgentCustomerViewModel item = new AgentCustomerViewModel(filteredList);
                result.Add(item);
            }
            return result;
        }
    }
}
    