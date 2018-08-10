using AgentReferralSystem.Api.Data.Models.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Moq
{
    public static class SaleTypeMoq
    {

        private static List<SaleType> SaleTypes { get; } = new List<SaleType>()
        {
            new SaleType{ SaleTypeId = 1, SaleTypeName = "Membership" },
            new SaleType{ SaleTypeId = 2, SaleTypeName = "ServiceMember" },
            new SaleType{ SaleTypeId = 3, SaleTypeName = "ServiceNonMember" },
            new SaleType{ SaleTypeId = 4, SaleTypeName = "CompoundingMember" },
            new SaleType{ SaleTypeId = 5, SaleTypeName = "CompoundingNonMember" }
        };

        public static SaleType GetSaleTypeById(this int id)
        {
            return SaleTypes.Where(s => s.SaleTypeId == id).FirstOrDefault();
        }
    }
}
