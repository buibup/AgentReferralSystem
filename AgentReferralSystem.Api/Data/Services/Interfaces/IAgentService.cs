using AgentReferralSystem.Api.Data.Models;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Services.Interfaces
{
    public interface IAgentService
    {
        Task<Dictionary<string, object>> GetAgentProfileByAgentId(int agentId);
        Task<Dictionary<string, object>> GetAgentProfileByAgentCode(string agentCode);
        Task<Dictionary<string, object>> GetAgentCustomerByAgentId(int agentId);
        Task<Dictionary<string, object>> GetAgentCustomerByAgentCode(string agentCode);
        Task<Dictionary<string, object>> GetAgentCommissionByAgentId(int agentId);
        Task<Dictionary<string, object>> GetAgentCommissionByAgentCode(string agentCode);
        Task<Dictionary<string, object>> GetAgentTargetByAgentId(int agentId);
        Task<Dictionary<string, object>> GetAgentTargetByAgentCode(string agentCode);
        Task<IEnumerable<Agent>> GetAgentList();
        Task<AgentViewModel> GetAgentViewModelByAgentIdAsync(int agentId);
        Task<List<AgentReportViewModel>> LoadAgentSummarizeByIdAsync(int agentId, int Year, int Month);
        Task<PACReferralType> GetPACReferralTypesByIdAsync(int agentId);
        Task<IEnumerable<PACReferralType>> GetPACReferralTypesAsync(string search);
        Task<Agent> GetAgentByIdAsync(int agentId);
        Task<Agent> GetAgentByAgentCodeAsync(string agentCode);
        Task<IEnumerable<SaleType>> GetSaleTypes();
        Task AddOrUpdateAgentAsync(Agent agent);
        Task DeleteAgentAsync(int agentId);
        Task SaveExportAgentAsync(AgentViewModel model);
        void ExportAgent(AgentViewModel model);

        Task<IEnumerable<CommissionItem>> getItemlist();

        Task UploadAgentImage(int agentId, string imageData);

        Task<Dictionary<string, object>> GenerateAgentExcel();
    }
}
