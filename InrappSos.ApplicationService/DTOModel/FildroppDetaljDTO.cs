using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InrappSos.DomainModel;

namespace InrappSos.ApplicationService.DTOModel
{
    public class FildroppDetaljDTO
    {
        public int Id { get; set; }
        public int ArendeId { get; set; }
        public string Arendenummer { get; set; }
        public string Arendenamn { get; set; }
        public string Kontaktperson { get; set; }
        public string Filnamn { get; set; }
        public string NyttFilnamn { get; set; }
        public DateTime AndradDatum { get; set; }

    }
}