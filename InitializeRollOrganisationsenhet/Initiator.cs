using System;
using System.Configuration;
using System.Linq;
using System.IO;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;

namespace InitializeRollOrganisationsenhet
{
    public class Initiator
    {
        private readonly IPortalSosService _portalSosService;
        private string _dbLogPath = String.Empty;
        private string _dbLogFileName = "DbUpdateLog.txt";
        private static readonly object _mailLogLock = new object();
        private string time = DateTime.Now.ToString("yyyy-MM-dd");


        public Initiator()
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));
            if (ConfigurationManager.AppSettings.AllKeys.Contains("DbLogPath"))
            {
                _dbLogPath = ConfigurationManager.AppSettings["DbLogPath"];
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("DbLogFileName"))
            {
                _dbLogFileName = ConfigurationManager.AppSettings["DbLogFileName"];
            }
            _dbLogPath = _dbLogPath + time + "_" + _dbLogFileName;

        }

        public void Initiate()
        {
            //Hämta alla organisationer
            var orgList = _portalSosService.HamtaAllaOrganisationer().ToList();
            var antOrg = orgList.Count();
            var antNyaRollOrganisationsenhet = 0;
            foreach (var org in orgList)
            {
                //Hämta organisationens kontakpersoner
                var contacts = _portalSosService.HamtaKontaktpersonerForOrg(org.Id);

                foreach (var contact in contacts)
                {
                    //Hämta varje kontakts valda delregister
                    var userId = _portalSosService.HamtaAnvandarId(contact);
                    var userName = _portalSosService.HamtaAnvandaresNamn(userId);
                    var rollList = _portalSosService.HamtaValdaDelregisterRollForAnvandare(userId);

                    foreach (var roll in rollList)
                    {
                        //Hämta aktiv uppgiftsskyldighet för valt delregister
                        var uppgskh = _portalSosService.HamtaAktivUppgiftsskyldighetForOrganisationOchRegister(org.Id,roll.DelregisterId);
                        if (uppgskh != null)
                        {
                            if (uppgskh.RapporterarPerEnhet)
                            {
                                //Hämta aktiva enhetsuppgiftsskyldigheter
                                var enhetsuppgskhList = _portalSosService.HamtaAktivEnhetsUppgiftsskyldighetForUppgiftsskyldighet(uppgskh.Id);
                                foreach (var enhetsuppgskh in enhetsuppgskhList)
                                {
                                    var exists = _portalSosService.HamtaRollOrgenhet(roll.Id,enhetsuppgskh.OrganisationsenhetsId);
                                    if (exists == null)
                                    {
                                        var rollOrgUnit = new RollOrganisationsenhet
                                        {
                                            RollId = roll.Id,
                                            OrganisationsenhetsId = enhetsuppgskh.OrganisationsenhetsId,
                                            SkapadDatum = DateTime.Now,
                                            SkapadAv = "InitializeRollOrganisationsenhet"
                                        };
                                        _portalSosService.SparaRollOrganisationsenhet(rollOrgUnit);
                                        Console.WriteLine("Sparar RollOrganisationsenhet för orgid: " + org.Id + ". Kontaktperson: " + userName);
                                        Log("Sparar RollOrganisationsenhet för orgid: " + org.Id + ". Kontaktperson: " + userName);
                                        antNyaRollOrganisationsenhet++;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            Console.WriteLine("Antal kontrollerade organisationer: " + antOrg);
            Log("Antal kontrollerade organisationer: " + antOrg);
            Console.WriteLine("Antal nya RollOrganisationshet: " + antNyaRollOrganisationsenhet);
            Log("Antal nya RollOrganisationshet: " + antNyaRollOrganisationsenhet);
        }

        private void Log(string message)
        {
            lock (_mailLogLock)
            {
                try
                {
                    File.AppendAllText(_dbLogPath, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + " " + message + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Couldn't log '" + message + "' to file log '" + _dbLogPath + "'" + Environment.NewLine + ex.ToString());
                }
            }
        }

    }
}
