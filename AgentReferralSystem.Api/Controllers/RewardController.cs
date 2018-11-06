using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgentReferralSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        public RewardController(IRewardService rewardService)
        {
            _rewardService = rewardService;
        }

        [HttpGet("GetRewardList")]
        public async Task<IActionResult> GetRewardList()
        {
            try
            {
                var result = await _rewardService.GetRewardList();

                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetRewardHistoryList")]
        public async Task<IActionResult> GetRewardHistoryList()
        {
            try
            {
                var result = await _rewardService.GetRewardHistoryList();

                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetRewardExchangeList")]
        [HttpGet("GetRewardExchangeList/{agentId}")]
        public async Task<IActionResult> GetRewardExchangeList(int agentId)
        {
            try
            {
                var result = await _rewardService.GetRewardExchangeList(agentId);

                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
