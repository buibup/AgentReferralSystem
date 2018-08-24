using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class AgentMonthSale
    {
        public int id { get; set; }
        public int AgentId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal MembershipAmount { get; set; }
        public decimal CommissMemberAmount { get; set; }
        public decimal ServiceMemberAmount { get; set; }
        public decimal CommissServiceMemberAmount { get; set; }
        public decimal ServiceNonMemberAmount { get; set; }
        public decimal CommissServiceNonMemberAmount { get; set; }
        public decimal CompoundMemberAmount { get; set; }
        public decimal CommissCompoundMemberAmount { get; set; }
        public decimal CompoundNonMemberAmount { get; set; }
        public decimal CommissCompoundNonMemberAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotaCommissionAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public bool isDelete { get; set; }
    }
}
