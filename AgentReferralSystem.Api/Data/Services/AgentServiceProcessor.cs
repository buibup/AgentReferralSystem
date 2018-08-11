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
        public static AgentViewModel AgentViewModelProcess(this Agent agent, IEnumerable<ARPatientBill> patientBills, IEnumerable<QBWCMEMBERS> bWCMEMBERs)
        {
            var years = patientBills.Select(d => d.EpisodeDate.Year).Distinct();
            var totalSalesPerMonths = new List<TotalSalesPerMonthViewModel>();


            foreach(var year in years)
            {
                foreach (var month in (Month[])Enum.GetValues(typeof(Month)))
                {
                    decimal totalSales = 0;
                    var saleDetails = new List<SaleDetailViewModel>();

                    switch (month)
                    {
                        case Month.January:
                            var data = patientBills.Where(d => d.EpisodeDate.Year == year).ToList();
                            break;
                        case Month.February:
                            break;
                        case Month.March:
                            break;
                        case Month.April:
                            break;
                        case Month.May:
                            break;
                        case Month.June:
                            break;
                        case Month.July:
                            break;
                        case Month.August:
                            break;
                        case Month.September:
                            break;
                        case Month.October:
                            break;
                        case Month.November:
                            break;
                        case Month.December:
                            break;
                        default:
                            break;
                    }


                    var totalSalesPerMonth = new TotalSalesPerMonthViewModel
                    {
                        Month = month,
                        SaleDetails = saleDetails,
                        TotalSales = totalSales
                    };

                    totalSalesPerMonths.Add(totalSalesPerMonth);
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

        private static List<SaleDetailViewModel> PatientBillListToSaleDetailList(this List<ARPatientBill> patientBills)
        {
            var result = new List<SaleDetailViewModel>();
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

            return result;
        }
    }
}
