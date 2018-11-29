using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.Cache
{
    public class ARPatientBill
    {
        public string ARPBL_RowId { get; set; }
        public string ARPBL_PAADM_DR { get; set; }
        public string PAADM_PAPMI_DR { get; set; }
        public string PAPMI_RowId { get; set; }
        public string PAPMI_ID { get; set; }
        public string PAPMI_No { get; set; }
        public string PAPMI_Name { get; set; }
        public string PAPMI_Name2 { get; set; }
        public int CTSEX_RowId { get; set; }
        public string CTSEX_Code { get; set; }
        public string CTSEX_Desc { get; set; }
        public string CTSEX_CodeTranslated { get; set; }
        public string CTSEX_DescTranslated { get; set; }
        public string PAPMI_DOB { get; set; }
        public string PAPMI_ForeignPhoneNo { get; set; }
        public int PCAT_RowId { get; set; }
        public string PCAT_Code { get; set; }
        public string PCAT_Desc { get; set; }
        public string PAPMI_Alias { get; set; }
        public string PAPMI_IPNo { get; set; }
        public string PAPMI_OPNo { get; set; }
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
        public DateTime? REFT_DateFrom { get; set; }
        public DateTime? REFT_DateTo { get; set; }
        public string REFT_NationalCode { get; set; }
        public string REFT_RefStDateApptDate { get; set; }
        public string REFT_Owner { get; set; }
        public string REFT_CodeTableTags { get; set; }
        public int REFT_Subregion_DR { get; set; }

        //Calculated Zone
        public decimal CommissionAmount { get; set; }
    }
}
