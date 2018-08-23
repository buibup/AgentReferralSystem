using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Calculate
{
    public static class MembershipCalc
    {
        public static decimal MembersCalculate(this int memberNum)
        {
            return memberNum * 3_000_000;
        }
    }
}
