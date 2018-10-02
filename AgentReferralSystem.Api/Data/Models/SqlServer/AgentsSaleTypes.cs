﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Models.SqlServer
{
    public class AgentsSaleTypes
    {
        [Column(TypeName = "decimal(5, 2)")]
        public decimal BaseCommission { get; set; }

        [Column(TypeName = "decimal(14, 2)")]
        public decimal Target { get; set; }

        public int TargetPeriod { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal IncreaseIfTargetMet { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Maximum { get; set; }

        public int ResetToBase { get; set; }

        public string ApplicableTargetInrease { get; set; }

        public int SaleTypeId { get; set; }

        public int AgentId { get; set; }
    }
}
