using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Query
{
    public static class CacheQuery
    {
        public static string GetPACReferralTypeAll(string search)
        {
            string query = string.Empty;

            if (string.IsNullOrEmpty(search))
            {
                query = @"
                SELECT REFT_RowId, REFT_Code, REFT_Desc
                FROM PAC_ReferralType
                WHERE REFT_DateTo >= CURRENT_DATE OR REFT_DateTo IS NULL
                ORDER BY REFT_Desc ASC";
            }
            else
            {
                query = $@"SELECT REFT_RowId, REFT_Code, REFT_Desc
                FROM PAC_ReferralType
                WHERE REFT_DateTo >= CURRENT_DATE OR REFT_DateTo IS NULL
                AND REFT_Desc LIKE '%{search}%'
                ORDER BY REFT_Desc ASC";
            }


            return query;
        }

        public static string GetARPatientsBillsByReferralTypeRowId()
        {
            return @"
                SELECT DISTINCT 
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_No,
                    ARPBL_PAADM_DR,
                    ARPBL_PAADM_DR->PAADM_ADMNO EpisodeNo,
	                ARPBL_AdmDate EpisodeDate,
	                ARPBL_DischDate DischargeDate,
	                ARPBL_DatePrinted BillPrintedDate,
	                ARPBL_InsuranceType_DR->INST_Desc,
	                ARPBL_BillNo,
	                ARPBL_TotalInsCo,
	                ARPBL_TotalPatient,
	                ARPBL_TotalPatientOfAllowed,
	                ARPBL_TotalServiceAllowed,
	                ARPBL_TotalSpecialist,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_RowId,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_Code,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_Desc,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LineTotal,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_PatientShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_InsCompanyShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LocalGovtShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_SpecialistSurcharge,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_UnitPrice,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId, 
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Code, 
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Desc
                FROM AR_PatientBill
                WHERE ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId = ?
                AND ARPBL_ReasonCancel_DR IS NULL
                AND AR_PatBillPaymAlloc->PAYM_ARCIM_DR->ARCIM_RowId IS NOT NULL";
        }

        public static string GetARCItmMastCompounding()
        {
            return @"
                SELECT ARCIM_RowId, ARCIM_BillSub_DR 
                FROM ARC_ItmMast
                WHERE ARCIM_BillSub_DR = '42||13'";
        }

        public static string GetQBWCMEMBERSByPapmiRowId()
        {
            return @"
                SELECT ID, QUESPAAdmDR, QUESPAPatMasDR,
	                QDateFrom, QDateTO
                FROM questionnaire.QBWCMEMBERS	
                WHERE QUESPAPatMasDR = ?
                ORDER BY ID DESC";
        }

        public static string GetQBWCMEMBERSByPapmiRowIdList(IEnumerable<int> rowIdList)
        {
            string _rowIdList = "";

            foreach(var id in rowIdList)
            {
                if (string.IsNullOrEmpty(_rowIdList))
                {
                    _rowIdList += id;
                }
                else
                {
                    _rowIdList += $", {id}";
                }
            }

            var query = $@"
                SELECT ID, QUESPAAdmDR, QUESPAPatMasDR,
	                QDateFrom, QDateTO
                FROM questionnaire.QBWCMEMBERS	
                WHERE QUESPAPatMasDR IN ({_rowIdList})";

            return query;
        }
    }
}
