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


namespace AgentReferralSystem.Api.Data.ViewModels
{
    public class LogViewModel
    {
        private LogFilePath _logPath { get; set; }
        private LogFileName _logfileName { get; set; }
        private List<string> logTextList { get; set; }
        private string logText { get; set; }
        private List<FunctionResultViewModel> resultViewList { get; set; } = new List<FunctionResultViewModel>();

        public LogViewModel(LogFilePath _logPath, LogFileName _logfileName, string logText, List<FunctionResultViewModel> resultViewList)
        {
            this._logPath = _logPath;
            this._logfileName = _logfileName;
            this.logText = logText;
            this.logTextList = new List<string>();
            this.resultViewList = resultViewList;
        }

        public LogViewModel(LogFilePath _logPath, LogFileName _logfileName,List<string> logTextList, List<FunctionResultViewModel> resultViewList)
        {
            this._logPath = _logPath;
            this._logfileName = _logfileName;
            this.logText = "";
            this.logTextList = logTextList;
            this.resultViewList = resultViewList;
        }

        public LogViewModel(LogFilePath _logPath, LogFileName _logfileName, string logText)
        {
            this._logPath = _logPath;
            this._logfileName = _logfileName;
            this.logText = logText;
            this.logTextList = new List<string>();
            this.resultViewList = new List<FunctionResultViewModel>();
        }

        public LogViewModel(LogFilePath _logPath, LogFileName _logfileName, List<string> logTextList)
        {
            this._logPath = _logPath;
            this._logfileName = _logfileName;
            this.logText = "";
            this.logTextList = logTextList;
            this.resultViewList = new List<FunctionResultViewModel>();
        }

        public LogViewModel(LogFilePath _logPath, LogFileName _logfileName, List<FunctionResultViewModel> resultViewList)
        {
            this._logPath = _logPath;
            this._logfileName = _logfileName;
            this.logText = "";
            this.logTextList = new List<string>();
            this.resultViewList = resultViewList;
        }

        public LogViewModel()
        {
            this._logPath = _logPath;
            this._logfileName = _logfileName;
            this.logText = "";
            this.logTextList = new List<string>();
            this.resultViewList = new List<FunctionResultViewModel>();
        }

        public async Task writeLog(string LogMode, string LogType)
        {
            try
            {
                string LogFilePath = "";
                switch (LogMode.ToLower())
                {
                    case "test":
                        LogFilePath = _logPath.Test;
                        break;
                    case "release":
                        LogFilePath = _logPath.Release;
                        break;
                    default:
                        throw new Exception("LogMode INVALID!!!");
                }
                string LogFileName = "";
                switch (LogType.ToLower())
                {
                    case "agent":
                        LogFileName = _logfileName.AgentLog;
                        break;
                    case "test":
                        LogFileName = _logfileName.TestLog;
                        break;
                    default:
                        throw new Exception("LogType INVALID!!!");
                }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(LogFilePath + DateTime.Now.ToString("ddMMyyyyHHmmss") + LogFileName))
                {
                    if (this.logText != "") await file.WriteAsync(this.logText);
                    else if (this.logTextList.Count > 0)
                    {
                        foreach (string line in logTextList)
                        {
                            await file.WriteLineAsync(line);
                        }
                    }

                    file.Flush();
                    file.Close();
                    file.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
