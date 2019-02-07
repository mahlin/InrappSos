using System;


namespace InrappSos.DomainModel
{
    public class UndantagForvantadfil
    {
        public int Id { get; set; }
        public int OrganisationsId { get; set; }
        public int DelregisterId { get; set; }
        public int ForvantadfilId { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual AdmDelregister AdmDelregister { get; set; }
        public virtual AdmForvantadfil AdmForvantadfil { get; set; }
    }
}
