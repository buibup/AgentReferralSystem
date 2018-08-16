using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models
{
    public class AgentOutput
    {
        public int AgentId { get; set; }
        public string AgentCode { get; set; }
        public string AgentDesc { get; set; }
        public DateTime AgreementDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Remark { get; set; }

        public ICollection<SaleTypesOutput> AgentSaleTypes { get; set; }
    }
}
