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
        public AgentService(ICacheDataAccess cacheDataAccess,
            ISqlServerDataAccess sqlServerDataAccess,
            IHostingEnvironment hostingEnvironment)
        {
            _cacheDataAccess = cacheDataAccess;
            _sqlServerDataAccess = sqlServerDataAccess;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task AddOrUpdateAgentAsync(Agent agent)
        {
            await _sqlServerDataAccess.AddOrUpdateAgentAsync(agent);
        }

        public async Task DeleteAgentAsync(int agentId)
        {
            await _sqlServerDataAccess.DeleteAgentAsync(agentId);
        }

        public async Task<AgentOutput> GetAgentByIdAsync(int agentId)
        {
            var model = await _sqlServerDataAccess.GetAgentByIdAsync(agentId);

            return model;
        }

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

                result = agent.AgentViewModelProcess(patientsBills, memberRegisList, itemCompoundingList);
            }

            return result;
        }

        public async Task<IEnumerable<PACReferralType>> GetPACReferralTypesAsync()
        {
            var result = await _cacheDataAccess.GetPACReferralTypeAllAsync();

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
    }
}
