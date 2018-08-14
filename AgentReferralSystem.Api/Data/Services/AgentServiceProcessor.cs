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
        public static AgentViewModel AgentViewModelProcess(this Agent agent,
            IEnumerable<ARPatientBill> patientBills,
            IEnumerable<QBWCMEMBERS> memberRegisList,
            IEnumerable<ARCItmMast> itemCompoundingList)
        {
            #region create variable 
            var start = agent.StartDate;
            var years = patientBills.Select(d => d.EpisodeDate.Year).Distinct();
            var totalSalesPerMonths = new List<TotalSalesPerMonthViewModel>();
            var totalSalesPerYears = new List<TotalSalesPerYearViewModel>();
            decimal totalSalesAgent = 0;
            decimal totalCommissionAgent = 0;

            var agentBase = agent;
            var agentsSaleTypes = agentBase.AgentSaleTypes;
            #endregion

            #region set object service
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
                    decimal totalSale = 0;

                    if (year == currentYear)
                    {
                        // filter patient on current month
                        var patientBillListOnCurrent = patientBills.Where(pt => (pt.EpisodeDate.Year == currentYear) && (pt.EpisodeDate.Month == currentMonth)).ToList();

                        // count membership on current month
                        var membersThisMonth = memberRegisList.Where(m => (m.QDateFrom.Year == currentYear) && (m.QDateFrom.Month == currentMonth)).ToList();

                        // count all patient bwc services on current month
                        var papmiDRList = patientBillListOnCurrent.Select(p => p.PAADM_PAPMI_DR).Distinct().ToList();

                        var memberPapmiDrList = memberRegisList.Select(m => m.QUESPAPatMasDR).Distinct().ToList();

                        // patient member
                        var patientBillMemberList = patientBillListOnCurrent.Where(p => memberPapmiDrList.Any(p2 => p2 == p.PAADM_PAPMI_DR)).ToList();

                        // patient non member
                        var patientBillNonmemberList = patientBillListOnCurrent.Where(p => !memberPapmiDrList.Any(p2 => p2 == p.PAADM_PAPMI_DR)).ToList();

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
                            if (memberPapmiDrList.Any(m => item.PAADM_PAPMI_DR == m)) // member
                            {
                                if (itemCompoundingList.Any(i => i.ARCIM_RowId == item.ARCIM_RowId)) // compounding 
                                {
                                    agentCalc.CompoundingMember.TargetSum += item.ITM_LineTotal;
                                    model.CompoundingMember = item.ITM_LineTotal;
                                    model.Commission = Decimal.Divide(Decimal.Multiply(item.ITM_LineTotal,agentCalc.CompoundingMember.BaseCommission),100);
                                }
                                else  // services
                                {
                                    agentCalc.ServiceMember.TargetSum += item.ITM_LineTotal;
                                    model.ServiceMember = item.ITM_LineTotal;
                                    model.Commission = Decimal.Divide(Decimal.Multiply(item.ITM_LineTotal,agentCalc.ServiceMember.BaseCommission),100);
                                }
                            }
                            else // non member
                            {
                                if (itemCompoundingList.Any(i => i.ARCIM_RowId == item.ARCIM_RowId)) // compounding 
                                {
                                    agentCalc.CompoundingNonMember.TargetSum += item.ITM_LineTotal;
                                    model.CompoundingNonMember = item.ITM_LineTotal;
                                    model.Commission = Decimal.Divide(Decimal.Multiply(item.ITM_LineTotal,agentCalc.CompoundingNonMember.BaseCommission),100);
                                }
                                else  // services
                                {
                                    agentCalc.ServiceNonMember.TargetSum += item.ITM_LineTotal;
                                    model.ServiceNonMember = item.ITM_LineTotal;
                                    model.Commission = Decimal.Divide(Decimal.Multiply(item.ITM_LineTotal,agentCalc.ServiceNonMember.BaseCommission),100);
                                }
                            }
                            totalSale += model.Commission;
                            saleDetailList.Add(model);
                        }
                        #endregion

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
                        agentCalc.ServiceMember.Commission = (agentCalc.ServiceMember.TargetSum * agentCalc.ServiceMember.BaseCommission) / 100;


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
                        agentCalc.ServiceNonMember.Commission = (agentCalc.ServiceNonMember.TargetSum * agentCalc.ServiceNonMember.BaseCommission) / 100;

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
                        agentCalc.CompoundingMember.Commission = (agentCalc.CompoundingMember.TargetSum * agentCalc.CompoundingMember.BaseCommission) / 100;

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
                        agentCalc.CompoundingNonMember.Commission = (agentCalc.CompoundingNonMember.TargetSum * agentCalc.CompoundingNonMember.BaseCommission) / 100;

                        #endregion

                        #endregion end target met

                        var commissionSumMonth = agentCalc.ServiceMember.Commission + agentCalc.ServiceNonMember.Commission +
                                agentCalc.CompoundingMember.Commission + agentCalc.CompoundingNonMember.Commission;

                        totalSalesYear += agentCalc.ServiceMember.TargetSum + agentCalc.ServiceNonMember.TargetSum +
                            agentCalc.CompoundingMember.TargetSum + agentCalc.CompoundingNonMember.TargetSum;
                        totalCommissionYear += commissionSumMonth;

                        totalSalesAgent += totalSalesYear;
                        totalCommissionAgent += totalCommissionYear;

                        var totalSalesPerMonth = new TotalSalesPerMonthViewModel
                        {
                            Month = (Month)currentMonth,
                            SaleDetails = saleDetailList,
                            MembershipCount = membersThisMonth.Count,
                            BWCServicesCount = papmiDRList.Count,
                            TotalSales = totalSale,
                            ServiceMemberSum = agentCalc.ServiceMember.TargetSum,
                            ServiceMemberSumCommission = agentCalc.ServiceMember.Commission,
                            ServiceNonMemberSum = agentCalc.ServiceNonMember.TargetSum,
                            ServiceNonMemberSumCommission = agentCalc.ServiceNonMember.Commission,
                            CompoundingMemberSum = agentCalc.CompoundingMember.TargetSum,
                            CompoundingMemberSumCommission = agentCalc.CompoundingMember.Commission,
                            CompoundingNonMemberSum = agentCalc.CompoundingNonMember.TargetSum,
                            CompoundingNonMemberSumCommission = agentCalc.CompoundingNonMember.Commission,
                            CommissionSum = commissionSumMonth
                        };


                        // add totalSalesPerMonth to list
                        totalSalesPerMonths.Add(totalSalesPerMonth);

                        #region reset target period && reset to base

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
                            agentCalc.ServiceMember.BaseCommission = serviceMemberObject.BaseCommission;
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
                            agentCalc.ServiceNonMember.BaseCommission = serviceNonMemberObject.BaseCommission;
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
                            agentCalc.CompoundingMember.BaseCommission = compoundingMemberObject.BaseCommission;
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
                            agentCalc.CompoundingNonMember.BaseCommission = compoundingNonMemberObject.BaseCommission;
                        }
                        #endregion

                        #endregion end reset target period && reset to base

                        // add month
                        start = start.AddMonths(1);

                        totalSalesPerYear = new TotalSalesPerYearViewModel
                        {
                            Year = year,
                            TotalSalesPerMonth = totalSalesPerMonths,
                            TotalSales = totalSalesYear,
                            TotalCommission = totalCommissionYear
                        };
                    }
                }

                totalSalesPerYears.Add(totalSalesPerYear);
            }

            var result = new AgentViewModel
            {
                AgentName = agent.AgentDesc,
                TotalSalesPerYear = totalSalesPerYears,
                TotalSales = totalSalesAgent,
                TotalCommission = totalCommissionAgent
            };

            return result;
        }
    }
}
