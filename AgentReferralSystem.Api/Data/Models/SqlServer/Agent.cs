using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentReferralSystem.Api.Data.ViewModels;


namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class Agent
    {
        public int AgentId { get; set; }
        public string AgentCode { get; set; }
        public string AgentDesc { get; set; }
        public DateTime AgreementDate { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string BankAccount { get; set; }
        public string DisplayImage { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Remark { get; set; }
        public decimal CurrentSale { get; set; }
        public decimal DisplayCommission { get; set; }
        public int CurrentReward { get; set; }
        public int TotalReward { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public bool isDelete { get; set; }

        //public List<AgentsSaleTypes> AgentSaleTypes { get; set; }
        //public List<AgentViewModel> AgentViewModel { get; set; }
    }
}
