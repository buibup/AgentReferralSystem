﻿using System;
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
    public class AgentsController : ControllerBase
    {
        private readonly IAgentService _agentService;
        public AgentsController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpGet("GetAgentList")]
        public async Task<IActionResult> GetAgentList()
        {
            var result = await _agentService.GetAgentList();

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("GetAgentReportById/{agentId}")]
        public async Task<IActionResult> GetAgentViewModelByAgentIdAsync(int agentId)
        {
            var result = await _agentService.GetAgentViewModelByAgentIdAsync(agentId);

            if (result == null || string.IsNullOrEmpty(result.AgentName)) return NotFound();

            return Ok(result);
        }

        [HttpGet("GetPACReferralTypes")]
        [HttpGet("GetPACReferralTypes/{search}")]
        public async Task<IActionResult> GetPACReferralTypesAsync(string search = "")
        {
            var result = await _agentService.GetPACReferralTypesAsync(search);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("GetPACReferralTypesById")]
        [HttpGet("GetPACReferralTypesById/{agentId}")]
        public async Task<IActionResult> GetPACReferralTypesByIdAsync(int agentId)
        {
            var result = await _agentService.GetPACReferralTypesByIdAsync(agentId);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("GetAgentById/{agentId}")]
        public async Task<IActionResult> GetAgentByIdAsync(int agentId)
        {
            var result = await _agentService.GetAgentByIdAsync(agentId);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("GetSaleTypes")]
        public async Task<IActionResult> GetSaleTypes()
        {
            {
                var result = await _agentService.GetSaleTypes();

                if (result == null) return NotFound();

                return Ok(result);
            }
        }

        [HttpPost("AddOrUpdateAgent")]
        public async Task<IActionResult> AddOrUpdateAgentAsync(Agent agent)
        {
            if (agent == null) return BadRequest();

            await _agentService.AddOrUpdateAgentAsync(agent);

            return Ok();
        }

        [HttpDelete("DeleteAgent/{agentId}")]
        public async Task<IActionResult> DeleteAgentAsync(int agentId)
        {
            await _agentService.DeleteAgentAsync(agentId);

            return NoContent();
        }
    }
}
