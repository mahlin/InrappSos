using System;
using System.Configuration;

namespace InrappSos.ApplicationService.Helpers
{
    public class ErrorManager
    {
         #region Public Methods

        public static void WriteToErrorLog(string className, string methodName, string message, int errorcode = 0, string userName = "")
        {

            try
            {
                string errorMessage = "Username: " + userName + "\r\n";

                if (errorcode != 0)
                {
                    errorMessage = errorMessage + "Code: " + errorcode + "\r\n Message: " + message;
                }
                else
                {
                    errorMessage = errorMessage + "Message: " + message;
                }
                
                FileLogWriter _log = new FileLogWriter();
           
                _log.WriteExceptionLog("An exception occurred in " + className + ", " + methodName + ".\r\n " + errorMessage);
            }
            catch (Exception e)
            {
                //ToDO..
                Console.WriteLine(e);
                var t = e.ToString();
                throw;
            }
        }

        #endregion
    }
    
}