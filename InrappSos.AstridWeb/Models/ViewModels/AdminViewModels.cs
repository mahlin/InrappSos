using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.AstridWeb.Models.ViewModels
{
    public class AdminViewModels
    {
        public class AdminViewModel
        {
            public List<AppUserAdminViewModel> AdminUsers { get; set; }
            public string SelectedUser { get; set; }
            public List<IdentityRoleViewModel> Roller { get; set; }
            public List<Arendetyp> CaseTypes { get; set; }
            public List<ArendeAnsvarig> CaseManagers { get; set; }
        }

        public class AppUserAdminViewModel
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
            public string StringOfRoles { get; set; }
            public List<IdentityRoleViewModel> ListOfRoles { get; set; }
            public IList<string> Roles { get; set; }
            public DateTime SkapadDatum { get; set; }
            public string SkapadAv { get; set; }
            public DateTime AndradDatum { get; set; }
            public string AndradAv { get; set; }
        }

        public class AdminRoleViewModel
        {
            public ApplicationRole ApplicationRole{ get; set; }
            public string SelectedApplication { get; set; }

        }

        public class ArendetypViewModel
        {
            public int Arendetypid { get; set; }
            public string ArendetypNamn { get; set; }
            public string Slussmapp { get; set; }
            [Display(Name = "Kontaktpersoners epostadress")]
            public string KontaktpersonerStr { get; set; }
            public bool Selected { get; set; } = false;
        }

        public class ArendeAnsvarigViewModel
        {
            public int Id { get; set; }
            public string Epostadress { get; set; }
        }
    }
}