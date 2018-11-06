using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Query
{
    public static class SqlServerQuery
    {
        public static string GetPercentConfigSQLString(int type,List<string> Criteria)
        {
            switch(type)
            {
                case 1:
                    string ConfigType = Criteria[0];
                    string ConfigName = Criteria[1];
                    return String.Format(@"select * from dbo.PercentConfig where isNull(isDelete,0) = 0 and ConfigType = '{0}' and ConfigName = '{1}'"
                                                , ConfigType, ConfigName);
                case 2:
                    string ConfigType2 = Criteria[0];
                    return String.Format(@"select * from dbo.PercentConfig where isNull(isDelete,0) = 0 and ConfigType = '{0}'"
                            , ConfigType2);
                default: return "";
            }
        }
    }
}
