using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EPPlus.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AgentReferralSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ValuesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("Export")]
        public IActionResult Export()
        {
            string sWebRootFolder = $@"{_hostingEnvironment.ContentRootPath}/Data/Export";
            string sFileName = @"export.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);

            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }

            List<PersonDto> persons = GetPersonDtos();

            List<PersonDto> pre50 = persons.Where(x => x.YearBorn < 1950).ToList();
            List<PersonDto> post50 = persons.Where(x => x.YearBorn >= 1950).ToList();

            // Convert list into ExcelPackage
            ExcelPackage excelPackage = persons.ToExcelPackage();

            // Convert list into byte array 
            byte[] excelPackageXlsx = persons.ToXlsx();


            // Generate ExcelPackage with configuration
            excelPackage = pre50.ToWorksheet("< 1950")
                                     .WithConfiguration(configuration => configuration.WithColumnConfiguration(x => x.AutoFit()))
                                     .WithColumn(x => x.FirstName, "First Name")
                                     .WithColumn(x => x.LastName, "Last Name")
                                     .WithColumn(x => x.YearBorn, "Year of Birth")
                                     .WithTitle("< 1950")
                                .NextWorksheet(post50, "> 1950")
                                     .WithColumn(x => x.LastName, "Last Name")
                                     .WithColumn(x => x.YearBorn, "Year of Birth")
                                     .WithTitle("> 1950")
                                     .ToExcelPackage();

            excelPackage.SaveAs(file);

            return Ok(URL);
        }

        private List<PersonDto> GetPersonDtos()
        {
            var result = new List<PersonDto>()
            {
                new PersonDto { FirstName = "test 1", LastName = "test 1", YearBorn = 1940 , NotMapped = 0 },
                new PersonDto { FirstName = "test 2", LastName = "test 2", YearBorn = 1941 , NotMapped = 0 },
                new PersonDto { FirstName = "test 3", LastName = "test 3", YearBorn = 1942 , NotMapped = 0 },
                new PersonDto { FirstName = "test 4", LastName = "test 4", YearBorn = 1943 , NotMapped = 0 },
                new PersonDto { FirstName = "test 5", LastName = "test 5", YearBorn = 1951 , NotMapped = 0 },
                new PersonDto { FirstName = "test 6", LastName = "test 6", YearBorn = 1990 , NotMapped = 0 },
                new PersonDto { FirstName = "test 7", LastName = "test 7", YearBorn = 1991 , NotMapped = 0 },
                new PersonDto { FirstName = "test 8", LastName = "test 8", YearBorn = 1992 , NotMapped = 0 },
            };

            return result;
        }

        public class PersonDto
        {
            [ExcelTableColumn("First name")]
            [Required(ErrorMessage = "First name cannot be empty.")]
            [MaxLength(50, ErrorMessage = "First name cannot be more than {1} characters.")]
            public string FirstName { get; set; }

            [ExcelTableColumn("Last name")]
            public string LastName { get; set; }

            [ExcelTableColumn(3)]
            [Range(1900, 2050, ErrorMessage = "Please enter a value bigger than {1}")]
            public int YearBorn { get; set; }

            public decimal NotMapped { get; set; }
        }
    }
}
