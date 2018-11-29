using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Services.Interfaces;
using AgentReferralSystem.Api.Data.ViewModels;
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

        #region MobileUIAPI
        [HttpPost("GetAgentProfileByAgentId/{agentId}")]
        public async Task<IActionResult> GetAgentProfileByAgentId([FromHeader]string Token, int agentId)
        {
            try
            {
                var result = await _agentService.GetAgentProfileByAgentId(agentId);

                if (result != null) return Ok(new { status = "OK", data = result["data"], Message = result["Message"] });
                else return Ok(new { status = "Error", Message = "GetAgentProfile Error : Data is Null" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", Message = "Exception Thrown: " + ex.Message });
            }
        }

        [HttpPost("GetAgentProfileByAgentCode/{agentCode}")]
        public async Task<IActionResult> GetAgentProfileByAgentCode([FromHeader]string Token, string agentCode)
        {
            try
            {
                var result = await _agentService.GetAgentProfileByAgentCode(agentCode);

                if (result != null) return Ok(new { status = "OK", data = result["data"], Message = result["Message"] });
                else return Ok(new { status = "Error", Message = "GetAgentProfile Error : Data is Null" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", Message = "Exception Thrown: " + ex.Message });
            }
        }

        [HttpPost("GetAgentCustomerByAgentId/{agentId}")]
        public async Task<IActionResult> GetAgentCustomerByAgentId([FromHeader]string Token,int agentId)
        {
            try
            {
                var result = await _agentService.GetAgentCustomerByAgentId(agentId);

                if (result != null) return Ok(new { status = "OK", data = result["data"], Message = result["Message"] });
                else return Ok(new { status = "Error", Message = "GetAgentCustomer Error : Data is Null" });
            }
            catch(Exception ex)
            {
                return Ok(new { status = "Error", Message = "Exception Thrown: " + ex.Message });
            }
        }

        [HttpPost("GetAgentCustomerByAgentCode/{agentCode}")]
        public async Task<IActionResult> GetAgentCustomerByAgentCode([FromHeader]string Token, string agentCode)
        {
            try
            {
                var result = await _agentService.GetAgentCustomerByAgentCode(agentCode);

                if (result != null) return Ok(new { status = "OK", data = result["data"], Message = result["Message"] });
                else return Ok(new { status = "Error", Message = "GetAgentCustomer Error : Data is Null" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", Message = "Exception Thrown: " + ex.Message });
            }
        }

        [HttpPost("GetAgentCommissionByAgentId/{agentId}")]
        public async Task<IActionResult> GetAgentCommissionByAgentId([FromHeader]string Token, int agentId)
        {
            try
            {
                var result = await _agentService.GetAgentCommissionByAgentId(agentId);

                if (result != null) return Ok(new { status = "OK", data = result["data"], Message = result["Message"] });
                else return Ok(new { status = "Error", Message = "GetAgentCustomer Error : Data is Null" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", Message = "Exception Thrown: " + ex.Message });
            }
        }

        [HttpPost("GetAgentCommissionByAgentCode/{agentCode}")]
        public async Task<IActionResult> GetAgentCommissionByAgentCode([FromHeader]string Token, string agentCode)
        {
            try
            {
                var result = await _agentService.GetAgentCommissionByAgentCode(agentCode);

                if (result != null) return Ok(new { status = "OK", data = result["data"], Message = result["Message"] });
                else return Ok(new { status = "Error", Message = "GetAgentCustomer Error : Data is Null" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", Message = "Exception Thrown: " + ex.Message });
            }
        }

        [HttpPost("GetAgentTargetByAgentId/{agentId}")]
        public async Task<IActionResult> GetAgentTargetByAgentId([FromHeader]string Token, int agentId)
        {
            try
            {
                var result = await _agentService.GetAgentTargetByAgentId(agentId);

                if (result != null) return Ok(new { status = "OK", data = result["data"], Message = result["Message"] });
                else return Ok(new { status = "Error", Message = "GetAgentTarget Error : Data is Null" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", Message = "Exception Thrown: " + ex.Message });
            }
        }

        [HttpPost("GetAgentTargetByAgentCode/{agentCode}")]
        public async Task<IActionResult> GetAgentTargetByAgentCode([FromHeader]string Token, string agentCode)
        {
            try
            {
                var result = await _agentService.GetAgentTargetByAgentCode(agentCode);

                if (result != null) return Ok(new { status = "OK", data = result["data"], Message = result["Message"] });
                else return Ok(new { status = "Error", Message = "GetAgentTarget Error : Data is Null" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", Message = "Exception Thrown: " + ex.Message });
            }
        }
        #endregion

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

            // test export excel
            //_agentService.ExportAgent(result);

            return Ok(result);
        }

        [HttpGet("LoadAgentSummarizeById/{agentId}-{Year}-{Month}")]
        public async Task<IActionResult> LoadAgentSummarizeById(int agentId, int Year,int Month)
        {
            var result = await _agentService.LoadAgentSummarizeByIdAsync(agentId, Year, Month);

            if (result == null) return NotFound();

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

        [HttpGet("GetAgentByAgentCode/{agentCode}")]
        public async Task<IActionResult> GetAgentByAgentCodeAsync(string agentCode)
        {
            try
            {
                var agent = await _agentService.GetAgentByAgentCodeAsync(agentCode);

                if (agent == null) return NotFound();

                return Ok(agent);
            }
            catch (Exception ex)
            {
                return Ok(new { status = "ERROR", message = "Exception Thrown: " + ex.Message });
            }
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
            try
            {
                if (agent == null) return BadRequest();
                //throw new Exception("Test");
                await _agentService.AddOrUpdateAgentAsync(agent);

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete("DeleteAgent/{agentId}")]
        public async Task<IActionResult> DeleteAgentAsync(int agentId)
        {
            await _agentService.DeleteAgentAsync(agentId);

            return NoContent();
        }

        [HttpPost("UploadAgentImage")]
        public async Task<IActionResult> UploadAgentImage([FromBody]ImageViewModel jsonData = null)
        {
            try
            {
                int agentId = jsonData.agentId;
                string Image = jsonData.Image;
                await _agentService.UploadAgentImage(agentId, Image);

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("DownloadAgentExcel")]
        public async Task<IActionResult> DownloadAgentExcel()
        {
            try
            {
                var result = await _agentService.GenerateAgentExcel();
                return Ok(new { status = "OK", data = result["data"], Message = result["Message"] });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "ERROR", message = "Exception Thrown: " + ex.Message });
            }
        }
    }
}
