using AgentReferralSystem.Api.Data.Models.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.DataAccess.Interfaces
{
    public interface ICacheDataAccess
    {
        Task<IEnumerable<PACReferralType>> GetPACReferralTypeAllAsync();
        Task<IEnumerable<ARPatientBill>> GetARPatientsBillsByReferralTypeRowIdAsync(int rowId);
        Task<IEnumerable<ARCItmMast>> GetARCItmMastCompoundingAsync();
        Task<QBWCMEMBERS> GetQBWCMEMBERSByPapmiRowIdAsync(int papmiRowId);
        Task<IEnumerable<QBWCMEMBERS>> GetQBWCMEMBERSByPapmiRowIdListAsync(IEnumerable<int> papmiRowIdList);
    }
}
