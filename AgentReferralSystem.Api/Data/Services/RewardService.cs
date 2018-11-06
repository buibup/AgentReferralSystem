using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using AgentReferralSystem.Api.Data.Models;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Moq;
using AgentReferralSystem.Api.Data.Services.Interfaces;
using AgentReferralSystem.Api.Data.ViewModels;
using EPPlus.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Services
{
    public class RewardService : IRewardService
    {
        private readonly ICacheDataAccess _cacheDataAccess;
        private readonly ISqlServerDataAccess _sqlServerDataAccess;
        private readonly IHostingEnvironment _hostingEnvironment;
        public RewardService(ICacheDataAccess cacheDataAccess,
            ISqlServerDataAccess sqlServerDataAccess,
            IHostingEnvironment hostingEnvironment)
        {
            _cacheDataAccess = cacheDataAccess;
            _sqlServerDataAccess = sqlServerDataAccess;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IEnumerable<RewardItem>> GetRewardList()
        {
            var result = await _sqlServerDataAccess.GetRewardList();

            return result;
        }

        public async Task<IEnumerable<RewardItemHistory>> GetRewardHistoryList()
        {
            var result = await _sqlServerDataAccess.GetRewardHistoryList();

            return result;
        }

        public async Task<IEnumerable<RewardExchange>> GetRewardExchangeList(int? agentId)
        {
            var result = await _sqlServerDataAccess.GetRewardExchangeList(agentId);

            return result;
        }
    }
}
