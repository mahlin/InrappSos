using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class SearchViewModels
    {

        public class SearchViewModel
        {
            public List<List<SearchResult>> SearchResultList { get; set; }
        }

        public class SearchResult
        {
            public string Origin { get; set; }
            public string Name { get; set; }
            public string DomainModelName { get; set; }
            public int Id { get; set; }
            public string IdStr { get; set; }
        }
    }
}