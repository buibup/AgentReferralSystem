using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class ScheduleMonthLog
    {
        public string ProcessLog { get; set; }
        public string Remark { get; set; }
        public DateTime ScheduleMonthReward { get; set; }
        public DateTime ScheduleMonthCommission { get; set; }
        public DateTime CreateDate { get; set; }
    } 
}
