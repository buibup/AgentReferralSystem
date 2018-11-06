using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Query;
using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.ViewModels
{
    public class AgentProfileViewModel
    {
        public int AgentId { get; set; }
        public string AgentCode { get; set; }
        public string AgentFullName { get; set; }
        //public string AgentFirstName { get; set; }
        //public string AgentMiddleName { get; set; }
        //public string AgentLastNmae { get; set; }
        public string AgentAddress { get; set; }
        public string EmailAddress { get; set; }
        public string BankAccount { get; set; }
        //public int BankAccountNumber { get; set; }
        public string ImageURL { get; set; }
        public DateTime SignAgreementDate { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        //public bool isInContract { get; set; }
        public string AgentRemark { get; set; }

        //Agent Target
        public decimal CurrentPercent { get; set; }
        public decimal CurrentSale { get; set; }
        public decimal TargetSale { get; set; }

        public AgentProfileViewModel(Agent agent)
        {
            this.AgentId = agent.AgentId;
            this.AgentCode = agent.AgentCode;
            this.AgentFullName = agent.AgentDesc;
            this.AgentAddress = agent.Address;
            this.EmailAddress = agent.Email;
            this.BankAccount = agent.BankAccount;
            this.ImageURL = agent.DisplayImage;
            this.SignAgreementDate = agent.AgreementDate;
            this.ContractStartDate = agent.DateFrom;
            this.ContractEndDate = agent.DateTo;
            this.AgentRemark = agent.Remark;
        }

        public AgentProfileViewModel(Agent agent, List<PercentConfig> ConfigList)
        {
            this.AgentId = agent.AgentId;
            this.AgentCode = agent.AgentCode;
            this.AgentFullName = agent.AgentDesc;
            this.AgentAddress = agent.Address;
            this.EmailAddress = agent.Email;
            this.BankAccount = agent.BankAccount;
            this.ImageURL = agent.DisplayImage;
            this.SignAgreementDate = agent.AgreementDate;
            this.ContractStartDate = agent.DateFrom;
            this.ContractEndDate = agent.DateTo;
            this.AgentRemark = agent.Remark;

            ProcessAgentTargetProfile(agent, ConfigList);
        }

        public void ProcessAgentTargetProfile(Agent agent, List<PercentConfig> ConfigList)
        {
            this.CurrentSale = agent.DisplayCommission;
            var CurrentConfig = ConfigList.Where(x => decimal.Parse(x.value2 ?? "0") >= agent.DisplayCommission).FirstOrDefault();
            var nextConfig = new PercentConfig();
            if (CurrentConfig == null) CurrentConfig = ConfigList.Last();
            int indexConfig = ConfigList.FindIndex(x => x.id == CurrentConfig.id);
            if ((indexConfig) == ConfigList.Count)
            {
                nextConfig = CurrentConfig;
            }
            else nextConfig = ConfigList[indexConfig + 1];
            this.TargetSale = decimal.Parse(nextConfig.value2);
            this.CurrentPercent = CurrentConfig.value1;
        }
    }
}
