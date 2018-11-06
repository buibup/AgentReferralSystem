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
    public class AgentOverviewModel
    {
        private ISqlServerDataAccess _sqlDataAccess;

        public AgentOverviewModel(Agent agent, int processMonth, int processYear, ISqlServerDataAccess sqlServerDataAccess)
        {
            this._sqlDataAccess = sqlServerDataAccess;
            this.agent = agent;
            this.LoadSaleList(processMonth, processYear);
            this.LoadExchangeList(processMonth, processYear);
        }

        public Agent agent { get; set; }
        public List<CommissionItem> saleList { get; set; }
        public List<RewardExchange> exchangeList { get; set; }

        private async void LoadSaleList(int processMonth,int processYear)
        {
            var data = (await (_sqlDataAccess.GetCommissionItemById(this.agent.AgentId))).ToList<CommissionItem>();
            //filter month Year
            this.saleList = data.Where(x => x.Episode_Date.Month == processMonth && x.Episode_Date.Year == processYear).ToList();
        }

        private async void LoadExchangeList(int processMonth, int processYear)
        {
            var data = (await (_sqlDataAccess.GetRewardExchangeList(this.agent.AgentId))).ToList<RewardExchange>();
            //filter month Year
            this.exchangeList = data.Where(x => x.CreateDate.Month == processMonth && x.CreateDate.Year == processYear).ToList();
        }
    }
}
