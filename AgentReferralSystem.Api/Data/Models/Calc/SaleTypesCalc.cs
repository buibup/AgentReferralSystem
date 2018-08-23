using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.Calc
{
    public class SaleTypesCalc
    {
        public TypesCalc Membership { get; set; }
        public TypesCalc ServiceMember { get; set; }
        public TypesCalc ServiceNonMember { get; set; }
        public TypesCalc CompoundingMember { get; set; }
        public TypesCalc CompoundingNonMember { get; set; }
    }

    public class TypesCalc
    {
        public decimal BaseCommission { get; set; }
        public decimal Target { get; set; }
        public int TargetPeriod { get; set; }
        public int ResetToBase { get; set; }
        public decimal IncreaseIfTargetMet { get; set; }
        public decimal Maximum { get; set; }
    }
}
