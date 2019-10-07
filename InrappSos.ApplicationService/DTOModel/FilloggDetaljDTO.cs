using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InrappSos.DomainModel;

namespace InrappSos.ApplicationService.DTOModel
{
    public class FilloggDetaljDTO
    {
        public int Id { get; set; }
        public int LeveransId { get; set; }
        public string Enhetskod { get; set; }
        public int OrganisationsenhetsId { get; set; }
        public string RegisterKortnamn { get; set; }
        public string Filnamn { get; set; }
        public string Period { get; set; }
        public DateTime Leveranstidpunkt { get; set; }
        public string Leveransstatus { get; set; }
        public string Kontaktperson { get; set; }
        public string Filstatus { get; set; }
        public string Resultatfil { get; set; }
        public bool Pagaende { get; set; }
        public bool Sen { get; set; }
        public string SFTPkontoNamn { get; set; }
        public string Aterkopplingskontakt { get; set; }


        internal static FilloggDetaljDTO FromFillogg(LevereradFil fillogg)
        {

            if (fillogg == null)
                return null;

            return new FilloggDetaljDTO
            {
                Id = fillogg.Id,
                LeveransId = fillogg.LeveransId,
                Filnamn = fillogg.Filnamn,
                Filstatus = fillogg.Filstatus
            };
        }
    }
}