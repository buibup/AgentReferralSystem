using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.Calc
{
    public class AgentCalc
    {
        public Calc Membership { get; set; }
        public Calc ServiceMember { get; set; }
        public Calc ServiceNonMember { get; set; }
        public Calc CompoundingMember { get; set; }
        public Calc CompoundingNonMember { get; set; }
    }

    public class Calc
    {
        public decimal BaseCommission { get; set; }

        public decimal Target { get; set; }
        public decimal TargetSumMonth { get; set; }
        public decimal TargetSum { get; set; }

        public int TargetPeriod { get; set; }
        public int TargetPeriodMonth { get; set; }

        public decimal IncreaseIfTargetMet { get; set; }

        public decimal Maximum { get; set; }

        public int ResetToBase { get; set; }
        public int ResetToBaseMonth { get; set; }

        public decimal CommissionSumMonth { get; set; }
        public decimal Commission { get; set; }
    }
}
