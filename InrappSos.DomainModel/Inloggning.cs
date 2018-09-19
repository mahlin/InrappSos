using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.DomainModel
{
    public class Inloggning
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}