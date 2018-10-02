using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentReferralSystem.Api.Data.Models.SqlServer;

namespace AgentReferralSystem.Api.Data.Moq
{
    public class PercentConfigMoq
    {
        private static List<PercentConfig> Percentconfig { get; } = new List<PercentConfig>()
        {
            new PercentConfig
            {
                id = 1,
                ConfigName = "BaseCommission",
                ConfigType = "CommissionRate",
                value1 = (decimal)2.50,
                value1Meaning = "Percent",
                value2 = null,
                value2Meaning = null,
                isDelete = false,
            },
            new PercentConfig
            {
                id = 2,
                ConfigName = "Rate1",
                ConfigType = "CommissionRate",
                value1 = (decimal)4,
                value1Meaning = "Percent",
                value2 = "1000000",
                value2Meaning = "Target",
                isDelete = false,
            },
            new PercentConfig
            {
                id = 3,
                ConfigName = "Rate2",
                ConfigType = "CommissionRate",
                value1 = (decimal)4.5,
                value1Meaning = "Percent",
                value2 = "1500000",
                value2Meaning = "Target",
                isDelete = false,
            },
            new PercentConfig
            {
                id = 4,
                ConfigName = "Rate3",
                ConfigType = "CommissionRate",
                value1 = (decimal)5,
                value1Meaning = "Percent",
                value2 = "2000000",
                value2Meaning = "Target",
                isDelete = false,
            },
            new PercentConfig
            {
                id = 5,
                ConfigName = "Rate4",
                ConfigType = "CommissionRate",
                value1 = (decimal)6.5,
                value1Meaning = "Percent",
                value2 = "3060448",
                value2Meaning = "Target",
                isDelete = false,
            },
            new PercentConfig
            {
                id = 6,
                ConfigName = "Rate5",
                ConfigType = "CommissionRate",
                value1 = (decimal)10,
                value1Meaning = "Percent",
                value2 = "10000000",
                value2Meaning = "Target",
                isDelete = false,
            },
            new PercentConfig
            {
                id = 7,
                ConfigName = "Reward",
                ConfigType = "RewardPointRate",
                value1 = (decimal)10000,
                value1Meaning = "Flat",
                value2 = "",
                value2Meaning = "",
                isDelete = false,
            },
        };
        public static List<PercentConfig> getConfig()
        {
            return Percentconfig;
        }
    }
}
