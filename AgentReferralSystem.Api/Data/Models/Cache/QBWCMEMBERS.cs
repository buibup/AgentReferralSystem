using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.Cache
{
    public class QBWCMEMBERS
    {
        public int ID { get; set; }
        public int QUESPAAdmDR { get; set; }
        public int QUESPAPatMasDR { get; set; }
        public DateTime QDateFrom { get; set; }
        public DateTime QDateTO { get; set; }
    }
}
