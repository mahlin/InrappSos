using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class RegisterForvantadfil
    {
        public int Id { get; set; }
        public int FilkravId { get; set; }
        public int? ForeskriftsId { get; set; }
        public string Filmask { get; set; }
        public string Regexp { get; set; }
        public bool Obligatorisk { get; set; }
        public bool Tom { get; set; }
    }
}
