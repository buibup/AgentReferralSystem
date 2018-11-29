using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class CommissionItem
    {
        public int Id { get; set; }
        public string ARPBL_RowId { get; set; }
        public int Agent_Id { get; set; }
        public string Agent_Code { get; set; }
        public string Agent_Name { get; set; }
        public string Patient_Name { get; set; }
        public string Patient_Desc { get; set; }
        public string PatientId { get; set; }
        public string HN_Number { get; set; }
        public string Episode_Number { get; set; }
        public DateTime Episode_Date { get; set; }
        public string Episode_Time { get; set; }
        public string Doctor_Code { get; set; }
        public string Doctor_Name { get; set; }
        public DateTime? Discharge_Date { get; set; }
        public DateTime? BillPrinted_Date { get; set; }  
        public string PatientBill_Number { get; set; }
        public string Bill_Type { get; set; }
        public string check_PaidInvoice { get; set; }
        public string Item_Id { get; set; }
        public string Item_Code { get; set; }
        public string Item_Desc { get; set; }
        public decimal Item_Total { get; set; }
        public decimal Item_Commission { get; set; }
        public bool isDelete { get; set; }
        public DateTime CreateDate { get; set; }
        
    }
}
