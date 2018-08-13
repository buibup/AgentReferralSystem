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

            while (start < agent.EndDate)
            {
                var saleDetailList = new List<SaleDetailViewModel>();
                var currentYear = start.Year;
                var currentMonth = start.Month;
                decimal totalSale = 0;

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


                // add item member
                foreach (var item in patientBillMemberList)
                {
                    decimal commisson = 0;
                    if (itemCompoundingList.Any(i => i.ARCIM_RowId == item.ARCIM_RowId)) // compounding member
                    {
                        try
                        {
                            commisson = (item.ITM_LineTotal * agentCalc.CompoundingMember.BaseCommission) / 100;
                        }
                        catch (Exception)
                        {
                            commisson = 0;
                        }
                        var model = new SaleDetailViewModel
                        {
                            HN = item.PAPMI_No,
                            Episode = item.EpisodeNo,
                            Date = item.EpisodeDate,
                            Membership = 0,
                            ServiceMember = 0,
                            ServiceNonMember = 0,
                            CompoundingMember = item.ITM_LineTotal,
                            CompoundingNonMember = 0,
                            TotalAmount = 0,
                            Commission = commisson
                        };

                        // sum target bath
                        agentCalc.CompoundingMember.TargetSum += commisson;

                        saleDetailList.Add(model);
                    }
                    else // service member
                    {
                        try
                        {
                            commisson = (item.ITM_LineTotal * agentCalc.ServiceMember.BaseCommission) / 100;
                        }
                        catch (Exception)
                        {
                            commisson = 0;
                        }
                        var model = new SaleDetailViewModel
                        {
                            HN = item.PAPMI_No,
                            Episode = item.EpisodeNo,
                            Date = item.EpisodeDate,
                            Membership = 0,
                            ServiceMember = item.ITM_LineTotal,
                            ServiceNonMember = 0,
                            CompoundingMember = 0,
                            CompoundingNonMember = 0,
                            TotalAmount = 0,
                            Commission = commisson
                        };

                        // sum target bath
                        agentCalc.ServiceMember.TargetSum += commisson;

                        saleDetailList.Add(model);
                    }

                    totalSale += commisson;
                }

                // add item non member
                foreach (var item in patientBillNonmemberList)
                {
                    decimal commisson = 0;
                    if (itemCompoundingList.Any(i => i.ARCIM_RowId == item.ARCIM_RowId)) // compounding nonmember
                    {
                        try
                        {
                            commisson = (item.ITM_LineTotal * agentCalc.CompoundingNonMember.BaseCommission) / 100;
                        }
                        catch (Exception)
                        {
                            commisson = 0;
                        }

                        var model = new SaleDetailViewModel
                        {
                            HN = item.PAPMI_No,
                            Episode = item.EpisodeNo,
                            Date = item.EpisodeDate,
                            Membership = 0,
                            ServiceMember = 0,
                            ServiceNonMember = 0,
                            CompoundingMember = 0,
                            CompoundingNonMember = item.ITM_LineTotal,
                            TotalAmount = 0,
                            Commission = commisson
                        };

                        // sum target bath
                        agentCalc.CompoundingNonMember.TargetSum += commisson;

                        saleDetailList.Add(model);
                    }
                    else // service nonmember
                    {
                        try
                        {
                            commisson = (item.ITM_LineTotal * agentCalc.ServiceNonMember.BaseCommission) / 100;
                        }
                        catch (Exception)
                        {
                            commisson = 0;
                        }

                        var model = new SaleDetailViewModel
                        {
                            HN = item.PAPMI_No,
                            Episode = item.EpisodeNo,
                            Date = item.EpisodeDate,
                            Membership = 0,
                            ServiceMember = 0,
                            ServiceNonMember = item.ITM_LineTotal,
                            CompoundingMember = 0,
                            CompoundingNonMember = 0,
                            TotalAmount = 0,
                            Commission = commisson
                        };

                        // sum target bath
                        agentCalc.ServiceNonMember.TargetSum += commisson;

                        saleDetailList.Add(model);
                    }

                    totalSale += commisson;
                } 


                

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
                // is service member target met 
                if (agentCalc.ServiceMember.TargetSum >= agentCalc.ServiceMember.Target)
                {
                    // check maximum
                    if(agentCalc.ServiceMember.BaseCommission < agentCalc.ServiceMember.Maximum)
                    {
                        // add base commission
                        agentCalc.ServiceMember.BaseCommission += agentCalc.ServiceMember.IncreaseIfTargetMet;
                    }
                }
                // calc service member commission
                agentCalc.ServiceMember.Commission = (agentCalc.ServiceMember.TargetSum * agentCalc.ServiceMember.BaseCommission) / 100;

                if(agentCalc.ServiceMember.ResetToBaseMonth >= agentCalc.ServiceMember.ResetToBase)
                {
                    // reset service member
                }


                // is non service member target met 
                if (agentCalc.ServiceNonMember.TargetSum >= agentCalc.ServiceNonMember.Target)
                {
                    // check maximum
                    if (agentCalc.ServiceNonMember.BaseCommission < agentCalc.ServiceNonMember.Maximum)
                    {
                        // add base commission
                        agentCalc.ServiceNonMember.BaseCommission += agentCalc.ServiceNonMember.IncreaseIfTargetMet;
                    }
                }

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

                // is non compounding member target met  
                if (agentCalc.CompoundingNonMember.TargetSum >= agentCalc.CompoundingNonMember.Target)
                {
                    // check maximum
                    if (agentCalc.CompoundingNonMember.BaseCommission < agentCalc.CompoundingNonMember.Maximum)
                    {
                        // add base commission
                        agentCalc.CompoundingNonMember.BaseCommission += agentCalc.CompoundingNonMember.IncreaseIfTargetMet;
                    }
                }
                #endregion

                var totalSalesPerMonth = new TotalSalesPerMonthViewModel
                {
                    Month = (Month)currentMonth,
                    SaleDetails = saleDetailList,
                    MembershipCount = membersThisMonth.Count,
                    BWCServicesCount = papmiDRList.Count,
                    TotalSales = totalSale,

                };

                totalSalesPerMonths.Add(totalSalesPerMonth);

                // add month
                start = agent.StartDate.AddMonths(1);

                // reset to base

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

        private static bool IsServicesTargetMet()
        {
            var result = false;


            return result;
        }
    }
}
