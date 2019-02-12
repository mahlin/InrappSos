using System;


namespace InrappSos.ApplicationService.DTOModel
{
    public class UndantagForvantadfilDTO
    {
        public int Id { get; set; }
        public int OrganisationsId { get; set; }
        public int DelregisterId { get; set; }
        public int ForvantadfilId { get; set; }
        public bool Selected { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }

    }
}
