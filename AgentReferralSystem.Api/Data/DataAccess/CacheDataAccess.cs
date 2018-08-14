﻿using AgentReferralSystem.Api.Data.Config;
using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Query;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.DataAccess
{
    public class CacheDataAccess : ICacheDataAccess
    {
        private readonly ConnectionStrings _connectionStrings;
        public CacheDataAccess(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<IEnumerable<ARCItmMast>> GetARCItmMastCompoundingAsync()
        {
            try
            {
                using(var conn = new OdbcConnection(_connectionStrings.Cache))
                {
                    var data = await conn.QueryAsync<ARCItmMast>(CacheQuery.GetARCItmMastCompounding());

                    return data;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<ARPatientBill>> GetARPatientsBillsByReferralTypeRowIdAsync(int reftRowId)
        {
            try
            {
                using(var conn = new OdbcConnection(_connectionStrings.Cache))
                {
                    var data = await conn.QueryAsync<ARPatientBill>(CacheQuery.GetARPatientsBillsByReferralTypeRowId(), new { REFT_RowId = reftRowId });

                    return data;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<PACReferralType>> GetPACReferralTypeAllAsync()
        {
            try
            {
                using(var conn = new OdbcConnection(_connectionStrings.Cache))
                {
                    var data = await conn.QueryAsync<PACReferralType>(CacheQuery.GetPACReferralTypeAll());

                    return data;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<QBWCMEMBERS> GetQBWCMEMBERSByPapmiRowIdAsync(int papmiRowId)
        {
            try
            {
                using(var conn = new OdbcConnection(_connectionStrings.Cache))
                {
                    var data = await conn.QueryAsync<QBWCMEMBERS>(CacheQuery.GetQBWCMEMBERSByPapmiRowId(), new { QUESPAPatMasDR = papmiRowId });

                    return data.FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<QBWCMEMBERS>> GetQBWCMEMBERSByPapmiRowIdListAsync(IEnumerable<int> papmiRowIdList)
        {
            try
            {
                using (var conn = new OdbcConnection(_connectionStrings.Cache))
                {
                    var data = await conn.QueryAsync<QBWCMEMBERS>(CacheQuery.GetQBWCMEMBERSByPapmiRowIdList(papmiRowIdList));

                    return data;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
