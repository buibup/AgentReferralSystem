using AgentReferralSystem.Api.Data.Calculate;
using AgentReferralSystem.Api.Data.Models;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Models.Calc;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Moq;
using AgentReferralSystem.Api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace AgentReferralSystem.Api.Data.Services
{
    public static class AgentServiceProcessor
    {
        #region dynamicRate
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
            var agentStartDate = agent.StartDate;

            // get all year of patient
            var patientYears = patientBills.Select(d => d.EpisodeDate.Year).Distinct();

            // set agent base
            var agentBase = agent;
            //agentBase.RetroCutoffMonth = agentBase.StartDate.Month;
            var agentsSaleTypes = agentBase.AgentSaleTypes;
            #endregion

            // set agent calc
            var agentCalc = agentsSaleTypes.GetSaleTypesCalc();

            foreach (var patientYear in patientYears)
            {
                // set variable year
                decimal totalSalesYear = 0;
                decimal totalCommissionYear = 0;
                TotalSalesPerYearViewModel totalSalesPerYear = null;

                while (agentStartDate < agent.EndDate && agentStartDate.Year == patientYear)
                {
                    // set variable month
                    var saleDetailList = new List<SaleDetailViewModel>();
                    var agentStartYear = agentStartDate.Year;
                    var currentMonth = agentStartDate.Month;
                    decimal totalSalesMonth = 0;

                    if (patientYear == agentStartYear)
                    {
                        // filter patient on current month
                        var patientBillListOnCurrent = patientBills.Where(pt => (pt.EpisodeDate.Year == agentStartYear) && (pt.EpisodeDate.Month == currentMonth)).ToList();

                        // count membership on current month
                        var membersThisMonth = memberRegisList.Where(m => (m.QDateFrom.Year == agentStartYear) && (m.QDateFrom.Month == currentMonth)).Select(m => m.QUESPAPatMasDR).Distinct().ToList();

                        // count all patient bwc services on current month
                        var papmiDRList = patientBillListOnCurrent.Select(p => p.PAADM_PAPMI_DR).Distinct().ToList();

                        // distinct member regist
                        var memberPapmiDrList = memberRegisList.Select(m => m.QUESPAPatMasDR).Distinct().ToList();

                        var saleDetail = new SaleDetailViewModel();

                        #region calc membership
                        if (membersThisMonth.Count > 0)
                        {
                            var membersCalc = membersThisMonth.Count.MembersCalculate();
                            agentCalc.Membership.TargetSumMonth = membersCalc;
                            agentCalc.Membership.TargetSum += membersCalc;
                        }
                        #endregion

                        #region calculate service and add sale detail to list
                        foreach (var item in patientBillListOnCurrent)
                        {
                            var model = new SaleDetailViewModel
                            {
                                HN = item.PAPMI_No,
                                Episode = item.EpisodeNo,
                                Date = item.EpisodeDate,
                                ServiceMember = 0,
                                ServiceNonMember = 0,
                                CompoundingMember = 0,
                                CompoundingNonMember = 0,
                                TotalAmount = 0
                            };

                            // calculate service
                            item.SetSaleDetailViewModel(ref model, ref agentCalc, memberPapmiDrList, itemCompoundingList.ToList());

                            saleDetailList.Add(model);
                        }
                        #endregion

                        if (saleDetailList.Count > 0)
                        {
                            // group saleDetailList by episode no
                            saleDetailList = saleDetailList.SaleDetailsGroupByEpiNo();
                        }

                        totalSalesMonth = agentCalc.SumTargetOfMonth();

                        // add target period month
                        IncreaseTargetPeriodMonth(ref agentCalc);

                        // add reset to base month
                        IncreaseResetToBaseMonth(ref agentCalc);

                        // target met calc
                        CommonCalc.TargetMet(ref agentCalc);

                        // sum commission of month
                        var commissionSumMonth = agentCalc.SumCommissionOfMonth();

                        // sum total sales of year
                        totalSalesYear += agentCalc.SumTargetOfMonth();

                        // sum total commission of year
                        totalCommissionYear += commissionSumMonth;

                        // set values to TotalSalesPerMonthViewModel model
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
                            CompoundingMemberSumCommission = decimal.Round(agentCalc.CompoundingMember.CommissionSumMonth, 2, MidpointRounding.AwayFromZero),
                            CompoundingNonMemberSum = agentCalc.CompoundingNonMember.TargetSumMonth,
                            CompoundingNonMemberSumCommission = decimal.Round(agentCalc.CompoundingNonMember.CommissionSumMonth, 2, MidpointRounding.AwayFromZero),
                            CommissionSum = decimal.Round(commissionSumMonth, 2, MidpointRounding.AwayFromZero)
                        };

                        // reset month value
                        ResetAgentMonthValues(ref agentCalc);

                        // add totalSalesPerMonth to list
                        totalSalesPerMonths.Add(totalSalesPerMonth);

                        // reset target period && reset to base
                        ResetAgentTargetPeriodAndResetToBase(ref agentCalc);

                        // add month
                        agentStartDate = agentStartDate.AddMonths(1);

                        // set values to TotalSalesPerYearViewModel 
                        totalSalesPerYear = new TotalSalesPerYearViewModel
                        {
                            Year = patientYear,
                            TotalSalesPerMonth = totalSalesPerMonths,
                            TotalSalesYear = decimal.Round(totalSalesYear, 2, MidpointRounding.AwayFromZero),
                            TotalCommissionYear = decimal.Round(totalCommissionYear, 2, MidpointRounding.AwayFromZero)
                        };
                    }
                }
                
                totalSalesAgent += decimal.Round(totalSalesYear, 2, MidpointRounding.AwayFromZero);
                totalCommissionAgent += decimal.Round(totalCommissionYear, 2, MidpointRounding.AwayFromZero);
                totalSalesPerYears.Add(totalSalesPerYear);
            }

            // add values to result
            var result = new AgentViewModel
            {
                AgentName = agent.AgentDesc,
                TotalSalesPerYear = totalSalesPerYears,
                AgentTotalSales = decimal.Round(totalSalesAgent, 2, MidpointRounding.AwayFromZero),
                AgentTotalCommission = decimal.Round(totalCommissionAgent, 2, MidpointRounding.AwayFromZero)
            };

            return result;
        }

        private static AgentCalc GetSaleTypesCalc(this IEnumerable<SaleTypesOutput> agentSaleTypes)
        {
            var membershipObject = agentSaleTypes
               .Where(d => d.SaleTypeId == (int)SaleTypeEnum.Membership)
               .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum, ApplicableTargetInrease = d.ApplicableTargetInrease }).FirstOrDefault();

            var serviceMemberObject = agentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.ServiceMember)
                .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum, ApplicableTargetInrease = d.ApplicableTargetInrease }).FirstOrDefault();

            var serviceNonMemberObject = agentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.ServiceNonMember)
                .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum, ApplicableTargetInrease = d.ApplicableTargetInrease }).FirstOrDefault();

            var compoundingMemberObject = agentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.CompoundingMember)
                .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum, ApplicableTargetInrease = d.ApplicableTargetInrease }).FirstOrDefault();

            var compoundingNonMemberObject = agentSaleTypes
                .Where(d => d.SaleTypeId == (int)SaleTypeEnum.CompoundingNonMember)
                .Select(d => new TypesCalc { BaseCommission = d.BaseCommission, Target = d.Target, TargetPeriod = d.TargetPeriod, ResetToBase = d.ResetToBase, IncreaseIfTargetMet = d.IncreaseIfTargetMet, Maximum = d.Maximum, ApplicableTargetInrease = d.ApplicableTargetInrease }).FirstOrDefault();

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
                    ResetToBaseMonth = 0,
                    ApplicableTargetIncrease = membershipObject.ApplicableTargetInrease,
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
                    ResetToBaseMonth = 0,
                    ApplicableTargetIncrease = serviceMemberObject.ApplicableTargetInrease,
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
                    ResetToBaseMonth = 0,
                    ApplicableTargetIncrease = serviceNonMemberObject.ApplicableTargetInrease,
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
                    ResetToBaseMonth = 0,
                    ApplicableTargetIncrease = compoundingMemberObject.ApplicableTargetInrease,
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
                    ResetToBaseMonth = 0,
                    ApplicableTargetIncrease = compoundingNonMemberObject.ApplicableTargetInrease,
                }
            };

            return agentCalc;
        }

        private static List<SaleDetailViewModel> SaleDetailsGroupByEpiNo(this List<SaleDetailViewModel> models)
        {
            var result = new List<SaleDetailViewModel>();

            // distinct episode
            var epiNoList = models.Select(d => d.Episode).Distinct().ToList();

            // group data by episode no
            foreach (var item in epiNoList)
            {
                var data = models.Where(e => e.Episode == item);

                var hn = data.Select(p => p.HN).FirstOrDefault();
                var epiDate = data.Select(p => p.Date).FirstOrDefault();
                var serviceMember = data.Select(p => p.ServiceMember).Sum();
                var serviceNonMember = data.Select(p => p.ServiceNonMember).Sum();
                var compoundingMember = data.Select(p => p.CompoundingMember).Sum();
                var compoundingNonMember = data.Select(p => p.CompoundingNonMember).Sum();
                var totalAmount = serviceMember + serviceNonMember + compoundingMember + compoundingNonMember;

                var model = new SaleDetailViewModel
                {
                    HN = hn,
                    Episode = item,
                    Date = epiDate,
                    ServiceMember = serviceMember,
                    ServiceNonMember = serviceNonMember,
                    CompoundingMember = compoundingMember,
                    CompoundingNonMember = compoundingNonMember,
                    TotalAmount = totalAmount
                };

                result.Add(model);
            }

            return result;
        }

        private static void IncreaseTargetPeriodMonth(ref AgentCalc agentCalc)
        {
            agentCalc.Membership.TargetPeriodMonth += 1;
            agentCalc.ServiceMember.TargetPeriodMonth += 1;
            agentCalc.ServiceNonMember.TargetPeriodMonth += 1;
            agentCalc.CompoundingMember.TargetPeriodMonth += 1;
            agentCalc.CompoundingNonMember.TargetPeriodMonth += 1;
        }

        private static void IncreaseResetToBaseMonth(ref AgentCalc agentCalc)
        {
            agentCalc.Membership.ResetToBaseMonth += 1;
            agentCalc.ServiceMember.ResetToBaseMonth += 1;
            agentCalc.ServiceNonMember.ResetToBaseMonth += 1;
            agentCalc.CompoundingMember.ResetToBaseMonth += 1;
            agentCalc.CompoundingNonMember.ResetToBaseMonth += 1;
        }

        private static void ResetAgentMonthValues(ref AgentCalc agentCalc)
        {
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
        }

        private static void ResetAgentTargetPeriodAndResetToBase(ref AgentCalc agentCalc)
        {
            #region MemberShip
            if (agentCalc.Membership.TargetPeriodMonth >= agentCalc.Membership.TargetPeriod)
            {
                // set target period to default
                agentCalc.Membership.TargetPeriodMonth = 0;
                agentCalc.Membership.TargetSum = 0;
            }

            if (agentCalc.Membership.ResetToBaseMonth >= agentCalc.Membership.ResetToBase)
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
        }

        #endregion
                
        #region fixedRateCommission

        public static List<AgentReportViewModel> ProcessCommission(this AgentOutput agent,
            IEnumerable<ARPatientBill> patientBills,
            IEnumerable<CommissionItem> BillList,
            List<PercentConfig> ConfigList, 
            int Year,int Month)
        {
            // Get UnRegisPatientBill
            var UnRegisBill = CommonCalc.FilterUnRegisItem(patientBills, BillList.ToList());

            //GEN Report
            List<AgentReportViewModel> output = ProcessYearlyCommission(patientBills, ConfigList, Year, Month);

            //throw new Exception("Test"); 
            return output;
        }

        private static List<AgentReportViewModel> ProcessYearlyCommission(
            IEnumerable<ARPatientBill> patientBills,
            List<PercentConfig> ConfigList,
            int Year, int Month)
        {
            var result = new List<AgentReportViewModel>();
            var patientYears = patientBills.OrderBy(x => x.EpisodeDate).Select(d => d.EpisodeDate.Year).Distinct();
            int startYear = 0;
            int endYear = 0;
            if (Year != 0) { startYear = Year; endYear = Year; }
            else { startYear = patientYears.First(); endYear = patientYears.Last(); }
            for (int i = startYear; i <= endYear; i++)
            {
                var targetPatientBills = patientBills.Where(x => x.EpisodeDate.Year == i);
                if (targetPatientBills.Count() > 0)
                {
                    var yearResult = new AgentReportViewModel();
                    yearResult.year = i;
                    yearResult.CommissionBillList = ProcessMonthlyCommission(targetPatientBills, ConfigList, Month);
                    result.Add(yearResult);
                }
            }
            return result;
        }

        private static List<ComBillViewModel> ProcessMonthlyCommission(
            IEnumerable<ARPatientBill> patientBills,
            IEnumerable<PercentConfig> ConfigList,
            int Month)
        {
            int startMonth = 0;
            int endMonth = 0;   
            if (Month != 0) { startMonth = Month; endMonth = Month; }
            else { startMonth = 1; endMonth = 12; }
            var result = new List<ComBillViewModel>();
            for (int i = startMonth; i <= endMonth; i++)
            {
                var targetPatientBills = patientBills.Where(x => x.EpisodeDate.Month == i);
                if (targetPatientBills.Count() > 0)
                {
                    var output = CommonCalc.calculateComByItemList(targetPatientBills, ConfigList);
                    output.month = i;
                    result.Add(output);
                }
            }
            return result;
        }

        #endregion

        #region newFunction

        public static void ProcessMonthlyReward(List<AgentOverviewModel> AgentModelList, PercentConfig PC)
        {
            foreach(AgentOverviewModel AgentModel in AgentModelList)
            {
                //Get all Item from agent
                Agent agent = AgentModel.agent;
                List<CommissionItem> ItemList = AgentModel.saleList;
                decimal sumSaleAmount = 0;
                //Sum All Price
                foreach(CommissionItem item in ItemList)
                {
                    sumSaleAmount += item.Item_Total;
                }
                //Calculate Reward
                int TotalReward = 0;
                if (PC.value2Meaning.ToLower() == "percent") TotalReward = (int)decimal.Multiply(sumSaleAmount, decimal.Divide(PC.value1, 100));
                else TotalReward = (int)decimal.Divide(sumSaleAmount, PC.value1);
                //Insert to DB
                agent.CurrentReward += TotalReward;
                agent.TotalReward += TotalReward;
            }
        }

        public static void SaveImage(Agent agent, string ImageData)
        {
            //var filePath = "C:\\Users\\Kantinun\\Desktop\\AgentReferralSystem\\AgentReferralSystem.Api\\bin\\Release\\netcoreapp2.1\\publish\\wwwroot\\";
            var filePath = "C:\\inetpub\\wwwroot\\AgentReferralSystem\\publish\\wwwroot\\";

            try
            {
                if (System.IO.File.Exists(filePath + agent.DisplayImage))
                    System.IO.File.Delete(filePath + agent.DisplayImage);
                byte[] bytes = Convert.FromBase64String(ImageData);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    Image image = Image.FromStream(ms);
                    image.Save(filePath + agent.DisplayImage, ImageFormat.Jpeg);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
    