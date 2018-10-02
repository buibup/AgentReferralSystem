using AgentReferralSystem.Api.Data.Models.Calc;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Calculate
{
    public static class CommonCalc
    {
        public static string getMonth(int monthNo)
        {
            switch(monthNo)
            {
                case 1: return "มกราคม,January";
                case 2: return "กุมภาพันธ์,February";
                case 3: return "มีนาคม,March";
                case 4: return "April,เมษายน";
                case 5: return "May,พฤษภาคม";
                case 6: return "June,มิถุุนายน";
                case 7: return "July,กรกฎาคม";
                case 8: return "August,สิงหาคม";
                case 9: return "September,กันยายน";
                case 10: return "October,ตุลาคม";
                case 11: return "November,พฤศจิกายน";
                case 12: return "December,ธันวาคม";
                default: return "";
            }
        }

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

        public static IEnumerable<ARPatientBill> FilterUnRegisItem(IEnumerable<ARPatientBill> TrakItem, List<CommissionItem> SQLItem)
        {
            return TrakItem.Where(x => !SQLItem.Any(y => x.ARPBL_RowId == y.ARPBL_RowId));
        }

        public static ComBillViewModel calculateComByItemList(IEnumerable<ARPatientBill> patientBills,
        IEnumerable<PercentConfig> ConfigList)
        {
            decimal sumItmValue = 0;
            //decimal sumCommiss = 0;
            var result = new ComBillViewModel();
            var BillList = new List<CommissionItem>();
            foreach (ARPatientBill Bill in patientBills)
            {
                var BillItem = new CommissionItem();
                BillItem.ARPBL_RowId = Bill.ARPBL_RowId;
                BillItem.Agent_Code = Bill.REFT_Code;
                BillItem.Agent_Name = Bill.REFT_Desc;
                BillItem.Patient_Name = (Bill.PAPMI_Name ?? "") + " " + (Bill.PAPMI_Name2 ?? "");
                BillItem.Patient_Desc = Bill.INST_Desc;
                BillItem.HN_Number = Bill.PAPMI_No;
                BillItem.Episode_Number = Bill.EpisodeNo;
                BillItem.Episode_Date = Bill.EpisodeDate;
                BillItem.Doctor_Code = Bill.CTPCP_Code;
                BillItem.Doctor_Name = Bill.CTPCP_Desc;
                BillItem.Discharge_Date = Bill.DischargeDate;
                BillItem.BillPrinted_Date = Bill.BillPrintedDate;
                BillItem.PatientBill_Number = Bill.ARPBL_BillNo;
                if ((Bill.ARPBL_BillNo ?? "").Substring(3, 2) == "CO") BillItem.Bill_Type = "I";
                else if ((Bill.ARPBL_BillNo ?? "").Substring(3, 2) == "TO") BillItem.Bill_Type = "R";
                else BillItem.Bill_Type = "";
                BillItem.check_PaidInvoice = "";
                BillItem.Item_Id = Bill.ARCIM_RowId;
                BillItem.Item_Code = Bill.ARCIM_Code;
                BillItem.Item_Desc = Bill.ARCIM_Desc;
                BillItem.Item_Total = Bill.ITM_LineTotal;
                decimal.Add(sumItmValue, BillItem.Item_Total);
                BillItem.Item_Commission = 0;
                //BillItem.Item_Commission
                //decimal.Add(sumCommiss, BillItem.Item_Commission);
                BillItem.isDelete = false;
                BillItem.CreateDate = DateTime.Now;
                BillList.Add(BillItem);
            }
            //Calculate Commission by Total Amount
            var CommissionRateConfig = ConfigList.Where(x => x.ConfigType == "CommissionRate");
            decimal CommissionAmount = 0;
            if (CommissionRateConfig.Count() > 0)
            {
                var InConditionConfig = CommissionRateConfig.Where(x => decimal.Parse((x.value2 ?? "0")) >= sumItmValue).FirstOrDefault();
                if (InConditionConfig == null && CommissionRateConfig.Count() > 0) InConditionConfig = CommissionRateConfig.Last();
                CommissionAmount = decimal.Multiply(sumItmValue, InConditionConfig.value1);
            }
            result.ItemList = BillList;
            result.sumItmValue = sumItmValue;
            result.sumCommiss = CommissionAmount;
            return result;
        }

        public static int calculateReward(ComBillViewModel CommissionBill, 
            IEnumerable<PercentConfig> ConfigList)
        {
            var result = 0;
            var RewardConfigList = ConfigList.Where(x => x.ConfigType == "RewardConfig").First();
            if (RewardConfigList.value1Meaning == "Flat")
                result = (int)decimal.Divide(CommissionBill.sumItmValue, RewardConfigList.value1);
            else if (RewardConfigList.value1Meaning == "Percent")
                result = (int)decimal.Multiply(CommissionBill.sumItmValue, decimal.Divide(RewardConfigList.value1, 100));
            return result;
        }
    }
}
