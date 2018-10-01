using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace InrappSos.AstridWeb.Helpers
{
    public sealed class FileLogWriter
    {
        private string errorLogFilePath = "C:\\logs\\InrappSosAstrid\\";
        private string errorLogFileName = "error.txt";
        private string time = DateTime.Now.ToString("yyyy-MM-dd");
        private string errorLogFile = String.Empty;


        public FileLogWriter()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("errorLogFilePath"))
            {
                errorLogFilePath = ConfigurationManager.AppSettings["errorLogFilePath"];
            }

            if (ConfigurationManager.AppSettings.AllKeys.Contains("errorLogFileName"))
            {
                errorLogFileName = ConfigurationManager.AppSettings["errorLogFileName"];
            }

            errorLogFile = errorLogFilePath + time + "_" + errorLogFileName;

            //Check if path and file exists. Create otherwise
            //if (!string.IsNullOrWhiteSpace(errorLogFilePath) && !Directory.Exists(errorLogFilePath)) Directory.CreateDirectory(errorLogFilePath);
            if (!string.IsNullOrWhiteSpace(errorLogFile) && !File.Exists(errorLogFile))
            {
                var errorfile = File.Create(errorLogFile);
                errorfile.Close();
            }
        }

        private void WriteLogToFile(string logMessage , string logFile)
        {
            //using (StreamWriter resource_0 = new StreamWriter(logFile, true, Encoding.Unicode))
            //    resource_0.WriteLine(logMessage);

            using (StreamWriter resource_0 = new StreamWriter(logFile, true, Encoding.Unicode))
            {
                resource_0.WriteLine(logMessage);
            }
        }

        public void WriteExceptionLog(string logMessage)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   " + logMessage);
            this.WriteLogToFile(stringBuilder.ToString(), this.errorLogFile);
        }



    }
}