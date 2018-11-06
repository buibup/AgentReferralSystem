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
    public class FunctionResultViewModel
    {  
        private string message { get; set; }
        private List<Dictionary<string, object>> data { get; set; } = new List<Dictionary<string, object>>();

        public FunctionResultViewModel(string message, List<Dictionary<string,object>> data)
        {
            this.message = message;
            this.data = data;
        }

        public FunctionResultViewModel(string message)
        {
            this.message = message;
            this.data = new List<Dictionary<string, object>>();
        }

        public FunctionResultViewModel(List<Dictionary<string,object>> data)
        {
            this.message = "";
            this.data = data;
        }

        public FunctionResultViewModel()
        {
            this.message = "";
            this.data = new List<Dictionary<string, object>>();
        }
    }
}
