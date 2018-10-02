using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.Cache
{
    public class ARPatientBill
    {
        public int ARPBL_RowId { get; set; }
        public int ARPBL_PAADM_DR { get; set; }
        public int PAADM_PAPMI_DR { get; set; }
        public string PAPMI_Name { get; set; }
        public string PAPMI_Name2 { get; set; }
        public string PAPMI_No { get; set; }
        public string EpisodeNo { get; set; }
        public DateTime EpisodeDate { get; set; }
        public string CTPCP_Code { get; set; }
        public string CTPCP_Desc { get; set; }
        public DateTime? DischargeDate { get; set; } 
        public DateTime? BillPrintedDate { get; set; }
        public string INST_Desc { get; set; }
        public string ARPBL_BillNo { get; set; }
        public decimal ARPBL_TotalInsCo { get; set; }
        public decimal ARPBL_TotalPatient { get; set; }
        public decimal ARPBL_TotalPatientOfAllowed { get; set; }
        public decimal ARPBL_TotalServiceAllowed { get; set; }
        public decimal ARPBL_TotalSpecialist { get; set; }
        public string ARCIM_RowId { get; set; }
        public string ARCIM_Code { get; set; }
        public string ARCIM_Desc { get; set; }
        public decimal ITM_LineTotal { get; set; }
        public decimal ITM_PatientShare { get; set; }
        public decimal ITM_InsCompanyShare { get; set; }
        public decimal ITM_LocalGovtShare { get; set; }
        public decimal ITM_SpecialistSurcharge { get; set; }
        public decimal ITM_UnitPrice { get; set; }
        public int REFT_RowId { get; set; }
        public string REFT_Code { get; set; }
        public string REFT_Desc { get; set; }

        //Calculated Zone
        public decimal CommissionAmount { get; set; }
    }
}
