using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models
{
    public class SaleTypesOutput
    {
        public int SaleTypeId { get; set; }

        public string SaleTypeName { get; set; }

        public decimal BaseCommission { get; set; }

        public decimal Target { get; set; }

        public int TargetPeriod { get; set; }

        public decimal IncreaseIfTargetMet { get; set; }

        public decimal Maximum { get; set; }

        public int ResetToBase { get; set; }

        public string ApplicableTargetInrease { get; set; }
    }
}
