using AgentReferralSystem.Api.Data.Config;
using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using AgentReferralSystem.Api.Data.Models;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.DataAccess
{
    public class SqlServerDataAccess : ISqlServerDataAccess
    {
        private readonly ConnectionStrings _connectionStrings;
        public SqlServerDataAccess(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task AddOrUpdateAgentAsync(Agent agent)
        {
            using(var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                var agentParam = new DynamicParameters();
                agentParam.Add("@AgentId", agent.AgentId);
                agentParam.Add("@AgentCode", agent.AgentCode);
                agentParam.Add("@AgentDesc", agent.AgentDesc);
                agentParam.Add("@AgreementDate", agent.AgreementDate);
                agentParam.Add("@DateFrom", agent.StartDate);
                agentParam.Add("@DateTo", agent.EndDate);
                agentParam.Add("@Remark", agent.Remark);

                await conn.ExecuteAsync("SaveAgent", agentParam, commandType: CommandType.StoredProcedure);

                var agentSaleTypeParam = new DynamicParameters();
                foreach(var item in agent.AgentSaleTypes)
                {
                    agentSaleTypeParam.Add("@BaseCommission", item.BaseCommission);
                    agentSaleTypeParam.Add("@Target", item.Target);
                    agentSaleTypeParam.Add("@TargetPeriod", item.TargetPeriod);
                    agentSaleTypeParam.Add("@IncreaseIfTargetMet", item.IncreaseIfTargetMet);
                    agentSaleTypeParam.Add("@Maximum", item.Maximum);
                    agentSaleTypeParam.Add("@ResetToBase", item.ResetToBase);
                    agentSaleTypeParam.Add("@ApplicableTargetInrease", item.ApplicableTargetInrease);
                    agentSaleTypeParam.Add("@SaleTypeId", item.SaleTypeId);
                    agentSaleTypeParam.Add("@AgentId", item.AgentId);

                    await conn.ExecuteAsync("SaveAgentsSaleTypes", agentSaleTypeParam, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public async Task DeleteAgentAsync(int agentId)
        {
            using(var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                var p = new DynamicParameters();
                p.Add("@AgentId", agentId);

                await conn.ExecuteAsync("DeleteAgent", p, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<AgentOutput> GetAgentByIdAsync(int agentId)
        {
            var agent = new AgentOutput();

            using(var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                var p = new DynamicParameters();
                p.Add("@AgentId", agentId);

                agent = (await conn.QueryAsync<AgentOutput>("GetAgentById", p, commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();

                if(agent != null)
                {
                    agent.AgentSaleTypes = (await conn.QueryAsync<SaleTypesOutput>("GetSaleTypeByAgentId", p, commandType: CommandType.StoredProcedure)).ToList();
                }
                
            }

            return agent;
        }
    }
}
