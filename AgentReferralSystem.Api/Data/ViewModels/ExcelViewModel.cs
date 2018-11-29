using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.Config;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Query;
using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using EPPlus.Core.Extensions;
using OfficeOpenXml;

namespace AgentReferralSystem.Api.Data.ViewModels
{
    public class ExcelViewModel
    {
        public List<string> headerList { get; set; } = new List<string>();
        public List<Dictionary<string, object>> dataList { get; set; } = new List<Dictionary<string, object>>();
        public string path { get; set; }
        public string filename { get; set; }

        public ExcelViewModel()
        {

        }

        public ExcelViewModel(List<string> headerList, List<Dictionary<string, object>> dataList, string path, string filename)
        {
            this.headerList = headerList;
            this.dataList = dataList;
            this.path = path;
            this.filename = filename;
        }

        public static async Task GenerateExcel(List<string[]> headerList, List<Dictionary<string, object>> dataList, string path, string filename)
        {
            string url = path + filename;
            await Task.Run(() =>
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var ws1 = excel.Workbook.Worksheets.Add("Worksheet1");
                    FileInfo excelFile = new FileInfo(url);

                    // Determine the header range (e.g. A1:D1)
                    string headerRange = "A1:" + Char.ConvertFromUtf32(headerList[0].Length + 64) + "1";

                    // Target a worksheet
                    var worksheet = excel.Workbook.Worksheets["Worksheet1"];

                    // Popular header row data
                    worksheet.Cells[headerRange].LoadFromArrays(headerList);
                    excel.SaveAs(excelFile);
                    excel.Dispose();
                }
            });
        }

        public static async Task GenerateExcel()
        {
            await Task.Run(() =>
            {

            });
        }
    }
}
