using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace AgentReferralSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        //Hard Reset API 

        //ALL IN ONE API (Do All API)

        //Schedule Monthly Task (MonthlyCommiss -> Monthly Reward)

        //Schedule Diary Task (MigrateCommiss -> Diary Commission)

        //Schedule MigrateCommissionItem
        [HttpPost("MigrateCommissionItem")]
        public async Task<IActionResult> MigrateCommissionItem()
        {
            try
            {
                await _scheduleService.MigrateCommissionItem();
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Schedule DiaryCommission
        [HttpPost("RecalDiaryCommission")]
        public async Task<IActionResult> RecalDiaryCommission()
        {
            try
            {
                await _scheduleService.RecalDiaryCommission();
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
            
        //Schedule MonthyCommission
        [HttpPost("MonthyCommission")]
        public async Task<IActionResult> MonthyCommission()
        {
            try
            {
                await _scheduleService.ScheduleMonthlyReward();
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //Schedule DiaryReward
        [HttpPost("DiaryReward")]
        public async Task<IActionResult> DiaryReward()
        {
            try
            {
                await _scheduleService.ScheduleMonthlyReward();
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Schedule MonthlyReward
        [HttpPost("MonnthlyReward")]
        public async Task<IActionResult> MonthlyReward()
        {
            try
            {
                await _scheduleService.ScheduleMonthlyReward();
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        //Testing Write Log File
        [HttpPost("TryWriteTextFile")]
        public async Task<IActionResult> TryWriteTextFile([FromHeader]string Code)
        {
            try
            {
                if (Code == "89TLTest")
                {
                    var result = await _scheduleService.TestLog();

                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Testing Write Excel File
        [HttpPost("TryWriteExcelFile")]
        public async Task<IActionResult> TryWriteExcelFile([FromHeader]string code)
        {
            try
            {
                if (code == "89ELTest")
                {
                    string url = await _scheduleService.TestExcel();

                    var memory = new MemoryStream();

                    using (var stream = new FileStream("wwwroot\\Excel\\" + url, FileMode.Open))
                    {                      
                        await stream.CopyToAsync(memory);

                        stream.Close();
                        stream.Dispose();
                    }
                    memory.Position = 0;
                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", url);
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("TryGetWriteExcelFile")]
        public async Task<IActionResult> TryWriteExcelFile()
        {
            try
            {
                string url = await _scheduleService.TestExcel();

                var memory = new MemoryStream();

                using (var stream = new FileStream("wwwroot\\Excel\\" + url, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", url);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
