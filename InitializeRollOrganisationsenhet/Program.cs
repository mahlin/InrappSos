using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InrappSos.ApplicationService.Helpers;


namespace InitializeRollOrganisationsenhet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Startar initiering av RollOrganisationsenhet");
            var manager = new Initiator();
            try
            {
                manager.Initiate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("InitializeRollOrganisationsenhet", "Initiate", e.ToString(), e.HResult, "InitializeRollOrganisationsenhet");
            }

            Console.WriteLine("Initiering av RollOrganisationsenhet klar");
            //För test
            Console.ReadLine();
        }
    }
}
