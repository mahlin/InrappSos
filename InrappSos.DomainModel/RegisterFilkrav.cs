using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class RegisterFilkrav
    {
        public int Id { get; set; }
        public string Namn { get; set; }
        public string InfoText { get; set; }
        public int AntalFiler { get; set; }
        public int AntalObligatoriskaFiler { get; set; }
        public int AntalEjObligatoriskaFiler { get; set; }
        public IEnumerable<string> Perioder { get; set; }
        public IEnumerable<RegisterForvantadfil> ForvantadeFiler { get; set; }
        //public bool Obligatorisk { get; set; } = true;
        //public IEnumerable<string> FilMasker { get; set; }
        //public IEnumerable<string> RegExper { get; set; }
    }
}
