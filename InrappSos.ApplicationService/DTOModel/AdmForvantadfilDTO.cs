using System;


namespace InrappSos.ApplicationService.DTOModel
{
    public class AdmForvantadfilDTO
    {
        public int Id { get; set; }
        public int FilkravId { get; set; }
        public int DelregisterId { get; set; }
        public int? ForeskriftsId { get; set; }
        public string Filmask { get; set; }
        public string Regexp { get; set; }
        public bool Obligatorisk { get; set; }
        public bool Tom { get; set; }

    }
}