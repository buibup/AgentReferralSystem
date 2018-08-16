using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using AgentReferralSystem.Api.Data.Models;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Models.SqlServer;
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

        public Task AddAgentAsync(Agent agent)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAgentAsync(int agentId)
        {
            throw new NotImplementedException();
        }

        public Task<AgentOutput> GetAgentByIdAsync(int agentId)
        {
            throw new NotImplementedException();
        }

        public async Task<AgentViewModel> GetAgentViewModelByAgentIdAsync(int agentId)
        {
            var result = new AgentViewModel();

            // get agent moq
            //var agent = AgentMoq.GetAgentByAgentId(agentId);

            var agent = await _sqlServerDataAccess.GetAgentByIdAsync(agentId);

            // get all patientbill of agent
            var patientsBills = await _cacheDataAccess.GetARPatientsBillsByReferralTypeRowIdAsync(agentId);

            // distinct patients by papmiRowId
            var papmiRowIdList = patientsBills.Select(p => p.PAADM_PAPMI_DR).Distinct();

            // get all patient register membership
            var memberRegisList = await _cacheDataAccess.GetQBWCMEMBERSByPapmiRowIdListAsync(papmiRowIdList);

            var itemCompoundingList = await _cacheDataAccess.GetARCItmMastCompoundingAsync();

            result = agent.AgentViewModelProcess(patientsBills, memberRegisList, itemCompoundingList);

            return result;
        }

        public async Task<IEnumerable<PACReferralType>> GetPACReferralTypesAsync()
        {
            var result = await _cacheDataAccess.GetPACReferralTypeAllAsync();

            return result;
        }
    }
}
