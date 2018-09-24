using System.Collections.Generic;
using InrappSos.DomainModel;

namespace InrappSos.FilipWeb.Models.ViewModels
{
    public class AboutViewModel
    {
        public IEnumerable<AdmFAQKategori> FaqCategories { get; set; }
        public bool PortalClosed { get; set; }
    }
}