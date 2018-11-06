using AgentReferralSystem.Api.Data.Config;
using AgentReferralSystem.Api.Data.Calculate;
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
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly ICacheDataAccess _cacheDataAccess;
        private readonly ISqlServerDataAccess _sqlServerDataAccess;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly LogFilePath _logPath;
        private readonly LogFileName _logFileName;
        public ScheduleService(ICacheDataAccess cacheDataAccess,
            ISqlServerDataAccess sqlServerDataAccess,
            IHostingEnvironment hostingEnvironment,
            LogFilePath logPath,
            LogFileName logFileName)
        {
            _logPath = logPath;
            _logFileName = logFileName;
            _cacheDataAccess = cacheDataAccess;
            _sqlServerDataAccess = sqlServerDataAccess;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task ScheduleMonthlyReward() 
        {
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(_logPath.Prod + DateTime.Now.ToString() + "MonthlyRewardLog.txt"))
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(_logPath.Test + DateTime.Now.ToString() + "MonthlyRewardLog.txt"))
            {
                try  
                {
                    int processMonth = 0;
                    int processYear = 0;
                    //Get Current Month Schedule
                    ScheduleMonthLog DBLog = await _sqlServerDataAccess.GetScheduleMonthLog();
                    if (DBLog == null) { processMonth = DateTime.Now.Month; processYear = DateTime.Now.Year; }
                    else { processMonth = DateTime.Now.Month + 1; processYear = DateTime.Now.Year; }


                    //Get AgentList from DB
                    List<Agent> AgentList = (List<Agent>)await _sqlServerDataAccess.GetAgentList();
                    if (AgentList.Count == 0) throw new Exception("No Agents please Contact IT");
                    //Setup every Agent
                    List<AgentOverviewModel> AgentModelList = new List<AgentOverviewModel>();
                    foreach (Agent agent in AgentList)
                    {
                        AgentOverviewModel item = new AgentOverviewModel(agent, processMonth, processYear, _sqlServerDataAccess);
                        AgentModelList.Add(item);
                    }

                    //Get RewardConfig
                    PercentConfig PC = (await _sqlServerDataAccess.GetConfigByCriteria("RewardConfig")).First() ;
                    if (PC != null)
                    {
                        try
                        {
                            AgentServiceProcessor.ProcessMonthlyReward(AgentModelList, PC);
                        }
                        catch (Exception ex)
                        {
                            file.WriteLine("ProcessMonthlyReward Error");
                            file.WriteLine("Message : " + ex.Message);
                            throw ex;
                        }
                    }
                    else
                    {
                        file.WriteLine("No Config, Please contact IT.");
                        throw new Exception("No Config, Please contact IT.");
                    }

                    //Insert ScheduleMonthLog
                    try
                    {
                        await _sqlServerDataAccess.InsertScheduleMonthLog(DBLog);
                        file.WriteLine("Update ScheduleLog Success");
                    }
                    catch(Exception ex)
                    {
                        file.WriteLine("Update ScheduleLog Fail");
                        file.WriteLine("Message : " + ex.Message);
                        throw ex;
                    }
                    file.WriteLine("ScheduleMonthlyReward Success");

                }
                catch (Exception ex)
                {
                    file.WriteLine("ScheduleMonthlyReward Error");
                    file.WriteLine("Message : " + ex.Message);
                    throw ex;
                }
                finally
                {
                    file.Flush();
                    file.Dispose();
                    file.Close();
                }
            }
        }

        #region NewFunction
        public async Task RecalDiaryCommission()
        {
            List<Agent> AgentList = (List<Agent>)await _sqlServerDataAccess.GetAgentList();
            var configList = (await _sqlServerDataAccess.GetConfigByCriteria("GlobalCommissionRate")).ToList();
        }

        //Load BillItemTrakcare and Insert CommissionItem
        public async Task MigrateCommissionItem()
        {
            try
            {
                //Get AgentList from DB
                List<Agent> AgentList = (List<Agent>)await _sqlServerDataAccess.GetAgentList();
                //Get ConfigList
                var configList = (await _sqlServerDataAccess.GetConfigByCriteria("GlobalCommissionRate")).ToList();

                //Loop Agent            
                foreach (Agent agent in AgentList)
                {
                    //Get BillList for agent
                    var patientsBills = (await _cacheDataAccess.GetARPatientsBillsByReferralTypeRowIdAsync(agent.AgentId)).ToList();
                    var dbBills = (await _sqlServerDataAccess.GetCommissionItemById(agent.AgentId)).ToList();
                    var insertBills = patientsBills.Where(x => !dbBills.Any(y => y.ARPBL_RowId == x.ARPBL_RowId)).ToList();
                    ProcessMigrateCommission(agent, insertBills);

                    await _sqlServerDataAccess.AddOrUpdateAgentAsync(agent);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void calculateMonthlyCommission()
        {

        }

        //Calculate DiaryCommission(DisplayCommission)
        private void calculateDiaryCommission()
        {

        }

        private async void calculateDiaryCommissionByItemList(Agent agent, List<CommissionItem> billList)
        {
            var _agent = agent;
            List<PercentConfig> configList = (await _sqlServerDataAccess.GetConfigByCriteria("GlobalCommissionRate")).ToList();
            PercentConfig currentConfig = configList.Where(x => decimal.Parse((x.value2 ?? "0")) >= agent.DisplayCommission).FirstOrDefault();
            if (currentConfig == null) currentConfig = configList.Last();
            decimal totalIncome = billList.Sum(x => x.Item_Total);
            _agent.CurrentSale = decimal.Add(_agent.CurrentSale,totalIncome);
            PercentConfig newConfig = configList.Where(x => decimal.Parse((x.value2 ?? "0")) >= agent.CurrentSale).FirstOrDefault();
            if (newConfig == null) newConfig = configList.Last();
            if (currentConfig.id != newConfig.id)
            {
                //var 
                foreach(CommissionItem bill in billList)
                {
                    bill.Item_Commission = decimal.Divide(decimal.Multiply(bill.Item_Total, newConfig.value1), 100);
                    await _sqlServerDataAccess.SaveCommissionItem(bill);
                }
                await _sqlServerDataAccess.AddOrUpdateAgentAsync(_agent);
            }
            await _sqlServerDataAccess.AddOrUpdateAgentAsync(_agent);
        }

        //Calculate Commission
        public void calculateAgentCommission()
        {

        }

        //Calculate Reward
        public void calculateAgentReward()
        {

        }

        //Migrate Commission
        private void ProcessMigrateCommission(Agent agent, List<ARPatientBill> BillList)
        {
            try
            {
                string message = "";
                List<CommissionItem> itemList = new List<CommissionItem>();
                foreach (ARPatientBill bill in BillList)
                {
                    CommissionItem item = new CommissionItem();
                    item.ARPBL_RowId = bill.ARPBL_RowId;
                    item.Agent_Id = agent.AgentId;
                    item.Agent_Code = agent.AgentCode;
                    item.Agent_Name = agent.AgentDesc;
                    item.PatientId = bill.PAPMI_ID;
                    item.HN_Number = bill.PAPMI_No;
                    item.Patient_Name = bill.PAPMI_Name + " " + bill.PAPMI_Name2;
                    item.Episode_Number = bill.EpisodeNo;
                    item.Episode_Date = bill.EpisodeDate;
                    item.Doctor_Code = bill.CTPCP_Code;
                    item.Doctor_Name = bill.CTPCP_Desc;
                    item.Discharge_Date = bill.DischargeDate;
                    item.Item_Id = bill.ARCIM_RowId;
                    item.Item_Code = bill.ARCIM_Code;
                    item.Item_Desc = bill.ARCIM_Desc;
                    item.Item_Total = ServicesCalc.GetItemValue(bill);
                    item.isDelete = false;
                    item.CreateDate = DateTime.Now;
                    itemList.Add(item);
                    try
                    {
                        _sqlServerDataAccess.SaveCommissionItem(item);
                    }
                    catch (Exception ex)
                    {
                        message += ex.Message;
                    }
                }
                calculateDiaryCommissionByItemList(agent, itemList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Local Process Function in Service

        #endregion

        #region Testing

        public async Task<List<Dictionary<string,object>>> TestLog()
        {
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
            Dictionary<string, object> TopData = new Dictionary<string, object>();

            Dictionary<string, object> FunctionData = new Dictionary<string, object>();
            string LogString = "";
            string FirstLog = "Loging Test";
            int data = 1;
            LogString = LogString + FirstLog;
            FunctionData.Add("data", data);
            FunctionData.Add("log", FirstLog);
            resultList.Add(FunctionData);

            LogViewModel logViewModel = new LogViewModel(_logPath, _logFileName, LogString);
            await logViewModel.writeLog("test", "test");
            return resultList;
        }

        #endregion
    }
}
