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
                    agentParam.Add("@AgreementDate", agent.AgreementDate);
                    agentParam.Add("@DateFrom", agent.StartDate);
                    agentParam.Add("@DateTo", agent.EndDate);
                    agentParam.Add("@Remark", agent.Remark ?? "");
                    agentParam.Add("@CurrentReward", agent.CurrentReward);
                    agentParam.Add("@TotalReward", agent.TotalReward);
                    agentParam.Add("@CreateDate", agent.CreateDate);

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

        public async Task<IEnumerable<Agent>> GetAgentList()
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                try
                {
                    var query = "select * from dbo.Agent";
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
                    //agent.AgentSaleTypes = (await conn.QueryAsync<SaleTypesOutput>("GetSaleTypeByAgentId", p, commandType: CommandType.StoredProcedure)).ToList();
                }
                
            }

            return agent;
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

        public async Task<IEnumerable<CommissionItem>> GetCommissionItemById(int agentId)
        {
            using (var conn = new SqlConnection(_connectionStrings.SqlServer))
            {
                conn.Open();
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@Agent_Id", agentId);

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
                    p.Add("@ARPBL_RowId", item.ARPBL_RowId);
                    p.Add("@Agent_Id", item.Agent_Id);
                    p.Add("@Agent_Code", item.Agent_Code);
                    p.Add("@Agent_Name", item.Agent_Name);
                    p.Add("@HN_Number", item.HN_Number);
                    p.Add("@Patient_Name", item.Patient_Name);
                    p.Add("@Patient_Desc", item.Patient_Desc);
                    p.Add("@Episode_Number", item.Episode_Number);
                    p.Add("@Episode_Date", item.Episode_Date);
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
                    string sql = @"select * from dbo.RewardItem";
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
    }
}
