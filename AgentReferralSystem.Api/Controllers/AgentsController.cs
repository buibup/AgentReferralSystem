using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("GetAgentReportById/{agentId}")]
        public async Task<IActionResult> GetAgentViewModelByAgentIdAsync(int agentId)
        {
            var result = await _agentService.GetAgentViewModelByAgentIdAsync(agentId);

            if (result == null) return NotFound();

            return Ok(result);
        }

        // POST: api/Agents
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Agents/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
