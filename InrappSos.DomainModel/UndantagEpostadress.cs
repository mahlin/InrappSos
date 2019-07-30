using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class UndantagEpostadress
    {
        public int Id { get; set; }
        public int OrganisationsId { get; set; }
        public string PrivatEpostAdress { get; set; }
        public int Status { get; set; }
        public DateTime? AktivFrom { get; set; }
        public DateTime? AktivTom { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
