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
                AND REFT_Code like 'BWC%'
                ORDER BY REFT_Desc ASC";
            }
            else
            {
                query = $@"SELECT REFT_RowId, REFT_Code, REFT_Desc
                FROM PAC_ReferralType
                WHERE REFT_DateTo >= CURRENT_DATE OR REFT_DateTo IS NULL
                AND REFT_Code like 'BWC%'
                AND REFT_Desc LIKE '%{search}%'
                ORDER BY REFT_Desc ASC";
            }


            return query;
        }

        public static string GetPACReferralTypeById(int agentid)
        {
            string query = string.Empty;

            if (string.IsNullOrEmpty(agentid.ToString()))
            {
                query = @"
                SELECT REFT_RowId, REFT_Code, REFT_Desc
                FROM PAC_ReferralType
                WHERE REFT_DateTo >= CURRENT_DATE OR REFT_DateTo IS NULL
                AND REFT_Code like 'BWC%'
                ORDER BY REFT_Desc ASC";
            }
            else
            {
                query = $@"
                SELECT REFT_RowId, REFT_Code, REFT_Desc
                FROM PAC_ReferralType
                WHERE REFT_DateTo >= CURRENT_DATE OR REFT_DateTo IS NULL
                AND REFT_Code like 'BWC%'
                AND REFT_RowId = '{agentid}'
                ";
            }


            return query;
        }

        public static string GetARPatientsBillsByReferralTypeRowId()
        {
            return @"
                    SELECT DISTINCT 
                    ARPBL_RowId,
                    ARPBL_PAADM_DR,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_RowId,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_ID,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_No,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Name,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Name2,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_RowId,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_Code,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_Desc,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_CodeTranslated,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_DescTranslated,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_DOB,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_ForeignPhoneNo,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_PatCategory_DR->PCAT_RowId,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_PatCategory_DR->PCAT_Code,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_PatCategory_DR->PCAT_Desc,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Alias,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_IPNo, 
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_OPNo,
                    ARPBL_PAADM_DR->PAADM_ADMNO EpisodeNo,
	                ARPBL_AdmDate EpisodeDate,
	                ARPBL_PAADM_DR-> PAADM_AdmTime EpisodeTime,
                    ARPBL_PAADM_DR->PAADM_AdmDocCodeDR->CTPCP_Code,
                    ARPBL_PAADM_DR->PAADM_AdmDocCodeDR->CTPCP_Desc,
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
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_DerivedFeeItem,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_ItemCat_DR->ARCIC_Code,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_Desc,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_RowId,	                
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LineTotal,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_PatientShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_InsCompanyShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LocalGovtShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_SpecialistSurcharge,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_UnitPrice,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId, 
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Code, 
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Desc,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_DateFrom,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_DateTo,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_NationalCode,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RefStDateApptDate,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Owner,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_CodeTableTags,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Subregion_DR
	                
                FROM AR_PatientBill
                WHERE 
                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId = ? AND 
                AND ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Code like 'BWC%'
                AND ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId IS NOT NULL 
                AND ARPBL_ReasonCancel_DR IS NULL                               
                AND AR_PatBillPaymAlloc->PAYM_ARCIM_DR->ARCIM_RowId IS NOT NULL
             	AND isNull(AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LineTotal,0) > 0
             	and isNull(AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_DerivedFeeItem, 'Y') <> 'Y'
             	and isNull(AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_ItemCat_DR->ARCIC_Code,'') <> '1801WM'
             	Order By EpisodeDate DESC, EpisodeTime DESC
             	";
        }

        public static string GetARPatientsBillsByREFT_Code(string REFT_Code)
        {
            return $@"
                    SELECT DISTINCT 
                    ARPBL_RowId,
                    ARPBL_PAADM_DR,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_RowId,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_ID,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_No,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Name,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Name2,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_RowId,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_Code,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_Desc,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_CodeTranslated,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_DescTranslated,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_DOB,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_ForeignPhoneNo,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_PatCategory_DR->PCAT_RowId,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_PatCategory_DR->PCAT_Code,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_PatCategory_DR->PCAT_Desc,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Alias,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_IPNo, 
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_OPNo,
                    ARPBL_PAADM_DR->PAADM_ADMNO EpisodeNo,
	                ARPBL_AdmDate EpisodeDate,
	                ARPBL_PAADM_DR-> PAADM_AdmTime EpisodeTime,
                    ARPBL_PAADM_DR->PAADM_AdmDocCodeDR->CTPCP_Code,
                    ARPBL_PAADM_DR->PAADM_AdmDocCodeDR->CTPCP_Desc,
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
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_DerivedFeeItem,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_ItemCat_DR->ARCIC_Code,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_Desc,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_RowId,	                
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LineTotal,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_PatientShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_InsCompanyShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LocalGovtShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_SpecialistSurcharge,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_UnitPrice,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId, 
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Code, 
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Desc,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_DateFrom,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_DateTo,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_NationalCode,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RefStDateApptDate,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Owner,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_CodeTableTags,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Subregion_DR
	                
                FROM AR_PatientBill
                WHERE 
                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Code = '{REFT_Code}'
                AND ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Code like 'BWC%'
                AND ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId IS NOT NULL 
                AND ARPBL_ReasonCancel_DR IS NULL                               
                AND AR_PatBillPaymAlloc->PAYM_ARCIM_DR->ARCIM_RowId IS NOT NULL
             	AND isNull(AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LineTotal,0) > 0
             	and isNull(AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_DerivedFeeItem, 'Y') <> 'Y'
             	and isNull(AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_ItemCat_DR->ARCIC_Code,'') <> '1801WM'
             	Order By EpisodeDate DESC, EpisodeTime DESC
                ";
        }
    
        public static string GetARPatientsBillsByReferralTypeByIdList(IEnumerable<int> rowIdList)
        {
            string _rowIdList = "";

            foreach (var id in rowIdList)
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

            var query =  $@"
                    SELECT DISTINCT 
                    ARPBL_RowId,
                    ARPBL_PAADM_DR,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_RowId,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_ID,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_No,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Name,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Name2,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_RowId,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_Code,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_Desc,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_CodeTranslated,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Sex_DR->CTSEX_DescTranslated,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_DOB,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_ForeignPhoneNo,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_PatCategory_DR->PCAT_RowId,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_PatCategory_DR->PCAT_Code,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_PatCategory_DR->PCAT_Desc,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_Alias,
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_IPNo, 
                    ARPBL_PAADM_DR->PAADM_PAPMI_DR->PAPMI_OPNo,
                    ARPBL_PAADM_DR->PAADM_ADMNO EpisodeNo,
	                ARPBL_AdmDate EpisodeDate,
	                ARPBL_PAADM_DR-> PAADM_AdmTime EpisodeTime,
                    ARPBL_PAADM_DR->PAADM_AdmDocCodeDR->CTPCP_Code,
                    ARPBL_PAADM_DR->PAADM_AdmDocCodeDR->CTPCP_Desc,
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
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_DerivedFeeItem,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_ItemCat_DR->ARCIC_Code,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_Desc,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_RowId,	                
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LineTotal,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_PatientShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_InsCompanyShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LocalGovtShare,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_SpecialistSurcharge,
	                AR_PatBillGroup->AR_PatBillGroupCharges->ITM_UnitPrice,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId, 
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Code, 
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Desc,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_DateFrom,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_DateTo,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_NationalCode,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RefStDateApptDate,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Owner,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_CodeTableTags,
	                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Subregion_DR
	                
                FROM AR_PatientBill
                WHERE 
                ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId IN ({_rowIdList})
                AND ARPBL_PAADM_DR->PAADM_ReferralType->REFT_Code like 'BWC%'
                AND ARPBL_PAADM_DR->PAADM_ReferralType->REFT_RowId IS NOT NULL 
                AND ARPBL_ReasonCancel_DR IS NULL                               
                AND AR_PatBillPaymAlloc->PAYM_ARCIM_DR->ARCIM_RowId IS NOT NULL
             	AND isNull(AR_PatBillGroup->AR_PatBillGroupCharges->ITM_LineTotal,0) > 0
             	and isNull(AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_DerivedFeeItem, 'Y') <> 'Y'
             	and isNull(AR_PatBillGroup->AR_PatBillGroupCharges->ITM_ARCIM_DR->ARCIM_ItemCat_DR->ARCIC_Code,'') <> '1801WM'
             	Order By EpisodeDate DESC, EpisodeTime DESC
            ";
            return query;
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

        public static string GetQBWCMEMBERSByPapmiRowIdList(IEnumerable<string> rowIdList)
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
