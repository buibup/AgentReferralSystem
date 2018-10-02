using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentReferralSystem.Api.Data.ViewModels;


namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class PercentConfig
    {
        public int id { get; set; }
        public string ConfigName { get; set; }
        public string ConfigType { get; set; }
        public decimal value1 { get; set; }
        public string value1Meaning { get; set; }
        public string value2 { get; set; }
        public string value2Meaning { get; set; }
        public bool isDelete { get; set; }
    }
}
