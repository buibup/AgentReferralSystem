using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class Agent
    {
        public int AgentId { get; set; }
        public string AgentCode { get; set; }
        public string AgentDesc { get; set; }
        public DateTime AgreementDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Remark { get; set; }

        public List<AgentsSaleTypes> AgentSaleTypes { get; set; }
    }
}
