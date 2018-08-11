using System;
using System.Collections.Generic;

namespace AgentReferralSystem.Api.Data.ViewModels
{
    public class AgentViewModel
    {
        public string AgentName { get; set; }
        public List<TotalSalesPerYearViewModel> TotalSalesPerYear { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class TotalSalesPerYearViewModel
    {
        public int Year { get; set; }
        public List<TotalSalesPerMonthViewModel> TotalSalesPerMonth { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class TotalSalesPerMonthViewModel
    {
        public List<SaleDetailViewModel> SaleDetails { get; set; }
        public Month Month { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class SaleDetailViewModel
    {
        public DateTime Date { get; set; }
        public string HN { get; set; }
        public string Episode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Membership { get; set; }
        public decimal ServiceMember { get; set; }
        public decimal ServiceNonMember { get; set; }
        public decimal CompoundingMember { get; set; }
        public decimal CompoundingNonMember { get; set; }
        public decimal Commission { get; set; }
    }

}
