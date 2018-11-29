using AgentReferralSystem.Api.Data.Models.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.DataAccess.Interfaces
{
    public interface ICacheDataAccess
    {
        Task<PACReferralType> GetPACReferralTypeByIdAsync(int agentId);
        Task<IEnumerable<PACReferralType>> GetPACReferralTypeAllAsync(string search);
        Task<IEnumerable<ARPatientBill>> GetARPatientsBillsByREFT_Code(string REFT_Code);
        Task<IEnumerable<ARPatientBill>> GetARPatientsBillsByReferralTypeRowIdAsync(int rowId);
        Task<IEnumerable<ARCItmMast>> GetARCItmMastCompoundingAsync();
        Task<QBWCMEMBERS> GetQBWCMEMBERSByPapmiRowIdAsync(int papmiRowId);
        Task<IEnumerable<QBWCMEMBERS>> GetQBWCMEMBERSByPapmiRowIdListAsync(IEnumerable<string> papmiRowIdList);
    }
}
