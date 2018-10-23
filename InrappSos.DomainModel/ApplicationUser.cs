using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.DomainModel
{
    public class ApplicationUser :IdentityUser
    {
        public int OrganisationId { get; set; }
        public string Namn { get; set; }
        //public string Email { get; set; }
        public string Kontaktnummer { get; set; }
        //public string PhoneNumber { get; set; }
        //public bool PhoneNumberConfirmed { get; set; }
        public DateTime? AktivFrom { get; set; }
        public DateTime? AktivTom { get; set; }
        public int? Status { get; set; }
        public DateTime SkapadDatum { get; set; }
        public string SkapadAv { get; set; }
        public DateTime AndradDatum { get; set; }
        public string AndradAv { get; set; }
        public virtual ICollection<Leverans> Leveranser { get; set; }
        public virtual ICollection<Inloggning> Inloggningar { get; set; }
        public virtual ICollection<Kontaktpersonstyp> Kontaktpersontyp { get; set; }
        public virtual ICollection<ArendeKontaktperson> ArendeKontaktperson { get; set; }
        public virtual ICollection<DroppadFil> DroppadFil { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

    }
}