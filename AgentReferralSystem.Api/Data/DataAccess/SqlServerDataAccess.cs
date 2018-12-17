using AgentReferralSystem.Api.Data.Config;
using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using AgentReferralSystem.Api.Data.Models;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Query;
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
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                try
                {
                    var agentParam = new DynamicParameters();
                    agentParam.Add("@AgentId", agent.AgentId);
                    agentParam.Add("@AgentCode", agent.AgentCode);
                    agentParam.Add("@AgentDesc", agent.AgentDesc);
                    agentParam.Add("@Address", agent.Address);
                    agentParam.Add("@Email", agent.Email);
                    agentParam.Add("@BankAccount", agent.BankAccount);
                    agentParam.Add("@DisplayImage", agent.DisplayImage);
                    agentParam.Add("@AgreementDate", agent.AgreementDate);
                    agentParam.Add("@DateFrom", agent.DateFrom);
                    agentParam.Add("@DateTo", agent.DateTo);
                    agentParam.Add("@Remark", agent.Remark ?? "");
                    agentParam.Add("@DisplayCommission", agent.DisplayCommission);
                    agentParam.Add("@CurrentSale", agent.CurrentSale);
                    agentParam.Add("@CurrentReward", agent.CurrentReward);
                    agentParam.Add("@TotalReward", agent.TotalReward);
                    agentParam.Add("@CreateDate", agent.CreateDate);
                    agentParam.Add("@LastModifyDate", DateTime.Now);
                    agentParam.Add("@isDelete", agent.isDelete);

                    await conn.ExecuteAsync("SaveAgent", agentParam, transaction: tran , commandType: CommandType.StoredProcedure);

                    #region oldConfig (SaleTypes)
                    //var agentSaleTypeParam = new DynamicParameters();
                    //foreach (var item in agent.AgentSaleTypes)
                    //{
                    //    agentSaleTypeParam.Add("@BaseCommission",item.BaseCommission);
                    //    agentSaleTypeParam.Add("@Target", item.Target);
                    //    agentSaleTypeParam.Add("@TargetPeriod", item.TargetPeriod);
                    //    agentSaleTypeParam.Add("@IncreaseIfTargetMet", item.IncreaseIfTargetMet);
                    //    agentSaleTypeParam.Add("@Maximum", item.Maximum);
                    //    agentSaleTypeParam.Add("@ResetToBase", item.ResetToBase);
                    //    agentSaleTypeParam.Add("@ApplicableTargetInrease", item.ApplicableTargetInrease);
                    //    agentSaleTypeParam.Add("@SaleTypeId", item.SaleTypeId);
                    //    agentSaleTypeParam.Add("@AgentId", item.AgentId);

                    //    await conn.ExecuteAsync("SaveAgentsSaleTypes", agentSaleTypeParam,transaction: tran, commandType: CommandType.StoredProcedure);
                    //}
                    #endregion
                    //throw new Exception("Test");
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                    conn.Dispose();
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

        public async Task<IEnumerable<Agent>> GetAgentList(string search = "")
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();

                try
                {
                    var query = "select * from dbo.Agent where isNull(isDelete,0) = 0";
                    if (!string.IsNullOrWhiteSpace(search)) query += " and AgentDesc like '% " + search + "%'";
                    var data = (await conn.QueryAsync<Agent>(query));
                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public async Task<Agent> GetAgentByIdAsync(int agentId)
        {
            var agent = new Agent();

            using(var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                var p = new DynamicParameters();
                p.Add("@AgentId", agentId);

                agent = (await conn.QueryAsync<Agent>("GetAgentById", p, commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();

                if(agent != null)
                {
                    //agent.AgentSaleTypes = (await conn.QueryAsync<SaleTypesOutput>("GetSaleTypeByAgentId", p, commandType: CommandType.StoredProcedure)).ToList();
                }
                
            }

            return agent;
        }

        public async Task<Agent> GetAgentByAgentCodeAsync(string agentCode)
        {
            try
            {
                var agent = new Agent();

                using (var conn = new SqlConnection(_connectionStrings.SqlServer))
                {
                    var p = new DynamicParameters();
                    p.Add("@AgentCode", agentCode);

                    agent = (await conn.QueryAsync<Agent>("GetAgentByAgentCode", p, commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();
                }
                return agent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SaleType>> GetSaleTypes()
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                try
                {
                    var query = "select * from dbo.SaleType";
                    var data = (await conn.QueryAsync<SaleType>(query));
                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public async Task<IEnumerable<CommissionItem>> GetItemList()
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                try
                {

                    var data = (await conn.QueryAsync<CommissionItem>
                        ("select * from dbo.CommissionItem where isNull(isDelete , 0) = 0", commandType: CommandType.Text));
                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }

            }
        }

        public async Task<IEnumerable<string>> GetCustomerByAgentId(int AgentId)
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@AgentId", AgentId);

                    var data = (await conn.QueryAsync<string>
                        ("GetCustomerInCommissionItemByAgentId", p, commandType: CommandType.StoredProcedure));
                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }

            }
        }

        public async Task<IEnumerable<CommissionItem>> GetCommissionItemById(int? agentId = null, string PatientId = null)
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@Agent_Id", agentId);
                    p.Add("@PatientId", PatientId);

                    var data = (await conn.QueryAsync<CommissionItem>
                        ("GetCommissionItemById", p, commandType: CommandType.StoredProcedure));
                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }

            }
        }

        public async Task SaveCommissionItem(CommissionItem item)
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                try
                {
                    var p = new DynamicParameters();
                    p.Add(@"Id", item.Id);
                    p.Add("@ARPBL_RowId", item.ARPBL_RowId);
                    p.Add("@Agent_Id", item.Agent_Id);
                    p.Add("@Agent_Code", item.Agent_Code);
                    p.Add("@Agent_Name", item.Agent_Name);
                    p.Add("@PatientId", item.PatientId);
                    p.Add("@HN_Number", item.HN_Number);
                    p.Add("@Patient_Name", item.Patient_Name);
                    p.Add("@Patient_Desc", item.Patient_Desc);
                    p.Add("@Episode_Number", item.Episode_Number);
                    p.Add("@Episode_Date", item.Episode_Date);
                    p.Add("@Episode_Time", item.Episode_Time);
                    p.Add("@Doctor_Code", item.Doctor_Code);
                    p.Add("@Doctor_Name", item.Doctor_Name);
                    p.Add("@Discharge_Date", item.Discharge_Date);
                    p.Add("@BillPrinted_Date", item.BillPrinted_Date);
                    p.Add("@PatientBill_Number", item.PatientBill_Number);
                    p.Add("@Bill_Type", item.Bill_Type);
                    p.Add("@check_PaidInvoice", item.check_PaidInvoice);
                    p.Add("@Item_Id", item.Item_Id);
                    p.Add("@Item_Code", item.Item_Code);
                    p.Add("@Item_Desc", item.Item_Desc);
                    p.Add("@Item_Total", item.Item_Total);
                    p.Add("@Item_Commission", item.Item_Commission);
                    p.Add("@isDelete", item.isDelete);
                    p.Add("@CreateDate", item.CreateDate);

                    await conn.ExecuteAsync("SaveCommissionItem", p, tran, commandType: CommandType.StoredProcedure);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                    conn.Dispose();
                }

            }
        }

        public async Task SaveUser(User user)
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                try
                {
                    string sql = @"INSERT INTO AgentReferralSystem.dbo.[User]
                                  (username, password, Email, Title_TH, FirstName, MiddleName, LastName, FullName, CreateDate, isDelete)
                                  VALUES(@username, @password, @Email, @Title_TH, @FirstName, @MiddleName, @LastName, @FullName, @CreateDate, 0)";
                    var p = new DynamicParameters();
                    p.Add("@username", user.username);
                    p.Add("@password", user.password);
                    p.Add("@Email", user.Email);
                    p.Add("@Title_TH", user.Title_TH);
                    p.Add("@FirstName", user.FirstName);
                    p.Add("@MiddleName", user.MiddleName);
                    p.Add("@LastName", user.LastName);
                    p.Add("@FullName", user.FullName);
                    p.Add("@CreateDate", user.CreateDate);
                    await conn.QueryAsync<CommissionItem>(sql, p, tran, commandType: CommandType.Text);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public async Task<IEnumerable<RewardItem>> GetRewardList()
        {
            var init = new List<RewardItem>();
            var data = init.AsEnumerable<RewardItem>();
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                try
                {
                    string sql = @"select * from dbo.RewardItem where isNull(isDelete,0) = 0";
                    data = await conn.QueryAsync<RewardItem>(sql,commandType: CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return data;
        }

        public async Task<IEnumerable<RewardItemHistory>> GetRewardHistoryList()
        {
            var init = new List<RewardItemHistory>();
            var data = init.AsEnumerable<RewardItemHistory>();
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                try
                {
                    string sql = @"select * from dbo.RewardHistory where isNull(isDelete,0) = 0";
                    data = await conn.QueryAsync<RewardItemHistory>(sql, commandType: CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return data;
        }

        public async Task<IEnumerable<RewardExchange>> GetRewardExchangeList(int? AgentId)
        {
            var init = new List<RewardExchange>();
            var data = init.AsEnumerable<RewardExchange>();
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                try
                {
                    string sql = "";
                    if (AgentId == null) sql = @"select * from dbo.RewardExchange where isNull(isDelete,0) = 0";
                    else sql = string.Format(@"select * from dbo.RewardExchange where isNull(isDelete,0) = 0 and AgentId = '{0}'", AgentId);
                    data = await conn.QueryAsync<RewardExchange>(sql, commandType: CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Dispose();
                    conn.Close();
                }
            }
            return data;
        }

        public async Task<ScheduleMonthLog> GetScheduleMonthLog()
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                var data = new ScheduleMonthLog();
                try
                {
                    data = (await conn.QueryAsync<ScheduleMonthLog>("select * From dbo.ScheduleMonthLog order by CreateDate DESC", commandType: CommandType.Text)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Dispose();
                    conn.Close();
                }
                return data;
            }
            
        }

        public async Task InsertScheduleMonthLog(ScheduleMonthLog Log)
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@ProcessLog", Log.ProcessLog);
                    p.Add("@Remark", Log.Remark);
                    p.Add("@ScheduleMonthReward", Log.ScheduleMonthReward);
                    p.Add("@ScheduleMonthCommission", Log.ScheduleMonthCommission);
                    p.Add("@CreateDate", DateTime.Now);
                    await conn.ExecuteAsync("InsertScheduleMonthLog", p, tran, commandType: CommandType.StoredProcedure);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        #region OverrideMethod GetConfigByCriteria
        public async Task<IEnumerable<PercentConfig>> GetConfigByCriteria(string ConfigType, string ConfigName)
        {
            List<string> Criteria = new List<string>();
            Criteria.Add(ConfigType);
            Criteria.Add(ConfigName);
            return await GetConfigByCriteria(Criteria, 1);
        }
        public async Task<IEnumerable<PercentConfig>> GetConfigByCriteria(string ConfigType)
        { 
            List<string> Criteria = new List<string>();
            Criteria.Add(ConfigType);
            return await GetConfigByCriteria(Criteria, 2);
        }
        #endregion
        private async Task<IEnumerable<PercentConfig>> GetConfigByCriteria(List<string> Criteria, int CriteriaType)
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                try
                {
                    //Type 1 (No Config/If else yet)
                    string ConfigType = Criteria[0];
                    string ConfigName = "";
                    if (Criteria.Count > 1) ConfigName = Criteria[1] ?? null;
                    string sql = SqlServerQuery.GetPercentConfigSQLString(CriteriaType, Criteria);
                    return await conn.QueryAsync<PercentConfig>(sql, commandType: CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Dispose();
                    conn.Close();
                }
            }
        }
    }
}
