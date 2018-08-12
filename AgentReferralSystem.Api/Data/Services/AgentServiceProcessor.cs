using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Services
{
    public static class AgentServiceProcessor
    {
        public static AgentViewModel AgentViewModelProcess(this Agent agent, 
            IEnumerable<ARPatientBill> patientBills, 
            IEnumerable<QBWCMEMBERS> bWCMEMBERs)
        {
            var start = agent.StartDate;
            var years = patientBills.Select(d => d.EpisodeDate.Year).Distinct();
            var totalSalesPerMonths = new List<TotalSalesPerMonthViewModel>();

            ResetToBase:
            var agentBase = agent;
            var agentsSaleTypes = agentBase.AgentSaleTypes;

            #region set object
            var membershipObject = agentBase.AgentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.Membership)
                .Select(d => new { d.BaseCommission, d.Target, d.TargetPeriod, d.ResetToBase, d.IncreaseIfTargetMet, d.Maximum }).FirstOrDefault();

            var serviceMemberObject = agentBase.AgentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.ServiceMember)
                .Select(d => new { d.BaseCommission, d.Target, d.TargetPeriod, d.ResetToBase, d.IncreaseIfTargetMet, d.Maximum }).FirstOrDefault();

            var serviceNonMemberObject = agentBase.AgentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.ServiceNonMember)
                .Select(d => new { d.BaseCommission, d.Target, d.TargetPeriod, d.ResetToBase, d.IncreaseIfTargetMet, d.Maximum }).FirstOrDefault();

            var compoundingMemberObject = agentBase.AgentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.CompoundingMember)
                .Select(d => new { d.BaseCommission, d.Target, d.TargetPeriod, d.ResetToBase, d.IncreaseIfTargetMet, d.Maximum }).FirstOrDefault();

            var compoundingNonMemberObject = agentBase.AgentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.CompoundingNonMember)
                .Select(d => new { d.BaseCommission, d.Target, d.TargetPeriod, d.ResetToBase, d.IncreaseIfTargetMet, d.Maximum }).FirstOrDefault();
            #endregion
            
            
            var monthCountResetToBase = 0;
            var monthCountCommTarget = 0;
            
            while (start < agent.EndDate)
            {
                var currentYear = start.Year;
                var currentMonth = start.Month;

                var patientBilllistOnCurrent = patientBills.Where(d => (d.EpisodeDate.Year == currentYear) && (d.EpisodeDate.Month == currentMonth)).ToList();
                

                var membersThisMonth = bWCMEMBERs.Where(d => d.QDateFrom.Month == currentMonth).ToList();
                var bWCServicesThisMonth = patientBilllistOnCurrent.Select(d => d.PAADM_PAPMI_DR).Distinct().ToList();

                var tupleTotalSalesPerMonthVM = patientBilllistOnCurrent.ToTotalSalesPerMonthVM(membersThisMonth);

                var totalSalesPerMonth = new TotalSalesPerMonthViewModel
                {
                    Month = (Month)currentMonth,
                    SaleDetails = tupleTotalSalesPerMonthVM.Item1,
                    MembershipCount = membersThisMonth.Count,
                    BWCServicesCount = bWCServicesThisMonth.Count,
                    TotalSales = tupleTotalSalesPerMonthVM.Item2,
                };

                totalSalesPerMonths.Add(totalSalesPerMonth);

                // if target met

                // add month
                start = agent.StartDate.AddMonths(1);

                monthCountResetToBase += 1;
                monthCountCommTarget += 1;

                if (monthCountResetToBase > membershipObject.ResetToBase)
                {
                    goto ResetToBase;
                }

            }

            var totalSalesPerYear = new TotalSalesPerYearViewModel
            {
                
            };

            var result = new AgentViewModel
            {
                AgentName = agent.AgentDesc,
                
            };

            return result;
        }

        private static Tuple<List<SaleDetailViewModel>, decimal> ToTotalSalesPerMonthVM(this List<ARPatientBill> patientBills,
            IEnumerable<QBWCMEMBERS> bWCMEMBERs)
        {
            var saleDetailVMList = new List<SaleDetailViewModel>();
            decimal totalSales = 0;


            foreach (var item in patientBills)
            {
                
                var model = new SaleDetailViewModel
                {
                    HN = item.PAPMI_No,
                    Episode = item.EpisodeNo,
                    Date = item.EpisodeDate,
                    Membership = 0,
                    ServiceMember = 0,
                    ServiceNonMember = 0,
                    CompoundingMember = 0,
                    CompoundingNonMember = 0,
                    TotalAmount = 0,
                    Commission = 0
                };
            }

            return Tuple.Create(saleDetailVMList, totalSales);
        }
    }
}
