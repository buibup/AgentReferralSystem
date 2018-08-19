using EPPlus.Core.Extensions;
using System;
using System.Collections.Generic;

namespace AgentReferralSystem.Api.Data.ViewModels
{
    [ExcelWorksheet("Agent")]
    public class AgentViewModel
    {
        [ExcelTableColumn("AgentName")]
        public string AgentName { get; set; }
        public List<TotalSalesPerYearViewModel> TotalSalesPerYear { get; set; }
        [ExcelTableColumn("TotalSales")]
        public decimal TotalSales { get; set; }
        [ExcelTableColumn("TotalCommission")]
        public decimal TotalCommission { get; set; }
    }

    public class TotalSalesPerYearViewModel
    {
        public int Year { get; set; }
        public List<TotalSalesPerMonthViewModel> TotalSalesPerMonth { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalCommission { get; set; }
    }

    public class TotalSalesPerMonthViewModel
    {
        public List<SaleDetailViewModel> SaleDetails { get; set; }
        public Month Month { get; set; }
        public int MembershipCount { get; set; }
        public int BWCServicesCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal MembershipSum { get; set; }
        public decimal MembershipSumCommission { get; set; }
        public decimal ServiceMemberSum { get; set; }
        public decimal ServiceMemberSumCommission { get; set; }
        public decimal ServiceNonMemberSum { get; set; }
        public decimal ServiceNonMemberSumCommission { get; set; }
        public decimal CompoundingMemberSum { get; set; }
        public decimal CompoundingMemberSumCommission { get; set; }
        public decimal CompoundingNonMemberSum { get; set; }
        public decimal CompoundingNonMemberSumCommission { get; set; }
        public decimal CommissionSum { get; set; }
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
