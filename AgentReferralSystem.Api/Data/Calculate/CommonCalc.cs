using AgentReferralSystem.Api.Data.Models.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Calculate
{
    public static class CommonCalc
    {
        public static decimal SumTargetOfMonth(this AgentCalc agentCalc)
        {
            return agentCalc.Membership.TargetSumMonth + agentCalc.ServiceMember.TargetSumMonth +
                            agentCalc.ServiceNonMember.TargetSumMonth + agentCalc.CompoundingMember.TargetSumMonth +
                            agentCalc.CompoundingNonMember.TargetSumMonth;
        }

        public static decimal SumCommissionOfMonth(this AgentCalc agentCalc)
        {
            return agentCalc.Membership.CommissionSumMonth + agentCalc.ServiceMember.CommissionSumMonth +
                            agentCalc.ServiceNonMember.CommissionSumMonth + agentCalc.CompoundingMember.CommissionSumMonth +
                            agentCalc.CompoundingNonMember.CommissionSumMonth;
        } 

        public static void TargetMet(ref AgentCalc agentCalc)
        {
            #region membership
            // is membership target met
            if (agentCalc.Membership.TargetSum >= agentCalc.Membership.Target &&
                agentCalc.Membership.TargetPeriodMonth <= agentCalc.Membership.TargetPeriod)
            {
                // check maximum
                if (agentCalc.Membership.BaseCommission < agentCalc.Membership.Maximum)
                {
                    // add base commission
                    agentCalc.Membership.BaseCommission += agentCalc.Membership.IncreaseIfTargetMet;
                }
            }
            // calc membership commission
            agentCalc.Membership.CommissionSumMonth = decimal.Round(agentCalc.Membership.TargetSumMonth * agentCalc.Membership.BaseCommission) / 100;

            #region checkRetroactive
            if (agentCalc.Membership.ApplicableTargetIncrease.ToLower() == "retroactive")
            {
                //agentCalc.Membership.TargetSum = 0;
                //agentCalc.Membership.BaseCommission = membershipObject.BaseCommission;
                //นำส่วนต่างเดือนที่ผ่านมา add เข้าไปในค่า commission (เอาเปอร์เซ็นที่เพิ่มมาคูณยอดขายเดือนที่ผ่านมา) โดยตัดตั้งแต่ CutoffMonth มาล่าสุด
            }
            #endregion

            #endregion

            #region service member
            // is service member target met 
            if (agentCalc.ServiceMember.TargetSum >= agentCalc.ServiceMember.Target)
            {
                // check maximum
                if (agentCalc.ServiceMember.BaseCommission < agentCalc.ServiceMember.Maximum)
                {
                    // add base commission
                    agentCalc.ServiceMember.BaseCommission += agentCalc.ServiceMember.IncreaseIfTargetMet;
                }
            }
            // calc service member commission
            agentCalc.ServiceMember.CommissionSumMonth = (agentCalc.ServiceMember.TargetSumMonth * agentCalc.ServiceMember.BaseCommission) / 100;

            #region checkRetroactive
            if (agentCalc.ServiceMember.ApplicableTargetIncrease.ToLower() == "retroactive")
            {
                //agentCalc.Membership.TargetSum = 0;
                //agentCalc.Membership.BaseCommission = membershipObject.BaseCommission;
                //นำส่วนต่างเดือนที่ผ่านมา add เข้าไปในค่า commission (เอาเปอร์เซ็นที่เพิ่มมาคูณยอดขายเดือนที่ผ่านมา) โดยตัดตั้งแต่ CutoffMonth มาล่าสุด
            }
            #endregion

            #endregion

            #region service non member
            // is service non member target met 
            if (agentCalc.ServiceNonMember.TargetSum >= agentCalc.ServiceNonMember.Target)
            {
                // check maximum
                if (agentCalc.ServiceNonMember.BaseCommission < agentCalc.ServiceNonMember.Maximum)
                {
                    // add base commission
                    agentCalc.ServiceNonMember.BaseCommission += agentCalc.ServiceNonMember.IncreaseIfTargetMet;
                }
            }

            #region checkRetroactive
            if (agentCalc.ServiceNonMember.ApplicableTargetIncrease.ToLower() == "retroactive")
            {
                //agentCalc.Membership.TargetSum = 0;
                //agentCalc.Membership.BaseCommission = membershipObject.BaseCommission;
                //นำส่วนต่างเดือนที่ผ่านมา add เข้าไปในค่า commission (เอาเปอร์เซ็นที่เพิ่มมาคูณยอดขายเดือนที่ผ่านมา) โดยตัดตั้งแต่ CutoffMonth มาล่าสุด
            }
            #endregion

            // calc service non member commission
            agentCalc.ServiceNonMember.CommissionSumMonth = (agentCalc.ServiceNonMember.TargetSumMonth * agentCalc.ServiceNonMember.BaseCommission) / 100;

            #endregion

            #region compounding member
            // is compounding member target met 
            if (agentCalc.CompoundingMember.TargetSum >= agentCalc.CompoundingMember.Target)
            {
                // check maximum
                if (agentCalc.CompoundingMember.BaseCommission < agentCalc.CompoundingMember.Maximum)
                {
                    // add base commission
                    agentCalc.CompoundingMember.BaseCommission += agentCalc.CompoundingMember.IncreaseIfTargetMet;
                }
            }

            // calc compounding member commission
            agentCalc.CompoundingMember.CommissionSumMonth = (agentCalc.CompoundingMember.TargetSumMonth * agentCalc.CompoundingMember.BaseCommission) / 100;

             #region checkRetroactive
            if (agentCalc.CompoundingMember.ApplicableTargetIncrease.ToLower() == "retroactive")
            {
                //agentCalc.Membership.TargetSum = 0;
                //agentCalc.Membership.BaseCommission = membershipObject.BaseCommission;
                //นำส่วนต่างเดือนที่ผ่านมา add เข้าไปในค่า commission (เอาเปอร์เซ็นที่เพิ่มมาคูณยอดขายเดือนที่ผ่านมา) โดยตัดตั้งแต่ CutoffMonth มาล่าสุด
            }
            #endregion

            #endregion

            #region compounding non member
            // is compounding non member target met  
            if (agentCalc.CompoundingNonMember.TargetSum >= agentCalc.CompoundingNonMember.Target)
            {
                // check maximum
                if (agentCalc.CompoundingNonMember.BaseCommission < agentCalc.CompoundingNonMember.Maximum)
                {
                    // add base commission
                    agentCalc.CompoundingNonMember.BaseCommission += agentCalc.CompoundingNonMember.IncreaseIfTargetMet;
                }
            }

            #region checkRetroactive
            if (agentCalc.CompoundingNonMember.ApplicableTargetIncrease.ToLower() == "retroactive")
            {
                //agentCalc.Membership.TargetSum = 0;
                //agentCalc.Membership.BaseCommission = membershipObject.BaseCommission;
                //นำส่วนต่างเดือนที่ผ่านมา add เข้าไปในค่า commission (เอาเปอร์เซ็นที่เพิ่มมาคูณยอดขายเดือนที่ผ่านมา) โดยตัดตั้งแต่ CutoffMonth มาล่าสุด
            }
            #endregion

            // calc compounding non member commission
            agentCalc.CompoundingNonMember.CommissionSumMonth = (agentCalc.CompoundingNonMember.TargetSumMonth * agentCalc.CompoundingNonMember.BaseCommission) / 100;

            #endregion
        }
    }
}
