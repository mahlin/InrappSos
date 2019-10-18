using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;

namespace InitializeRollOrganisationsenhet
{
    public class Initiator
    {
        private readonly IPortalSosService _portalSosService;

        public Initiator()
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
        }

        public void Initiate()
        {
            //Hämta alla organisationer
            var orgList = _portalSosService.HamtaAllaOrganisationer();
            foreach (var org in orgList)
            {
                //Hämta organisationens kontakpersoner
                var contacts = _portalSosService.HamtaKontaktpersonerForOrg(org.Id);

                foreach (var contact in contacts)
                {
                    //Hämta varje kontakts valda delregister
                    var userId = _portalSosService.HamtaAnvandarId(contact);
                    var rollList = _portalSosService.HamtaValdaDelregisterRollForAnvandare(userId);

                    foreach (var roll in rollList)
                    {
                        //Hämta aktiv uppgiftsskyldighet för valt delregister
                        var uppgskh = _portalSosService.HamtaAktivUppgiftsskyldighetForOrganisationOchRegister(org.Id,roll.DelregisterId);
                        if (uppgskh.RapporterarPerEnhet)
                        {
                            //Hämta aktiva enhetsuppgiftsskyldigheter
                            var enhetsuppgskhList = _portalSosService.HamtaAktivEnhetsUppgiftsskyldighetForUppgiftsskyldighet(uppgskh.Id);
                            foreach (var enhetsuppgskh in enhetsuppgskhList)
                            {
                                var rollOrgUnit = new RollOrganisationsenhet
                                {
                                    RollId = roll.Id,
                                    OrganisationsenhetsId = enhetsuppgskh.OrganisationsenhetsId,
                                    SkapadDatum = DateTime.Now,
                                    SkapadAv = "InitializeRollOrganisationsenhet"
                                };
                                _portalSosService.SparaRollOrganisationsenhet(rollOrgUnit);
                            }
                        }

                    }

                }
            }
        }

    }
}
