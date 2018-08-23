using AgentReferralSystem.Api.Data.Models;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Models.Calc;
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
        public static SaleTypesCalc SaleTypesCalcBase { get; set; } = new SaleTypesCalc();

        public static AgentViewModel AgentViewModelProcess(this AgentOutput agent,
            IEnumerable<ARPatientBill> patientBills,
            IEnumerable<QBWCMEMBERS> memberRegisList,
            IEnumerable<ARCItmMast> itemCompoundingList)
        {

            #region create variable 
            var totalSalesPerMonths = new List<TotalSalesPerMonthViewModel>();
            var totalSalesPerYears = new List<TotalSalesPerYearViewModel>();
            decimal totalSalesAgent = 0;
            decimal totalCommissionAgent = 0;

            // set agent start date
            var start = agent.StartDate;

            // get all year of patient
            var years = patientBills.Select(d => d.EpisodeDate.Year).Distinct();

            // set agent base
            var agentBase = agent;
            var agentsSaleTypes = agentBase.AgentSaleTypes;
            #endregion

            // set agent calc
            var agentCalc = agentsSaleTypes.GetSaleTypesCalc();


            foreach (var year in years)
            {
                decimal totalSalesYear = 0;
                decimal totalCommissionYear = 0;
                TotalSalesPerYearViewModel totalSalesPerYear = null;

                while (start < agent.EndDate && start.Year == year)
                {
                    var saleDetailList = new List<SaleDetailViewModel>();
                    var currentYear = start.Year;
                    var currentMonth = start.Month;
                    decimal totalSalesMonth = 0;

                    if (year == currentYear)
                    {
                        // filter patient on current month
                        var patientBillListOnCurrent = patientBills.Where(pt => (pt.EpisodeDate.Year == currentYear) && (pt.EpisodeDate.Month == currentMonth)).ToList();

                        // count membership on current month
                        var membersThisMonth = memberRegisList.Where(m => (m.QDateFrom.Year == currentYear) && (m.QDateFrom.Month == currentMonth)).Select(m => m.QUESPAPatMasDR).Distinct().ToList();

                        // count all patient bwc services on current month
                        var papmiDRList = patientBillListOnCurrent.Select(p => p.PAADM_PAPMI_DR).Distinct().ToList();

                        var memberPapmiDrList = memberRegisList.Select(m => m.QUESPAPatMasDR).Distinct().ToList();

                        // patient member
                        var patientBillMemberList = patientBillListOnCurrent.Where(p => memberPapmiDrList.Any(p2 => p2 == p.PAADM_PAPMI_DR)).ToList();

                        // patient non member
                        var patientBillNonmemberList = patientBillListOnCurrent.Where(p => !memberPapmiDrList.Any(p2 => p2 == p.PAADM_PAPMI_DR)).ToList();


                        #region calc membership
                        if (membersThisMonth.Count > 0)
                        {
                            agentCalc.Membership.TargetSumMonth = membersThisMonth.Count * 3_000_000;
                            agentCalc.Membership.TargetSum += membersThisMonth.Count * 3_000_000; ;
                        }
                        #endregion

                        #region Gun's Code
                        // Add Member(Extend Code)
                        foreach (var item in patientBillListOnCurrent)
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

                            #region calc bwc service
                            if (memberPapmiDrList.Any(m => item.PAADM_PAPMI_DR == m)) // member
                            {
                                if (itemCompoundingList.Any(i => i.ARCIM_RowId == item.ARCIM_RowId)) // compounding 
                                {
                                    agentCalc.CompoundingMember.TargetSumMonth += item.ITM_LineTotal;
                                    agentCalc.CompoundingMember.TargetSum += item.ITM_LineTotal;
                                    model.CompoundingMember = item.ITM_LineTotal;
                                    model.Commission = Decimal.Divide(Decimal.Multiply(item.ITM_LineTotal,agentCalc.CompoundingMember.BaseCommission),100);
                                }
                                else  // services
                                {
                                    agentCalc.ServiceMember.TargetSumMonth += item.ITM_LineTotal;
                                    agentCalc.ServiceMember.TargetSum += item.ITM_LineTotal;
                                    model.ServiceMember = item.ITM_LineTotal;
                                    model.Commission = Decimal.Divide(Decimal.Multiply(item.ITM_LineTotal,agentCalc.ServiceMember.BaseCommission),100);
                                }
                            }
                            else // non member
                            {
                                if (itemCompoundingList.Any(i => i.ARCIM_RowId == item.ARCIM_RowId)) // compounding 
                                {
                                    agentCalc.CompoundingNonMember.TargetSumMonth += item.ITM_LineTotal;
                                    agentCalc.CompoundingNonMember.TargetSum += item.ITM_LineTotal;
                                    model.CompoundingNonMember = item.ITM_LineTotal;
                                    model.Commission = Decimal.Divide(Decimal.Multiply(item.ITM_LineTotal,agentCalc.CompoundingNonMember.BaseCommission),100);
                                }
                                else  // services
                                {
                                    agentCalc.ServiceNonMember.TargetSumMonth += item.ITM_LineTotal;
                                    agentCalc.ServiceNonMember.TargetSum += item.ITM_LineTotal;
                                    model.ServiceNonMember = item.ITM_LineTotal;
                                    model.Commission = Decimal.Divide(Decimal.Multiply(item.ITM_LineTotal,agentCalc.ServiceNonMember.BaseCommission),100);
                                }
                            }
                            #endregion

                            saleDetailList.Add(model);
                        }
                        #endregion

                        totalSalesMonth = agentCalc.Membership.TargetSumMonth + agentCalc.ServiceMember.TargetSumMonth +
                            agentCalc.ServiceNonMember.TargetSumMonth + agentCalc.CompoundingMember.TargetSumMonth +
                            agentCalc.CompoundingNonMember.TargetSumMonth;

                        // add target period month
                        agentCalc.Membership.TargetPeriodMonth += 1;
                        agentCalc.ServiceMember.TargetPeriodMonth += 1;
                        agentCalc.ServiceNonMember.TargetPeriodMonth += 1;
                        agentCalc.CompoundingMember.TargetPeriodMonth += 1;
                        agentCalc.CompoundingNonMember.TargetPeriodMonth += 1;

                        // add reset to base month
                        agentCalc.Membership.ResetToBaseMonth += 1;
                        agentCalc.ServiceMember.ResetToBaseMonth += 1;
                        agentCalc.ServiceNonMember.ResetToBaseMonth += 1;
                        agentCalc.CompoundingMember.ResetToBaseMonth += 1;
                        agentCalc.CompoundingNonMember.ResetToBaseMonth += 1;

                        #region target met

                        #region membership
                        // is membership target met
                        if(agentCalc.Membership.TargetSum >= agentCalc.Membership.Target && 
                            agentCalc.Membership.TargetPeriodMonth <= agentCalc.Membership.TargetPeriod)
                        {
                            // check maximum
                            if(agentCalc.Membership.BaseCommission < agentCalc.Membership.Maximum)
                            {
                                // add base commission
                                agentCalc.Membership.BaseCommission += agentCalc.Membership.IncreaseIfTargetMet;
                            }
                        }
                        // calc membership commission
                        agentCalc.Membership.CommissionSumMonth = decimal.Round(agentCalc.Membership.TargetSumMonth * agentCalc.Membership.BaseCommission) / 100;
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

                        // calc compounding non member commission
                        agentCalc.CompoundingNonMember.CommissionSumMonth = (agentCalc.CompoundingNonMember.TargetSumMonth * agentCalc.CompoundingNonMember.BaseCommission) / 100;

                        #endregion

                        #endregion end target met

                        var commissionSumMonth = agentCalc.Membership.CommissionSumMonth + agentCalc.ServiceMember.CommissionSumMonth + 
                            agentCalc.ServiceNonMember.CommissionSumMonth + agentCalc.CompoundingMember.CommissionSumMonth + 
                            agentCalc.CompoundingNonMember.CommissionSumMonth;

                        totalSalesYear += agentCalc.Membership.TargetSumMonth + agentCalc.ServiceMember.TargetSumMonth + 
                            agentCalc.ServiceNonMember.TargetSumMonth + agentCalc.CompoundingMember.TargetSumMonth + 
                            agentCalc.CompoundingNonMember.TargetSumMonth;

                        totalCommissionYear += commissionSumMonth;

                        var totalSalesPerMonth = new TotalSalesPerMonthViewModel
                        {
                            Month = (Month)currentMonth,
                            SaleDetails = saleDetailList,
                            MembershipCount = membersThisMonth.Count,
                            BWCServicesCount = papmiDRList.Count,
                            TotalSales = totalSalesMonth,
                            MembershipSum = agentCalc.Membership.TargetSumMonth,
                            MembershipSumCommission = decimal.Round(agentCalc.Membership.CommissionSumMonth, 2, MidpointRounding.AwayFromZero),
                            ServiceMemberSum = agentCalc.ServiceMember.TargetSumMonth,
                            ServiceMemberSumCommission = decimal.Round(agentCalc.ServiceMember.CommissionSumMonth, 2, MidpointRounding.AwayFromZero),
                            ServiceNonMemberSum = agentCalc.ServiceNonMember.TargetSumMonth,
                            ServiceNonMemberSumCommission = decimal.Round(agentCalc.ServiceNonMember.CommissionSumMonth, 2, MidpointRounding.AwayFromZero),
                            CompoundingMemberSum = agentCalc.CompoundingMember.TargetSumMonth,
                            CompoundingMemberSumCommission = decimal.Round(agentCalc.CompoundingMember.CommissionSumMonth, 2,MidpointRounding.AwayFromZero),
                            CompoundingNonMemberSum = agentCalc.CompoundingNonMember.TargetSumMonth,
                            CompoundingNonMemberSumCommission = decimal.Round(agentCalc.CompoundingNonMember.CommissionSumMonth, 2, MidpointRounding.AwayFromZero),
                            CommissionSum = decimal.Round(commissionSumMonth, 2, MidpointRounding.AwayFromZero)
                        };

                        #region reset month value

                        //totalSalesMonth = 0;

                        agentCalc.Membership.TargetSumMonth = 0;
                        agentCalc.Membership.CommissionSumMonth = 0;

                        agentCalc.ServiceMember.TargetSumMonth = 0;
                        agentCalc.ServiceMember.CommissionSumMonth = 0;

                        agentCalc.ServiceNonMember.TargetSumMonth = 0;
                        agentCalc.ServiceNonMember.CommissionSumMonth = 0;

                        agentCalc.CompoundingMember.TargetSumMonth = 0;
                        agentCalc.CompoundingMember.CommissionSumMonth = 0;

                        agentCalc.CompoundingNonMember.TargetSumMonth = 0;
                        agentCalc.CompoundingNonMember.CommissionSumMonth = 0;

                        #endregion


                        // add totalSalesPerMonth to list
                        totalSalesPerMonths.Add(totalSalesPerMonth);

                        #region reset target period && reset to base

                        #region MemberShip
                        if(agentCalc.Membership.TargetPeriodMonth >= agentCalc.Membership.TargetPeriod)
                        {
                            // set target period to default
                            agentCalc.Membership.TargetPeriodMonth = 0;
                            agentCalc.Membership.TargetSum = 0;
                        }

                        if(agentCalc.Membership.ResetToBaseMonth >= agentCalc.Membership.ResetToBase)
                        {
                            agentCalc.Membership.ResetToBase = 0;
                            agentCalc.Membership.BaseCommission = SaleTypesCalcBase.Membership.BaseCommission;
                        }
                        #endregion

                        #region ServiceMember
                        if (agentCalc.ServiceMember.TargetPeriodMonth >= agentCalc.ServiceMember.TargetPeriod)
                        {
                            // set target period to default
                            agentCalc.ServiceMember.TargetPeriodMonth = 0;
                            agentCalc.ServiceMember.TargetSum = 0;
                        }

                        if (agentCalc.ServiceMember.ResetToBaseMonth >= agentCalc.ServiceMember.ResetToBase)
                        {
                            // set reset to base to default
                            agentCalc.ServiceMember.ResetToBaseMonth = 0;
                            // set percent commission to base
                            agentCalc.ServiceMember.BaseCommission = SaleTypesCalcBase.ServiceMember.BaseCommission;
                        }
                        #endregion

                        #region ServiceNonMember
                        if (agentCalc.ServiceNonMember.TargetPeriodMonth >= agentCalc.ServiceNonMember.TargetPeriod)
                        {
                            // set target period to default
                            agentCalc.ServiceNonMember.TargetPeriodMonth = 0;
                            agentCalc.ServiceNonMember.TargetSum = 0;
                        }

                        if (agentCalc.ServiceNonMember.ResetToBaseMonth >= agentCalc.ServiceNonMember.ResetToBase)
                        {
                            // set reset to base to default
                            agentCalc.ServiceNonMember.ResetToBaseMonth = 0;
                            // set percent commission to base
                            agentCalc.ServiceNonMember.BaseCommission = SaleTypesCalcBase.ServiceNonMember.BaseCommission;
                        }
                        #endregion

                        #region CompoundingMember
                        if (agentCalc.CompoundingMember.TargetPeriodMonth >= agentCalc.CompoundingMember.TargetPeriod)
                        {
                            // set target period to default
                            agentCalc.CompoundingMember.TargetPeriodMonth = 0;
                            agentCalc.CompoundingMember.TargetSum = 0;
                        }

                        if (agentCalc.CompoundingMember.ResetToBaseMonth >= agentCalc.CompoundingMember.ResetToBase)
                        {
                            // set reset to base to default
                            agentCalc.CompoundingMember.ResetToBaseMonth = 0;
                            // set percent commission to base
                            agentCalc.CompoundingMember.BaseCommission = SaleTypesCalcBase.CompoundingMember.BaseCommission;
                        }
                        #endregion

                        #region CompoundingNonMember
                        if (agentCalc.CompoundingNonMember.TargetPeriodMonth >= agentCalc.CompoundingNonMember.TargetPeriod)
                        {
                            // set target period to default
                            agentCalc.CompoundingNonMember.TargetPeriodMonth = 0;
                            agentCalc.CompoundingNonMember.TargetSum = 0;
                        }

                        if (agentCalc.CompoundingNonMember.ResetToBaseMonth >= agentCalc.CompoundingNonMember.ResetToBase)
                        {
                            // set reset to base to default
                            agentCalc.CompoundingNonMember.ResetToBaseMonth = 0;
                            // set percent commission to base
                            agentCalc.CompoundingNonMember.BaseCommission = SaleTypesCalcBase.CompoundingNonMember.BaseCommission;
                        }
                        #endregion

                        #endregion end reset target period && reset to base

                        // add month
                        start = start.AddMonths(1);

                        totalSalesPerYear = new TotalSalesPerYearViewModel
                        {
                            Year = year,
                            TotalSalesPerMonth = totalSalesPerMonths,
                            TotalSales = decimal.Round(totalSalesYear, 2, MidpointRounding.AwayFromZero),
                            TotalCommission = decimal.Round(totalCommissionYear, 2, MidpointRounding.AwayFromZero)
                        };
                    }
                }

                totalSalesAgent += decimal.Round(totalSalesYear, 2, MidpointRounding.AwayFromZero);
                totalCommissionAgent += decimal.Round(totalCommissionYear, 2, MidpointRounding.AwayFromZero);
                totalSalesPerYears.Add(totalSalesPerYear);
            }

            var result = new AgentViewModel
            {
                AgentName = agent.AgentDesc,
                TotalSalesPerYear = totalSalesPerYears,
                TotalSales = decimal.Round(totalSalesAgent, 2, MidpointRounding.AwayFromZero),
                TotalCommission = decimal.Round(totalCommissionAgent, 2, MidpointRounding.AwayFromZero)
            };

            return result;
        }

        private static AgentCalc GetSaleTypesCalc(this IEnumerable<SaleTypesOutput> agentSaleTypes)
        {
            var membershipObject = agentSaleTypes
               .Where(d => d.SaleTypeId == (int)SaleTypeEnum.Membership)
               .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum }).FirstOrDefault();

            var serviceMemberObject = agentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.ServiceMember)
                .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum }).FirstOrDefault();

            var serviceNonMemberObject = agentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.ServiceNonMember)
                .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum }).FirstOrDefault();

            var compoundingMemberObject = agentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.CompoundingMember)
                .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum }).FirstOrDefault();

            var compoundingNonMemberObject = agentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.CompoundingNonMember)
                .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum }).FirstOrDefault();

            // set base calc 
            SaleTypesCalcBase.Membership = membershipObject;
            SaleTypesCalcBase.ServiceMember = serviceMemberObject;
            SaleTypesCalcBase.ServiceNonMember = serviceNonMemberObject;
            SaleTypesCalcBase.CompoundingMember = compoundingMemberObject;
            SaleTypesCalcBase.CompoundingNonMember = compoundingNonMemberObject;

            // set result
            var agentCalc = new AgentCalc
            {
                Membership = new Calc
                {
                    BaseCommission = membershipObject.BaseCommission,
                    Target = membershipObject.Target,
                    TargetSum = 0,
                    TargetPeriod = membershipObject.TargetPeriod,
                    TargetPeriodMonth = 0,
                    IncreaseIfTargetMet = membershipObject.IncreaseIfTargetMet,
                    Maximum = membershipObject.Maximum,
                    ResetToBase = membershipObject.ResetToBase,
                    ResetToBaseMonth = 0
                },
                ServiceMember = new Calc
                {
                    BaseCommission = serviceMemberObject.BaseCommission,
                    Target = serviceMemberObject.Target,
                    TargetSum = 0,
                    TargetPeriod = serviceMemberObject.TargetPeriod,
                    TargetPeriodMonth = 0,
                    IncreaseIfTargetMet = serviceMemberObject.IncreaseIfTargetMet,
                    Maximum = serviceMemberObject.Maximum,
                    ResetToBase = serviceMemberObject.ResetToBase,
                    ResetToBaseMonth = 0
                },
                ServiceNonMember = new Calc
                {
                    BaseCommission = serviceNonMemberObject.BaseCommission,
                    Target = serviceNonMemberObject.Target,
                    TargetSum = 0,
                    TargetPeriod = serviceNonMemberObject.TargetPeriod,
                    TargetPeriodMonth = 0,
                    IncreaseIfTargetMet = serviceNonMemberObject.IncreaseIfTargetMet,
                    Maximum = serviceNonMemberObject.Maximum,
                    ResetToBase = serviceNonMemberObject.ResetToBase,
                    ResetToBaseMonth = 0
                },
                CompoundingMember = new Calc
                {
                    BaseCommission = compoundingMemberObject.BaseCommission,
                    Target = compoundingMemberObject.Target,
                    TargetSum = 0,
                    TargetPeriod = compoundingMemberObject.TargetPeriod,
                    TargetPeriodMonth = 0,
                    IncreaseIfTargetMet = compoundingMemberObject.IncreaseIfTargetMet,
                    Maximum = compoundingMemberObject.Maximum,
                    ResetToBase = compoundingMemberObject.ResetToBase,
                    ResetToBaseMonth = 0
                },
                CompoundingNonMember = new Calc
                {
                    BaseCommission = compoundingNonMemberObject.BaseCommission,
                    Target = compoundingNonMemberObject.Target,
                    TargetSum = 0,
                    TargetPeriod = compoundingNonMemberObject.TargetPeriod,
                    TargetPeriodMonth = 0,
                    IncreaseIfTargetMet = compoundingNonMemberObject.IncreaseIfTargetMet,
                    Maximum = compoundingNonMemberObject.Maximum,
                    ResetToBase = compoundingNonMemberObject.ResetToBase,
                    ResetToBaseMonth = 0
                }
            };

            return agentCalc;
        }

    }
}
