using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class FileUploadViewModels
    {
        public class FileUploadViewModel
        {
            public string SelectedTemplateFile { get; set; }

            public IEnumerable<FileInfoViewModel> Mallar { get; set; }
        }


        public class FileInfoViewModel
        {
            public string Filename { get; set; }
            public DateTime LastWriteTime { get; set; }
            public DateTime CreationTime { get; set; }
            public long Length { get; set; }
        }
    }


   
}