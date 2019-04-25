using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InrappSos.ApplicationService.Helpers;

namespace ErrorLogNotifier
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Send email");
            var manager = new NotificationManager();
            try
            {
                manager.Notify();
            }
            catch (System.Net.Mail.SmtpException e)
            {
                Console.WriteLine(e);
                //throw new ArgumentException(e.Message);
                ErrorManager.WriteToErrorLog("ErrorLogNotifier", "SendErrorLogEmail", e.ToString(), e.HResult, "ErrorLogNotifier(Robin)");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("ErrorLogNotifier", "SendErrorLogEmail", e.ToString(), e.HResult, "ErrorLogNotifier(Robin)");
            }


            //För test
            Console.ReadLine();
        }
    }
}
