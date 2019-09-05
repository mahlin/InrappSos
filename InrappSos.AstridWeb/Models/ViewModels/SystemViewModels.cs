using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.DomainModel;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class SystemViewModels
    {
        public class SystemViewModel
        {
            public IEnumerable<AdmFAQKategori> FAQCategories { get; set; }

            public IEnumerable<FAQViewModel> FAQs { get; set; }

            public int SelectedFAQCategory { get; set; }

            public string SelectedFAQCategoryName { get; set; }

            public int SelectedFAQId { get; set; }

            public string SelectedInfo { get; set; }
            [AllowHtml]
            public string SelectedInfoText { get; set; }
            public int SelectedInfoId { get; set; }
            public string SelectedTemplateFile { get; set; }

            public int SelectedDocumentId { get; set; }
            public int SelectedHolidayId { get; set; }

            public int SelectedSpecialDayId { get; set; }

            public FAQViewModel SelectedFAQ { get; set; }

            public IEnumerable<AdmInformation> InfoPages { get; set; }
            //public IEnumerable<AdmDokument> Mallar { get; set; }

            public IEnumerable<FileInfoViewModel> Mallar { get; set; }

            public IEnumerable<AdmHelgdagViewModel> Holidays { get; set; }

            public IEnumerable<AdmSpecialdagViewModel> SpecialDays { get; set; }

            public  AdmKonfiguration AdmConfig { get; set; }

            public OpeningHoursInfoDTO OpeningHours { get; set; }

        }

        public class FileInfoViewModel
        {
            public string Filename { get; set; }
            public DateTime LastWriteTime { get; set; }
            public DateTime CreationTime { get; set; }
            public long Length { get; set; }

        }

        public class FAQCategoryViewModel
        {
            public string Kategori{ get; set; }
            public int? Sortering { get; set; }

        }

        public class FAQViewModel
        {
            public int Id { get; set; }
            public int? RegisterId { get; set; }
            public int? SelectedRegisterId { get; set; }
            public string RegisterKortNamn { get; set; }
            public int FAQkategoriId { get; set; }
            public string Fraga { get; set; }
            [AllowHtml]
            public string Svar { get; set; }
            public int? Sortering { get; set; }

        }



        public class InfoTextViewModel
        {
            public int Id { get; set; }
            public string Informationstyp { get; set; }
            [AllowHtml]
            public string Text { get; set; }
        }

        public class AdmHelgdagViewModel
        {
            public int Id { get; set; }
            public string Informationstyp { get; set; }
            public int SelectedInformationId { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Helgdatum { get; set; }
            public string Helgdag { get; set; }

        }

        public class AdmSpecialdagViewModel
        {
            public int Id { get; set; }
            public string Informationstyp { get; set; }
            public int SelectedInformationId { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Specialdagdatum { get; set; }
            [DisplayName("Öppna")]
            public TimeSpan Oppna { get; set; }
            [RegularExpression("^(?:[01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Felaktigt format.")]
            public string OppnaStr { get; set; }
            [DisplayName("Stäng")]
            public TimeSpan Stang { get; set; }
            public string Anledning { get; set; }
           
        }

        public class OpeningHours
        {
            public int ClosedFromHour { get; set; }

            [DataType(DataType.Time)]
            [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime OpeningTime { get; set; }

            [RegularExpression("^(?:[01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Felaktigt format.")]
            public string OpeningTimeStr { get; set; }

            [DataType(DataType.Time)]
            [DisplayFormat(DataFormatString = "{0:HH:mm}")]
            public DateTime ClosingTime { get; set; }

            [RegularExpression("^(?:[01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Felaktigt format.")]
            public string ClosingTimeStr { get; set; }

            public int ClosedFromMin { get; set; }
            public int ClosedToHour { get; set; }
            public int ClosedToMin { get; set; }
            [DisplayName("Stäng portalen")]
            public bool ClosedAnyway { get; set; }
            public List<OpeningDay> ClosedDaysList { get; set; }
            [AllowHtml]
            public string InfoTextForClosedPage { get; set; }

        }

   
    }
}