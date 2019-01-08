using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace UndisReportCollector
{
    public class HTTPModule : IHttpModule
    {
        /// <summary>
        /// Match the SendReport method url
        /// </summary>
        private static readonly Regex SendReportRegex = new Regex(@"^.+\/Nodes\(AgentId='(?<AgentId>.+)'\)\/SendReport$");

        /// <summary>
        /// Folder where we write reports
        /// </summary>
        private string ReportsFolder = ConfigurationManager.AppSettings["UndisReportsFolder"];

        /// <summary>
        /// Do we have to write reports when status is InProgress
        /// </summary>
        private bool ReportInProgress = false;

        public HTTPModule()
        {
        }

        public String ModuleName
        {
            get { return "Undis Report Collector HTTP Module"; }
        }

        // In the Init function, register for HttpApplication 
        // events by adding your handlers.
        public void Init(HttpApplication application)
        {
            // Register BeginRequest handler
            application.BeginRequest += Application_BeginRequest;

            if (ConfigurationManager.AppSettings["UndisReportInProgress"] != null)
            {
                ReportInProgress = Boolean.Parse(ConfigurationManager.AppSettings["UndisReportInProgress"]);
            }
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            // Create HttpApplication and HttpContext objects to access
            // request and response properties.
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;

            var urlMatch = SendReportRegex.Match(context.Request.Path);
            if (urlMatch.Success)
            {
                var agentId = urlMatch.Groups["AgentId"];

                // Loading POST data
                byte[] postData = new byte[context.Request.InputStream.Length];

                // save the position so that further processing of the request happens normally
                var originalPosition = context.Request.InputStream.Position;

                try
                {
                    context.Request.InputStream.Read(postData, 0, (int)context.Request.InputStream.Length);
                }
                catch
                {
                    postData = null;
                }
                finally
                {
                    // reset the position for normal request execution
                    context.Request.InputStream.Position = originalPosition;
                }

                // Decoding POST data as JSON
                DSCReport dscReport = new DSCReport();
                StatusData statusData = new StatusData();

                if (postData != null && postData.Length > 0)
                {
                    // Extract data from main report
                    try
                    {
                        string postDataStr = System.Text.Encoding.ASCII.GetString(postData);
                        JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                        dscReport = json_serializer.Deserialize<DSCReport>(postDataStr);
                    }
                    catch
                    {
                        dscReport = null;
                    }

                    // Extract data from StatusData part of main report
                    if (dscReport != null && dscReport.StatusData.Length > 0)
                    {
                        try
                        {
                            string statusDataStr = dscReport.StatusData[0];
                            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                            statusData = json_serializer.Deserialize<StatusData>(statusDataStr);
                        }
                        catch
                        {
                            statusData = null;
                        }
                    }
                    else
                    {
                        statusData = null;
                    }

                    // Write report on disk
                    if (dscReport.Status != null || ReportInProgress)
                    {
                        if (Directory.Exists(ReportsFolder))
                        {
                            var fileName = $"{agentId}.json";
                            // compute file name based on AgentId
                            var fileFullPath = Path.Combine(ReportsFolder, fileName);

                            try
                            {
                                // write to file request content
                                using (var fileWriter = new FileStream(fileFullPath, FileMode.Create))
                                {
                                    using (var binaryWriter = new BinaryWriter(fileWriter))
                                    {
                                        binaryWriter.Write(postData);
                                        binaryWriter.Flush();
                                        binaryWriter.Close();
                                    }

                                }
                            }
                            catch
                            {
                                // TODO if needed
                            }
                        }
                    }

                    // Write new log line in log file
                    Log log = new Log(dscReport, statusData);
                    log.Flush();
                }
            }
        }

        public void Dispose() { }
    }
}