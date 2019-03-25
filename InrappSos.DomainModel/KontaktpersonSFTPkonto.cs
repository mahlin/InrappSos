using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.DomainModel
{
    public class KontaktpersonSFTPkonto
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int SFTPkontoId { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual SFTPkonto SFTPkonto { get; set; }
    }
}
