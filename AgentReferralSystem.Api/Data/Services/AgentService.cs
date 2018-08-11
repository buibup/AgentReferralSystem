using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using AgentReferralSystem.Api.Data.Moq;
using AgentReferralSystem.Api.Data.Services.Interfaces;
using AgentReferralSystem.Api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Services
{
    public class AgentService : IAgentService
    {
        private readonly ICacheDataAccess _cacheDataAccess;
        private readonly ISqlServerDataAccess _sqlServerDataAccess;
        public AgentService(ICacheDataAccess cacheDataAccess,
            ISqlServerDataAccess sqlServerDataAccess)
        {
            _cacheDataAccess = cacheDataAccess;
            _sqlServerDataAccess = sqlServerDataAccess;
        }
        public async Task<AgentViewModel> GetAgentViewModelByAgentIdAsync(int agentId)
        {
            var result = new AgentViewModel();

            // get agent
            var agent = AgentMoq.GetAgentByAgentId(agentId);

            // get all patientbill of agent
            var patientsBills = await _cacheDataAccess.GetARPatientsBillsByReferralTypeRowIdAsync(agentId);

            // distinct patients by papmiRowId
            var papmiRowIdList = patientsBills.Select(p => p.PAADM_PAPMI_DR).Distinct().ToList();

            // get all patient register membership
            var memberRegisList = await _cacheDataAccess.GetQBWCMEMBERSByPapmiRowIdListAsync(papmiRowIdList);

            

            return result;
        }
    }
}
