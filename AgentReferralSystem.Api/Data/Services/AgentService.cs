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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Services
{
    public class AgentService : IAgentService
    { 
        private readonly ICacheDataAccess _cacheDataAccess;
        private readonly ISqlServerDataAccess _sqlServerDataAccess;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly LogFilePath _logPath;
        private readonly LogFileName _logFileName;
        private readonly ExcelFilePath _excelPath;
        private readonly ExcelFileName _excelFileName;
        private readonly ExcelHeader _excelHeader;
        public AgentService(ICacheDataAccess cacheDataAccess,
            ISqlServerDataAccess sqlServerDataAccess,
            IHostingEnvironment hostingEnvironment,
            LogFilePath logPath,
            LogFileName logFileName,
            ExcelFilePath excelPath,
            ExcelFileName excelFileName,
            ExcelHeader excelHeader)
        {
            _logPath = logPath;
            _logFileName = logFileName;
            _excelPath = excelPath;
            _excelFileName = excelFileName;
            _excelHeader = excelHeader;
            _cacheDataAccess = cacheDataAccess;
            _sqlServerDataAccess = sqlServerDataAccess;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task AddOrUpdateAgentAsync(Agent agent)
        {
            try
            {
                await _sqlServerDataAccess.AddOrUpdateAgentAsync(agent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteAgentAsync(int agentId)
        {
            await _sqlServerDataAccess.DeleteAgentAsync(agentId);
        }
        
        public async Task<IEnumerable<Agent>> GetAgentList()
        {
            var result = await _sqlServerDataAccess.GetAgentList();

            return result;
        }

        public async Task<Dictionary<string,object>> GetAgentProfile(int agentId)
        {
            try
            {
                var result = new Dictionary<string, object>();
                var model = await _sqlServerDataAccess.GetAgentByIdAsync(agentId);
                var config = await _sqlServerDataAccess.GetConfigByCriteria("GlobalCommissionRate");

                if (model != null)
                {
                    result.Add("status", "OK");
                    result.Add("Message", "");
                    result.Add("data", new AgentProfileViewModel(model,config.ToList()));
                }
                else
                {
                    result.Add("status", "Error");
                    result.Add("Message", "Can't Load Agent with AgentId " + agentId);
                }
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dictionary<string, object>> GetAgentCustomer(int agentId)
        {
            try
            {
                var result = new Dictionary<string, object>();
                var customerList = await _sqlServerDataAccess.GetCustomerByAgentId(agentId);
                if (customerList != null && customerList.Count() > 0)
                {
                    //foreach(int customerId in customerList)
                    //{
                    //    var ItemList = (await _sqlServerDataAccess.GetCommissionItemById(agentId)).ToList();
                    //    dataList = AgentCustomerViewModel.GenerateACVMList(ItemList);

                    //    var ItemList = (await _sqlServerDataAccess.GetCommissionItemById(agentId, customerId)).ToList();
                    //    AgentCustomerViewModel item = new AgentCustomerViewModel(ItemList);
                    //    dataList.Add(item);
                    //}

                    var ItemList = (await _sqlServerDataAccess.GetCommissionItemById(agentId)).ToList();
                    var data = AgentCustomerOverall.GenerateCustomerOverall(ItemList);
                    result.Add("status", "OK");
                    result.Add("Message", "");
                    result.Add("data", data);
                }
                else
                {
                    result.Add("status", "Error");
                    result.Add("Message", "No Customer with AgentId: " + agentId);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dictionary<string, object>> GetAgentCommission(int agentId)
        {
            try
            {
                var result = new Dictionary<string, object>();
                var itemList = (await _sqlServerDataAccess.GetCommissionItemById(agentId)).ToList();
                var configList = (await _sqlServerDataAccess.GetConfigByCriteria("GlobalCommissionRate")).ToList();
                if (itemList != null && itemList.Count() > 0)
                {

                    var data = await AgentCommissionViewModel.GenerateAgentComVMList(itemList, configList);
                    result.Add("status", "OK");
                    result.Add("Message", "");
                    result.Add("data", data);
                }
                else
                {
                    result.Add("status", "Error");
                    result.Add("Message", "No CommissionItem with AgentId: " + agentId);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dictionary<string,object>> GetAgentTarget(int agentId)
        {
            try
            {
                var result = new Dictionary<string, object>();
                var customerList = await _sqlServerDataAccess.GetCustomerByAgentId(agentId);
                var dataList = new List<AgentCustomerViewModel>();
                if (customerList != null && customerList.Count() > 0)
                {
                    foreach (string customerId in customerList)
                    {
                        var ItemList = (await _sqlServerDataAccess.GetCommissionItemById(agentId, customerId)).ToList();
                        AgentCustomerViewModel item = new AgentCustomerViewModel(ItemList);
                        dataList.Add(item);
                    }
                    result.Add("status", "OK");
                    result.Add("Message", "");
                    result.Add("data", dataList);
                }
                else
                {
                    result.Add("status", "Error");
                    result.Add("Message", "No Customer with AgentId: " + agentId);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Agent> GetAgentByIdAsync(int agentId)
        {
            var model = await _sqlServerDataAccess.GetAgentByIdAsync(agentId);
            return model;
        }

        public async Task<Agent> GetAgentByAgentCodeAsync(string agentCode)
        {
            try
            {
                var agent = await _sqlServerDataAccess.GetAgentByAgentCodeAsync(agentCode);
                return agent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UploadAgentImage(int agentId, string ImageData)
        {
            var agent = await _sqlServerDataAccess.GetAgentByIdAsync(agentId);

            AgentServiceProcessor.SaveImage(agent, ImageData);
        }

        //Old Calculation
        public async Task<AgentViewModel> GetAgentViewModelByAgentIdAsync(int agentId)
        {
            var result = new AgentViewModel();

            // get agent moq
            //var agent = AgentMoq.GetAgentByAgentId(agentId);

            var agent = await _sqlServerDataAccess.GetAgentByIdAsync(agentId);
            // get all patientbill of agent
            var patientsBills = (await _cacheDataAccess.GetARPatientsBillsByReferralTypeRowIdAsync(agentId)).ToList();

            if(patientsBills.Count > 0)
            {
                // distinct patients by papmiRowId
                var papmiRowIdList = patientsBills.Select(p => p.PAADM_PAPMI_DR).Distinct();
                 
                // get all patient register membership
                var memberRegisList = await _cacheDataAccess.GetQBWCMEMBERSByPapmiRowIdListAsync(papmiRowIdList);

                var itemCompoundingList = await _cacheDataAccess.GetARCItmMastCompoundingAsync();

                //throw new Exception("Test");
                //result = agent.AgentViewModelProcess(patientsBills, memberRegisList, itemCompoundingList);
            }

            return result;
        }

        //Manual Load & Calculation  
        public async Task<List<AgentReportViewModel>> LoadAgentSummarizeByIdAsync(int agentId, int Year, int Month)
        {
            var result = new List<AgentReportViewModel>();

            var agent = await _sqlServerDataAccess.GetAgentByIdAsync(agentId);
            // get all patientbill of agent
            var patientsBills = (await _cacheDataAccess.GetARPatientsBillsByReferralTypeRowIdAsync(agentId)).ToList();

            // get all CommissionItem of agent
            var BillList = (await _sqlServerDataAccess.GetCommissionItemById(agentId)).ToList();

            #region Moq
            // get agent moq
            //var agent = AgentMoq.GetAgentByAgentId(agentId);
            var ConfigList = PercentConfigMoq.getConfig();
            var MoqBill = ARPatientBillMoq.getPatientBillList();
            #endregion

            if (patientsBills.Count > 0)
            {
                // distinct patients by papmiRowId
                var papmiRowIdList = patientsBills.Select(p => p.PAADM_PAPMI_DR).Distinct();

                // get all patient register membership
                //var memberRegisList = await _cacheDataAccess.GetQBWCMEMBERSByPapmiRowIdListAsync(papmiRowIdList);

                //var itemCompoundingList = await _cacheDataAccess.GetARCItmMastCompoundingAsync();

                //throw new Exception("Test");
               // result = agent.ProcessCommission(patientsBills, BillList, ConfigList, Year, Month);
            }

            return result;
        }

        public async Task<PACReferralType> GetPACReferralTypesByIdAsync(int agentId)
        {
            var result = await _cacheDataAccess.GetPACReferralTypeByIdAsync(agentId);

            return result;
        }

        public async Task<IEnumerable<PACReferralType>> GetPACReferralTypesAsync(string search)
        {
            var result = await _cacheDataAccess.GetPACReferralTypeAllAsync(search);

            return result;
        }

        public async Task<IEnumerable<SaleType>> GetSaleTypes()
        {
            var result = await _sqlServerDataAccess.GetSaleTypes();

            return result;
        }

        public async Task SaveExportAgentAsync(AgentViewModel model)
        {
            string sWebRootFolder = $@"{_hostingEnvironment.ContentRootPath}/Data/Export";
            string sFileName = $"Agent-{model.AgentName}.xlsx";

            await Task.Run(() =>
            {
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                }

                var data = model;

                ExcelPackage excelPackage = Assembly.GetExecutingAssembly().GenerateExcelPackage(nameof(data));
                excelPackage.SaveAs(file);

                foreach(var item in data.TotalSalesPerYear)
                {
                    sFileName = $"Agent-{model.AgentName}-{item.Year}.xlsx";
                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                    if (file.Exists)
                    {
                        file.Delete();
                        file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                    }

                    ExcelPackage excel = null;

                    excel = item.TotalSalesPerMonth.ToWorksheet($"{item.Year}")
                                    .WithConfiguration(configuration => configuration.WithColumnConfiguration(x => x.AutoFit()))
                                    .WithColumn(x => x.Month, "Month")
                                    .WithColumn(x => x.BWCServicesCount, "Bwc Service Count")
                                    .WithColumn(x => x.CommissionSum, "Commission Sum")
                                    .WithColumn(x => x.CompoundingMemberSum, "Compounding Member Sum")
                                    .WithColumn(x => x.CompoundingMemberSumCommission, "Compounding Member Sum Commission")
                                    .WithColumn(x => x.CompoundingNonMemberSum, "Compounding Non Member Sum")
                                    .WithColumn(x => x.CompoundingNonMemberSumCommission, "Compounding Non Member Sum Commission")
                                    .WithColumn(x => x.MembershipCount, "Membership Count")
                                    .WithColumn(x => x.MembershipSum, "Membership Sum")
                                    .WithColumn(x => x.MembershipSumCommission, "Membership Sum Commission")
                                    .WithColumn(x => x.ServiceMemberSum, "Service Member Sum")
                                    .WithColumn(x => x.ServiceMemberSumCommission, "Service Member Sum Commission")
                                    .WithColumn(x => x.ServiceNonMemberSum, "Service Non Member Sum")
                                    .WithColumn(x => x.ServiceNonMemberSumCommission, "Service Non Member Sum Commission")
                                    .WithColumn(x => x.TotalSales, "Total Sales")
                                    .WithTitle($"Summary commission of year {item.Year}")
                                    .ToExcelPackage();
                    excel.SaveAs(file);
                }
            });
        }

        public void ExportAgent(AgentViewModel model)
        {
            string sWebRootFolder = $@"{_hostingEnvironment.ContentRootPath}/Data/Export";
            string sFileName = $"Agent-{model.AgentName}.xlsx";

            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = null;
                var years = model.TotalSalesPerYear.Select(d => d.Year).Distinct();

                foreach (var year in years)
                {
                    var agentYear = model.TotalSalesPerYear.Where(a => a.Year == year).FirstOrDefault();

                    worksheet = package.Workbook.Worksheets.Add($"{year}");

                    worksheet.Cells[1, 1].Value = $"Total Sales {year}";
                    worksheet.Cells[2, 1].Value = $"Total Commission {year}";

                    worksheet.Cells[1, 2].Value = agentYear.TotalSalesYear;
                    worksheet.Cells[2, 2].Value = agentYear.TotalCommissionYear;

                    worksheet.Cells[4, 1].Value = "Month";
                    worksheet.Cells[4, 2].Value = "Total Membership";
                    worksheet.Cells[4, 3].Value = "Total BWC Services";
                    worksheet.Cells[4, 4].Value = "Total Sales";
                    worksheet.Cells[4, 5].Value = "Membership Revenue";
                    worksheet.Cells[4, 6].Value = "Membership Commission Revenue";
                    worksheet.Cells[4, 7].Value = "ServiceMember Revenue";
                    worksheet.Cells[4, 8].Value = "ServiceMember Commission Revenue";
                    worksheet.Cells[4, 9].Value = "ServiceNonMember Revenue";
                    worksheet.Cells[4, 10].Value = "ServiceNonMember Commission Revenue";
                    worksheet.Cells[4, 11].Value = "Compounding Member Revenue";
                    worksheet.Cells[4, 12].Value = "Compounding Member Commission Revenue";
                    worksheet.Cells[4, 13].Value = "Compounding NonMember Revenue";
                    worksheet.Cells[4, 14].Value = "Compounding NonMember Commission Revenue";
                    worksheet.Cells[4, 15].Value = "Total Commission Revenue";

                    for (int i = 1; i <= 12; i++)
                    {
                        var agentMonth = agentYear.TotalSalesPerMonth.Where(am => (int)am.Month == i).FirstOrDefault();
                        if(agentMonth == null) agentMonth = new TotalSalesPerMonthViewModel();

                        worksheet.Cells[i + 4, 1].Value = $"{(Month)i}";
                        worksheet.Cells[i + 4, 2].Value = agentMonth.MembershipCount;
                        worksheet.Cells[i + 4, 3].Value = agentMonth.BWCServicesCount;
                        worksheet.Cells[i + 4, 4].Value = agentMonth.TotalSales;
                        worksheet.Cells[i + 4, 5].Value = agentMonth.MembershipSum;
                        worksheet.Cells[i + 4, 6].Value = agentMonth.MembershipSumCommission;
                        worksheet.Cells[i + 4, 7].Value = agentMonth.ServiceMemberSum;
                        worksheet.Cells[i + 4, 8].Value = agentMonth.ServiceMemberSumCommission;
                        worksheet.Cells[i + 4, 9].Value = agentMonth.ServiceNonMemberSum;
                        worksheet.Cells[i + 4, 10].Value = agentMonth.ServiceNonMemberSumCommission;
                        worksheet.Cells[i + 4, 11].Value = agentMonth.CompoundingMemberSum;
                        worksheet.Cells[i + 4, 12].Value = agentMonth.CompoundingMemberSumCommission;
                        worksheet.Cells[i + 4, 13].Value = agentMonth.CompoundingNonMemberSum;
                        worksheet.Cells[i + 4, 14].Value = agentMonth.CompoundingNonMemberSumCommission;
                        worksheet.Cells[i + 4, 15].Value = agentMonth.CommissionSum;
                    }
                }

                package.Save();
            }
        }

        public async Task<IEnumerable<CommissionItem>> getItemlist()
        {
            var result = await _sqlServerDataAccess.GetItemList();

            return result;
        }

        public async Task<Dictionary<string, object>> GenerateAgentExcel()
        {
            try
            {
                var result = new Dictionary<string, object>();
                //var excelPath = _excelPath.Release;
                var excelPath = _excelPath.Test;
                var excelName = _excelFileName.AgentSummaryExcel;
                var excelHeader = _excelHeader.AgentSummaryExcel;
                var agent = await _sqlServerDataAccess.GetAgentList();
                //ExcelViewModel.GenerateExcel();   
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
