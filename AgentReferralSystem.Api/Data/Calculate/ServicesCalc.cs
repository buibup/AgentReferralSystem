using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Models.Calc;
using AgentReferralSystem.Api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Calculate
{
    public static class ServicesCalc
    {
        public static void SetSaleDetailViewModel(this ARPatientBill item,
            ref SaleDetailViewModel model, ref AgentCalc agentCalc,
            List<int> memberPapmiDrList, List<ARCItmMast> itemCompoundingList)
        {

            #region calc bwc service
            if (memberPapmiDrList.Any(m => item.PAADM_PAPMI_DR == m)) // member
            {
                if (itemCompoundingList.Any(i => i.ARCIM_RowId == item.ARCIM_RowId)) // compounding 
                {
                    agentCalc.CompoundingMember.TargetSumMonth += item.ITM_LineTotal;
                    agentCalc.CompoundingMember.TargetSum += item.ITM_LineTotal;
                    model.CompoundingMember = item.ITM_LineTotal;
                }
                else  // services
                {
                    agentCalc.ServiceMember.TargetSumMonth += item.ITM_LineTotal;
                    agentCalc.ServiceMember.TargetSum += item.ITM_LineTotal;
                    model.ServiceMember = item.ITM_LineTotal;
                }
            }
            else // non member
            {
                if (itemCompoundingList.Any(i => i.ARCIM_RowId == item.ARCIM_RowId)) // compounding 
                {
                    agentCalc.CompoundingNonMember.TargetSumMonth += item.ITM_LineTotal;
                    agentCalc.CompoundingNonMember.TargetSum += item.ITM_LineTotal;
                    model.CompoundingNonMember = item.ITM_LineTotal;
                }
                else  // services
                {
                    agentCalc.ServiceNonMember.TargetSumMonth += item.ITM_LineTotal;
                    agentCalc.ServiceNonMember.TargetSum += item.ITM_LineTotal;
                    model.ServiceNonMember = item.ITM_LineTotal;
                }
            }
            #endregion
        }
    }
}
