using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace UndisReportCollector
{

    public class Log
    {
        private DateTime timestamp = DateTime.Now;
        private string jobId = null;
        private string operationType = null;
        private string refreshMode = null;
        private string status = null;
        private bool rebootRequested = false;
        private int durationInSeconds = 0;
        private int numberOfResources = 0;
        private string hostname = null;
        private enum LevelEnum { UNDIFINED, CRITICAL, ERROR, WARN, INFO, VERBOSE };

        /// <summary>
        /// Folder where we write logs
        /// </summary>
        private string LogsFolder = ConfigurationManager.AppSettings["UndisLogsFolder"];   

        public Log (DSCReport dscReport, StatusData statusData)
        {
            if (dscReport != null)
            {
                // If there is no status in DSC Report, Job is InProgress
                if (dscReport.Status == null)
                {
                    this.status = "InProgress";
                }
                else
                {
                    this.status = dscReport.Status;
                }

                // Get data from DSC Report
                this.jobId = dscReport.JobId;
                this.operationType = dscReport.OperationType;
                this.refreshMode = dscReport.RefreshMode;
                this.rebootRequested = dscReport.RebootRequested;

                // Get data from DSC StatusReport, if available
                if (statusData != null)
                {
                    this.durationInSeconds = statusData.DurationInSeconds;
                    this.numberOfResources = statusData.NumberOfResources;
                    this.hostname = statusData.Hostname;
                }
            }
        }

        /// <summary>
        /// Write log line in log file
        /// </summary>
        public void Flush ()
        {
            int configLogLevel = (int)LevelEnum.INFO;
            int logLevel = (int)LevelEnum.INFO;

            if (ConfigurationManager.AppSettings["UndisLogLevel"] != null)
            {
                if (LevelEnum.IsDefined(typeof(LevelEnum), ConfigurationManager.AppSettings["UndisLogLevel"].ToUpper()))
                {
                    configLogLevel = (int)LevelEnum.Parse(typeof(LevelEnum), ConfigurationManager.AppSettings["UndisLogLevel"].ToUpper());
                }
            }

            switch (this.status)
            {
                case "InProgress":
                    logLevel = (int)LevelEnum.VERBOSE;
                    break;
                case "Failed":
                    logLevel = (int)LevelEnum.ERROR;
                    break;
            }

            if (Directory.Exists(LogsFolder) && logLevel <= configLogLevel)
            {
                // Build log filename
                var fileName = $"DSCReports_{timestamp.ToString("yyyyMMdd")}.log";
                var fileFullPath = Path.Combine(LogsFolder, fileName);

                // Build log line
                string logLine = "";
                logLine += timestamp.ToString("yyyy-MM-dd HH:mm:ss ");

                logLine += Enum.GetName(typeof(LevelEnum), logLevel);

                logLine += $" Hostname={this.hostname} OperationType={this.operationType} Status={this.status} RefreshMode={this.refreshMode}";
                logLine += $" RebootRequested={this.rebootRequested} DurationInSeconds={this.durationInSeconds} NumberOfResources={this.numberOfResources} JobId={this.jobId}";

                try
                {
                    // Write log line in log file
                    using (StreamWriter file = new StreamWriter(fileFullPath, true))
                    {
                        file.WriteLine(logLine);
                        file.Close();
                    }
                }
                catch
                {
                    // TODO if needed
                }
            }
        }
    }
}

