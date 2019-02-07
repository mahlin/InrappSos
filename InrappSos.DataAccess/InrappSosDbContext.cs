using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Web;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.DataAccess
{
    public class InrappSosDbContext : IdentityDbContext<ApplicationUser>
    {
        public InrappSosDbContext() : base("name=DefaultConnection")
        {
#if DEBUG
            Database.Log = s => Debug.WriteLine(s);
#endif
            Configuration.ProxyCreationEnabled = false;
        }

        public InrappSosDbContext(string connString) : base(connString)
        {
#if DEBUG
            Database.Log = s => Debug.WriteLine(s);
#endif
            Configuration.ProxyCreationEnabled = false;
        }

        public static InrappSosDbContext Create()
        {
            return new InrappSosDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            MapEntities(modelBuilder);
        }

        private void MapEntities(DbModelBuilder modelBuilder)
        {
            //Organisation
            modelBuilder.Entity<Organisation>().Property(e => e.Id).HasColumnName("organisationsid");
            modelBuilder.Entity<Organisation>().Property(e => e.Landstingskod).HasColumnName("landstingskod");
            modelBuilder.Entity<Organisation>().Property(e => e.Kommunkod).HasColumnName("kommunkod");
            modelBuilder.Entity<Organisation>().Property(e => e.Inrapporteringskod).HasColumnName("inrapporteringskod");
            //modelBuilder.Entity<Organisation>().Property(e => e.Organisationstyp).HasColumnName("organisationstyp");
            modelBuilder.Entity<Organisation>().Property(e => e.Organisationsnr).HasColumnName("organisationsnr");
            modelBuilder.Entity<Organisation>().Property(e => e.Organisationsnamn).HasColumnName("organisationsnamn");
            modelBuilder.Entity<Organisation>().Property(e => e.Hemsida).HasColumnName("hemsida");
            modelBuilder.Entity<Organisation>().Property(e => e.EpostAdress).HasColumnName("epostadress");
            modelBuilder.Entity<Organisation>().Property(e => e.Telefonnr).HasColumnName("telefonnr");
            modelBuilder.Entity<Organisation>().Property(e => e.Adress).HasColumnName("adress");
            modelBuilder.Entity<Organisation>().Property(e => e.Postnr).HasColumnName("postnr");
            modelBuilder.Entity<Organisation>().Property(e => e.Postort).HasColumnName("postort");
            modelBuilder.Entity<Organisation>().Property(e => e.Epostdoman).HasColumnName("epostdoman");
            modelBuilder.Entity<Organisation>().Property(e => e.AktivFrom).HasColumnName("aktivfrom");
            modelBuilder.Entity<Organisation>().Property(e => e.AktivTom).HasColumnName("aktivtom");
            modelBuilder.Entity<Organisation>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Organisation>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<Organisation>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<Organisation>().Property(e => e.AndradAv).HasColumnName("andradav");

            //Organisationsenhet
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.Id).HasColumnName("organisationsenhetsid");
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.OrganisationsId).HasColumnName("organisationsid");
            modelBuilder.Entity<Organisationsenhet>()
                .HasRequired(c => c.Organisation)
                .WithMany(d => d.Organisationsenhet)
                .HasForeignKey(c => c.OrganisationsId);
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.Enhetsnamn).HasColumnName("enhetsnamn");
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.Enhetskod).HasColumnName("enhetskod");
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.AktivFrom).HasColumnName("aktivfrom");
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.AktivTom).HasColumnName("aktivtom");
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.Filkod).HasColumnName("filkod");
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<Organisationsenhet>().Property(e => e.AndradAv).HasColumnName("andradav");

            //ApplicationUser (kontaktperson)
            modelBuilder.Entity<ApplicationUser>().ToTable("Kontaktperson");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.OrganisationId).HasColumnName("organisationsid");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.Namn).HasColumnName("namn");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.Kontaktnummer).HasColumnName("kontaktnummer");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.AktivFrom).HasColumnName("aktivfrom");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.AktivTom).HasColumnName("aktivtom");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.AndradAv).HasColumnName("andradav");

            //Kontaktpersonstyp
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.Id).HasColumnName("id");
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.AdmKontaktpersonstypId).HasColumnName("admkontaktpersonstypid");
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.KontaktpersonId).HasColumnName("kontaktpersonid");
            modelBuilder.Entity<Kontaktpersonstyp>()
                .HasRequired(c => c.AdmKontaktpersonstyp)
                .WithMany(d => d.Kontaktpersonstyp)
                .HasForeignKey(c => c.AdmKontaktpersonstypId);
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmKontaktpersonstyp
            modelBuilder.Entity<AdmKontaktpersonstyp>().Property(e => e.Id).HasColumnName("admkontaktpersonstypid");
            modelBuilder.Entity<AdmKontaktpersonstyp>().Property(e => e.KontaktpersonstypStr).HasColumnName("Kontaktpersonstyp");
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<Kontaktpersonstyp>().Property(e => e.AndradAv).HasColumnName("andradav");

            //Inloggning
            modelBuilder.Entity<Inloggning>().Property(e => e.Id).HasColumnName("inloggningsid");
            modelBuilder.Entity<Inloggning>().Property(e => e.ApplicationUserId).HasColumnName("kontaktpersonid");
            modelBuilder.Entity<Inloggning>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Inloggning>().Property(e => e.SkapadAv).HasColumnName("skapadav");

            //Roll
            modelBuilder.Entity<Roll>().Property(e => e.Id).HasColumnName("rollid");
            modelBuilder.Entity<Roll>().Property(e => e.DelregisterId).HasColumnName("delregisterId");
            modelBuilder.Entity<Roll>().Property(e => e.ApplicationUserId).HasColumnName("kontaktpersonid");
            modelBuilder.Entity<Roll>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Roll>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<Roll>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<Roll>().Property(e => e.AndradAv).HasColumnName("andradav");

            //Leverans
            modelBuilder.Entity<Leverans>().Property(e => e.Id).HasColumnName("leveransid");
            modelBuilder.Entity<Leverans>().Property(e => e.OrganisationId).HasColumnName("organisationsid");
            modelBuilder.Entity<Leverans>().Property(e => e.OrganisationsenhetsId).HasColumnName("organisationsenhetsId");
            modelBuilder.Entity<Leverans>().Property(e => e.ApplicationUserId).HasColumnName("kontaktpersonid");
            modelBuilder.Entity<Leverans>().Property(e => e.DelregisterId).HasColumnName("delregisterId");
            modelBuilder.Entity<Leverans>().Property(e => e.ForvantadleveransId).HasColumnName("forvantadleveransId");
            modelBuilder.Entity<Leverans>().Property(e => e.Leveranstidpunkt).HasColumnName("leveranstidpunkt");
            modelBuilder.Entity<Leverans>().Property(e => e.Leveransstatus).HasColumnName("leveransstatus");
            modelBuilder.Entity<Leverans>()
                .HasRequired(c => c.AdmForvantadleverans)
                .WithMany(d => d.Leverans)
                .HasForeignKey(c => c.ForvantadleveransId);
            modelBuilder.Entity<Leverans>()
                .HasRequired(c => c.Organisation)
                .WithMany(d => d.Leveranser)
                .HasForeignKey(c => c.OrganisationId);
            modelBuilder.Entity<Leverans>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Leverans>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<Leverans>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<Leverans>().Property(e => e.AndradAv).HasColumnName("andradav");

            //Levereradfil
            modelBuilder.Entity<LevereradFil>().Property(e => e.Id).HasColumnName("filid");
            modelBuilder.Entity<LevereradFil>().Property(e => e.LeveransId).HasColumnName("leveransid");
            modelBuilder.Entity<LevereradFil>().Property(e => e.Filnamn).HasColumnName("filnamn");
            modelBuilder.Entity<LevereradFil>().Property(e => e.NyttFilnamn).HasColumnName("nyttfilnamn");
            modelBuilder.Entity<LevereradFil>().Property(e => e.Ordningsnr).HasColumnName("ordningsnr");
            modelBuilder.Entity<LevereradFil>().Property(e => e.Filstatus).HasColumnName("filstatus");
            modelBuilder.Entity<LevereradFil>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<LevereradFil>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<LevereradFil>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<LevereradFil>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmRegister
            modelBuilder.Entity<AdmRegister>().ToTable("admRegister");
            modelBuilder.Entity<AdmRegister>().Property(e => e.Id).HasColumnName("registerid");
            modelBuilder.Entity<AdmRegister>().Property(e => e.Registernamn).HasColumnName("registernamn");
            modelBuilder.Entity<AdmRegister>().Property(e => e.Beskrivning).HasColumnName("beskrivning");
            modelBuilder.Entity<AdmRegister>().Property(e => e.Kortnamn).HasColumnName("kortnamn");
            modelBuilder.Entity<AdmRegister>().Property(e => e.Inrapporteringsportal).HasColumnName("inrapporteringsportal");
            modelBuilder.Entity<AdmRegister>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmRegister>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmRegister>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmRegister>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmDelregister
            modelBuilder.Entity<AdmDelregister>().ToTable("admDelregister");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.Id).HasColumnName("delregisterid");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.RegisterId).HasColumnName("registerid");
            modelBuilder.Entity<AdmDelregister>()
                .HasRequired(c => c.AdmRegister)
                .WithMany(d => d.AdmDelregister)
                .HasForeignKey(c => c.RegisterId);
            modelBuilder.Entity<AdmDelregister>().Property(e => e.Delregisternamn).HasColumnName("delregisternamn");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.Beskrivning).HasColumnName("beskrivning");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.Kortnamn).HasColumnName("kortnamn");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.Slussmapp).HasColumnName("slussmapp");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.Inrapporteringsportal).HasColumnName("inrapporteringsportal");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmDelregister>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmUppgiftsskyldighet
            modelBuilder.Entity<AdmUppgiftsskyldighet>().ToTable("admUppgiftsskyldighet");
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.Id).HasColumnName("uppgiftsskyldighetid");
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.OrganisationId).HasColumnName("organisationsid");
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.DelregisterId).HasColumnName("delregisterid");
            modelBuilder.Entity<AdmUppgiftsskyldighet>()
                .HasRequired(c => c.AdmDelregister)
                .WithMany(d => d.AdmUppgiftsskyldighet)
                .HasForeignKey(c => c.DelregisterId);
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.SkyldigFrom).HasColumnName("skyldigfrom");
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.SkyldigTom).HasColumnName("skyldigtom");
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.RapporterarPerEnhet).HasColumnName("rapporterarperenhet");
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmUppgiftsskyldighet>().Property(e => e.AndradAv).HasColumnName("andradav");

            //UppgiftsskyldighetOrganisationstyp
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>().Property(e => e.Id).HasColumnName("uppgiftsskyldighetid");
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>().Property(e => e.DelregisterId).HasColumnName("delregisterid");
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>().Property(e => e.OrganisationstypId).HasColumnName("organisationstypId");
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>()
                .HasRequired(c => c.AdmDelregister)
                .WithMany(d => d.UppgiftsskyldighetOrganisationstyp)
                .HasForeignKey(c => c.DelregisterId);
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>().Property(e => e.SkyldigFrom).HasColumnName("skyldigfrom");
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>().Property(e => e.SkyldigTom).HasColumnName("skyldigtom");
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<UppgiftsskyldighetOrganisationstyp>().Property(e => e.AndradAv).HasColumnName("andradav");

            //Organisationstyp
            modelBuilder.Entity<Organisationstyp>().Property(e => e.Id).HasColumnName("id");
            modelBuilder.Entity<Organisationstyp>().Property(e => e.OrganisationsId).HasColumnName("organisationsid");
            modelBuilder.Entity<Organisationstyp>().Property(e => e.OrganisationstypId).HasColumnName("organisationstypid");
            modelBuilder.Entity<Organisationstyp>()
                .HasRequired(c => c.Organisation)
                .WithMany(d => d.Organisationstyp)
                .HasForeignKey(c => c.OrganisationsId);
            modelBuilder.Entity<Organisationstyp>()
                .HasRequired(c => c.AdmOrganisationstyp)
                .WithMany(d => d.Organisationstyp)
                .HasForeignKey(c => c.OrganisationstypId);
            modelBuilder.Entity<Organisationstyp>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Organisationstyp>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<Organisationstyp>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<Organisationstyp>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmOrganisationstyp
            modelBuilder.Entity<AdmOrganisationstyp>().Property(e => e.Id).HasColumnName("organisationstypid");
            modelBuilder.Entity<AdmOrganisationstyp>().Property(e => e.Typnamn).HasColumnName("typnamn");
            modelBuilder.Entity<AdmOrganisationstyp>().Property(e => e.Beskrivning).HasColumnName("beskrivning");
            modelBuilder.Entity<AdmOrganisationstyp>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmOrganisationstyp>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmOrganisationstyp>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmOrganisationstyp>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmEnhetsUppgiftsskyldighet
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>().ToTable("admEnhetsuppgiftsskyldighet");
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>().Property(e => e.Id).HasColumnName("enhetsuppgiftsskyldighetid");
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>().Property(e => e.OrganisationsenhetsId).HasColumnName("organisationsenhetsId");
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>()
                .HasRequired(c => c.Organisationsenhet)
                .WithMany(d => d.AdmEnhetsUppgiftsskyldighet)
                .HasForeignKey(c => c.OrganisationsenhetsId);
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>()
                .HasRequired(c => c.AdmUppgiftsskyldighet)
                .WithMany(d => d.AdmEnhetsUppgiftsskyldighet)
                .HasForeignKey(c => c.UppgiftsskyldighetId);
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>().Property(e => e.SkyldigFrom).HasColumnName("skyldigfrom");
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>().Property(e => e.SkyldigTom).HasColumnName("skyldigtom");
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmEnhetsUppgiftsskyldighet>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmInsamlingsfrekvens
            modelBuilder.Entity<AdmInsamlingsfrekvens>().ToTable("admInsamlingsfrekvens");
            modelBuilder.Entity<AdmInsamlingsfrekvens>().Property(e => e.Id).HasColumnName("insamlingsfrekvensid");
            modelBuilder.Entity<AdmInsamlingsfrekvens>().Property(e => e.Insamlingsfrekvens).HasColumnName("insamlingsfrekvens");
            modelBuilder.Entity<AdmInsamlingsfrekvens>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmInsamlingsfrekvens>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmInsamlingsfrekvens>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmInsamlingsfrekvens>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmFilkrav
            modelBuilder.Entity<AdmFilkrav>().ToTable("admFilkrav");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.Id).HasColumnName("filkravid");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.DelregisterId).HasColumnName("delregisterid");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.ForeskriftsId).HasColumnName("foreskriftsId");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.InsamlingsfrekvensId).HasColumnName("insamlingsfrekvensId");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.Namn).HasColumnName("namn");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.Uppgiftsstartdag).HasColumnName("uppgiftsstartdag");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.Uppgiftslutdag).HasColumnName("uppgiftslutdag");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.Rapporteringsstartdag).HasColumnName("rapporteringsstartdag");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.Rapporteringsslutdag).HasColumnName("rapporteringsslutdag");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.RapporteringSenastdag).HasColumnName("rapporteringsenastdag");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.Paminnelse1dag).HasColumnName("paminnelse1dag");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.Paminnelse2dag).HasColumnName("paminnelse2dag");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.Paminnelse3dag).HasColumnName("paminnelse3dag");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.RapporteringEfterAntalManader).HasColumnName("rapporteringefterantalmanader");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.UppgifterAntalmanader).HasColumnName("uppgifterantalmanader");
            modelBuilder.Entity<AdmFilkrav>()
                .HasRequired(c => c.AdmDelregister)
                .WithMany(d => d.AdmFilkrav)
                .HasForeignKey(c => c.DelregisterId);
            modelBuilder.Entity<AdmFilkrav>()
                .HasRequired(c => c.AdmForskrift)
                .WithMany(d => d.AdmFilkrav)
                .HasForeignKey(c => c.ForeskriftsId);
            modelBuilder.Entity<AdmFilkrav>()
                .HasRequired(c => c.AdmInsamlingsfrekvens)
                .WithMany(d => d.AdmFilkrav)
                .HasForeignKey(c => c.InsamlingsfrekvensId);
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmFilkrav>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmForvantadleverans
            modelBuilder.Entity<AdmForvantadleverans>().ToTable("admForvantadleverans");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.Id).HasColumnName("forvantadleveransid");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.FilkravId).HasColumnName("filkravId");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.DelregisterId).HasColumnName("delregisterid");
            modelBuilder.Entity<AdmForvantadleverans>()
                .HasRequired(c => c.AdmDelregister)
                .WithMany(d => d.AdmForvantadleverans)
                .HasForeignKey(c => c.DelregisterId);
            modelBuilder.Entity<AdmForvantadleverans>()
                .HasRequired(c => c.AdmFilkrav)
                .WithMany(d => d.AdmForvantadleverans)
                .HasForeignKey(c => c.FilkravId);
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.Rapporteringsstart)
                .HasColumnName("rapporteringsstart");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.Rapporteringsslut)
                .HasColumnName("rapporteringsslut");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.Rapporteringsenast)
                .HasColumnName("rapporteringsenast");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.Paminnelse1).HasColumnName("paminnelse1");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.Paminnelse2).HasColumnName("paminnelse2");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.Paminnelse3).HasColumnName("paminnelse3");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmForvantadleverans>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmForvantadfil
            modelBuilder.Entity<AdmForvantadfil>().ToTable("admForvantadfil");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.Id).HasColumnName("forvantadfilid");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.FilkravId).HasColumnName("filkravid");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.ForeskriftsId).HasColumnName("foreskriftsid");
            modelBuilder.Entity<AdmForvantadfil>()
                .HasRequired(c => c.AdmFilkrav)
                .WithMany(d => d.AdmForvantadfil)
                .HasForeignKey(c => c.FilkravId);
            modelBuilder.Entity<AdmForvantadfil>()
                .HasRequired(c => c.AdmForeskrift)
                .WithMany(d => d.AdmForvantadfil)
                .HasForeignKey(c => c.ForeskriftsId);
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.Filmask).HasColumnName("filmask");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.Regexp).HasColumnName("regexp");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.Obligatorisk).HasColumnName("obligatorisk");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.Tom).HasColumnName("tom");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmForvantadfil>().Property(e => e.AndradAv).HasColumnName("andradav");

            //Aterkoppling
            modelBuilder.Entity<Aterkoppling>().Property(e => e.Id).HasColumnName("aterkopplingsid");
            modelBuilder.Entity<Aterkoppling>().Property(e => e.LeveransId).HasColumnName("leveransId");
            modelBuilder.Entity<Aterkoppling>().Property(e => e.Leveransstatus).HasColumnName("leveransstatus");
            modelBuilder.Entity<Aterkoppling>().Property(e => e.Resultatfil).HasColumnName("resultatfil");
            modelBuilder.Entity<Aterkoppling>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Aterkoppling>().Property(e => e.SkapadAv).HasColumnName("skapadav");


            //AdmInformation
            modelBuilder.Entity<AdmInformation>().ToTable("admInformation");
            modelBuilder.Entity<AdmInformation>().Property(e => e.Id).HasColumnName("informationsid");
            modelBuilder.Entity<AdmInformation>().Property(e => e.Informationstyp).HasColumnName("informationstyp");
            modelBuilder.Entity<AdmInformation>().Property(e => e.Text).HasColumnName("text");
            modelBuilder.Entity<AdmInformation>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmInformation>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmInformation>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmInformation>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmHelgdag
            modelBuilder.Entity<AdmHelgdag>().ToTable("admHelgdag");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.Id).HasColumnName("helgdagid");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.InformationsId).HasColumnName("informationsid");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.Helgdatum).HasColumnName("helgdatum");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.Helgdag).HasColumnName("helgdag");
            modelBuilder.Entity<AdmHelgdag>()
                .HasRequired(c => c.AdmInformation)
                .WithMany(d => d.AdmHelgdag)
                .HasForeignKey(c => c.InformationsId);
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmSpecialdag
            modelBuilder.Entity<AdmSpecialdag>().ToTable("admSpecialdag");
            modelBuilder.Entity<AdmSpecialdag>().Property(e => e.Id).HasColumnName("specialdagid");
            modelBuilder.Entity<AdmSpecialdag>().Property(e => e.InformationsId).HasColumnName("informationsid");
            modelBuilder.Entity<AdmSpecialdag>().Property(e => e.Specialdagdatum).HasColumnName("specialdagdatum");
            modelBuilder.Entity<AdmSpecialdag>().Property(e => e.Oppna).HasColumnName("oppna");
            modelBuilder.Entity<AdmSpecialdag>().Property(e => e.Stang).HasColumnName("stang");
            modelBuilder.Entity<AdmSpecialdag>().Property(e => e.Anledning).HasColumnName("anledning");
            modelBuilder.Entity<AdmSpecialdag>()
                .HasRequired(c => c.AdmInformation)
                .WithMany(d => d.AdmSpecialdag)
                .HasForeignKey(c => c.InformationsId);
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmHelgdag>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmKonfiguration
            modelBuilder.Entity<AdmKonfiguration>().ToTable("admkonfiguration");
            modelBuilder.Entity<AdmKonfiguration>().Property(e => e.Id).HasColumnName("konfigurationsid");
            modelBuilder.Entity<AdmKonfiguration>().Property(e => e.Typ).HasColumnName("typ");
            modelBuilder.Entity<AdmKonfiguration>().Property(e => e.Varde).HasColumnName("varde");
            modelBuilder.Entity<AdmKonfiguration>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmKonfiguration>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmKonfiguration>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmKonfiguration>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmFAQKategori
            modelBuilder.Entity<AdmFAQKategori>().ToTable("admFAQKategori");
            modelBuilder.Entity<AdmFAQKategori>().Property(e => e.Id).HasColumnName("faqkategoriid");
            modelBuilder.Entity<AdmFAQKategori>().Property(e => e.Kategori).HasColumnName("kategori");
            modelBuilder.Entity<AdmFAQKategori>().Property(e => e.Sortering).HasColumnName("sortering");
            modelBuilder.Entity<AdmFAQKategori>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmFAQKategori>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmFAQKategori>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmFAQKategori>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmFAQ
            modelBuilder.Entity<AdmFAQ>().ToTable("admFAQ");
            modelBuilder.Entity<AdmFAQ>().Property(e => e.Id).HasColumnName("faqid");
            modelBuilder.Entity<AdmFAQ>().Property(e => e.RegisterId).HasColumnName("registerid");
            modelBuilder.Entity<AdmFAQ>().Property(e => e.FAQkategoriId).HasColumnName("faqkategoriid");
            modelBuilder.Entity<AdmFAQ>()
                .HasRequired(c => c.AdmFAQKategori)
                .WithMany(d => d.AdmFAQ)
                .HasForeignKey(c => c.FAQkategoriId);
            modelBuilder.Entity<AdmFAQ>().Property(e => e.Fraga).HasColumnName("fraga");
            modelBuilder.Entity<AdmFAQ>().Property(e => e.Svar).HasColumnName("svar");
            modelBuilder.Entity<AdmFAQ>().Property(e => e.Sortering).HasColumnName("sortering");
            modelBuilder.Entity<AdmFAQ>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmFAQ>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmFAQ>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmFAQ>().Property(e => e.AndradAv).HasColumnName("andradav");

            //AdmForeskrift
            modelBuilder.Entity<AdmForeskrift>().ToTable("admForeskrift");
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.Id).HasColumnName("foreskriftsid");
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.RegisterId).HasColumnName("registerid");
            modelBuilder.Entity<AdmForeskrift>()
                .HasRequired(c => c.AdmRegister)
                .WithMany(d => d.AdmForeskrift)
                .HasForeignKey(c => c.RegisterId);
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.Forfattningsnamn).HasColumnName("forfattningsnamn");
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.GiltigFrom).HasColumnName("giltigfrom");
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.GiltigTom).HasColumnName("giltigtom");
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.Beslutsdatum).HasColumnName("beslutsdatum");
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AdmForeskrift>().Property(e => e.AndradAv).HasColumnName("andradav");

            //Rapporteringsresultat (view_rapporteringsresultat)
            modelBuilder.Entity<Rapporteringsresultat>().ToTable("view_rapporteringsresultat");
            modelBuilder.Entity<Rapporteringsresultat>().Property(e => e.Lankod).HasColumnName("lankod");
            modelBuilder.Entity<Rapporteringsresultat>().Property(e => e.Kommunkod).HasColumnName("kommunkod");
            modelBuilder.Entity<Rapporteringsresultat>().Property(e => e.Organisationsnamn).HasColumnName("organisationsnamn");
            modelBuilder.Entity<Rapporteringsresultat>().Property(e => e.RegisterId).HasColumnName("registerid");
            modelBuilder.Entity<Rapporteringsresultat>().Property(e => e.Register).HasColumnName("register");
            modelBuilder.Entity<Rapporteringsresultat>().Property(e => e.Period).HasColumnName("period");
            modelBuilder.Entity<Rapporteringsresultat>().Property(e => e.RegisterKortnamn).HasColumnName("kortnamn");
            modelBuilder.Entity<Rapporteringsresultat>().Property(e => e.Enhetskod).HasColumnName("enhetskod");
            modelBuilder.Entity<Rapporteringsresultat>().Property(e => e.AntalLeveranser).HasColumnName("antalleveranser");

            //Arende
            modelBuilder.Entity<Arende>().Property(e => e.Id).HasColumnName("arendeid");
            modelBuilder.Entity<Arende>().Property(e => e.OrganisationsId).HasColumnName("organisationsid");
            modelBuilder.Entity<Arende>().Property(e => e.Arendenamn).HasColumnName("arendenamn");
            modelBuilder.Entity<Arende>().Property(e => e.Arendenr).HasColumnName("arendenr");
            modelBuilder.Entity<Arende>().Property(e => e.ArendetypId).HasColumnName("arendetypid");
            modelBuilder.Entity<Arende>().Property(e => e.ArendestatusId).HasColumnName("arendestatusid");
            modelBuilder.Entity<Arende>().Property(e => e.StartDatum).HasColumnName("startdatum");
            modelBuilder.Entity<Arende>().Property(e => e.SlutDatum).HasColumnName("slutdatum");
            modelBuilder.Entity<Arende>().Property(e => e.AnsvarigEpost).HasColumnName("ansvarigepost");
            modelBuilder.Entity<Arende>()
                .HasRequired(c => c.Organisation)
                .WithMany(d => d.Arende)
                .HasForeignKey(c => c.OrganisationsId);
            modelBuilder.Entity<Arende>()
                .HasRequired(c => c.ArendeStatus)
                .WithMany(d => d.Arende)
                .HasForeignKey(c => c.ArendestatusId);
            modelBuilder.Entity<Arende>()
                .HasRequired(c => c.Arendetyp)
                .WithMany(d => d.Arende)
                .HasForeignKey(c => c.ArendetypId);
            modelBuilder.Entity<Arende>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Arende>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<Arende>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<Arende>().Property(e => e.AndradAv).HasColumnName("andradav");

            //ArendeStatus
            modelBuilder.Entity<ArendeStatus>().Property(e => e.Id).HasColumnName("arendestatusid");
            modelBuilder.Entity<ArendeStatus>().Property(e => e.ArendeStatusNamn).HasColumnName("arendestatusnamn");
            modelBuilder.Entity<ArendeStatus>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<ArendeStatus>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<ArendeStatus>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<ArendeStatus>().Property(e => e.AndradAv).HasColumnName("andradav");

            //Arendetyp
            modelBuilder.Entity<Arendetyp>().Property(e => e.Id).HasColumnName("arendetypid");
            modelBuilder.Entity<Arendetyp>().Property(e => e.ArendetypNamn).HasColumnName("arendetypnamn");
            modelBuilder.Entity<Arendetyp>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<Arendetyp>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<Arendetyp>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<Arendetyp>().Property(e => e.AndradAv).HasColumnName("andradav");

            //ArendeKontaktperson
            modelBuilder.Entity<ArendeKontaktperson>().Property(e => e.Id).HasColumnName("arendekontaktpersonid");
            modelBuilder.Entity<ArendeKontaktperson>().Property(e => e.ArendeId).HasColumnName("arendeid");
            modelBuilder.Entity<ArendeKontaktperson>().Property(e => e.ApplicationUserId).HasColumnName("kontaktpersonid");
            modelBuilder.Entity<ArendeKontaktperson>()
                .HasRequired(c => c.ApplicationUser)
                .WithMany(d => d.ArendeKontaktperson)
                .HasForeignKey(c => c.ApplicationUserId);
            modelBuilder.Entity<ArendeKontaktperson>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<ArendeKontaktperson>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<ArendeKontaktperson>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<ArendeKontaktperson>().Property(e => e.AndradAv).HasColumnName("andradav");

            //DroppadFil
            modelBuilder.Entity<DroppadFil>().Property(e => e.Id).HasColumnName("filid");
            modelBuilder.Entity<DroppadFil>().Property(e => e.ArendeId).HasColumnName("arendeid");
            modelBuilder.Entity<DroppadFil>().Property(e => e.ApplicationUserId).HasColumnName("kontaktpersonid");

            modelBuilder.Entity<DroppadFil>().Property(e => e.Filnamn).HasColumnName("filnamn");
            modelBuilder.Entity<DroppadFil>().Property(e => e.NyttFilnamn).HasColumnName("nyttfilnamn");
            modelBuilder.Entity<DroppadFil>()
                .HasRequired(c => c.ApplicationUser)
                .WithMany(d => d.DroppadFil)
                .HasForeignKey(c => c.ApplicationUserId);
            modelBuilder.Entity<DroppadFil>()
                .HasRequired(c => c.Arende)
                .WithMany(d => d.DroppadFil)
                .HasForeignKey(c => c.ArendeId);
            modelBuilder.Entity<DroppadFil>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<DroppadFil>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<DroppadFil>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<DroppadFil>().Property(e => e.AndradAv).HasColumnName("andradav");

            //UndantagEpostadress
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.Id).HasColumnName("undantagsid");
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.OrganisationsId).HasColumnName("organisationsid");
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.ArendeId).HasColumnName("arendeid");
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.PrivatEpostAdress).HasColumnName("epostadress");
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.Status).HasColumnName("status");
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.AktivFrom).HasColumnName("aktivfrom");
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.AktivTom).HasColumnName("aktivtom");
            modelBuilder.Entity<UndantagEpostadress>()
                .HasRequired(c => c.Organisation)
                .WithMany(d => d.UndantagEpostadress)
                .HasForeignKey(c => c.OrganisationsId);
            modelBuilder.Entity<UndantagEpostadress>()
                .HasRequired(c => c.Arende)
                .WithMany(d => d.UndantagEpostadress)
                .HasForeignKey(c => c.ArendeId);
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<UndantagEpostadress>().Property(e => e.AndradAv).HasColumnName("andradav");


            //UndantagForvantadfil
            modelBuilder.Entity<UndantagForvantadfil>().Property(e => e.Id).HasColumnName("undantagforvantadfilid");
            modelBuilder.Entity<UndantagForvantadfil>().Property(e => e.OrganisationsId).HasColumnName("organisationsid");
            modelBuilder.Entity<UndantagForvantadfil>().Property(e => e.DelregisterId).HasColumnName("delregisterId");
            modelBuilder.Entity<UndantagForvantadfil>().Property(e => e.ForvantadfilId).HasColumnName("forvantadfilId");
            modelBuilder.Entity<UndantagForvantadfil>()
                .HasRequired(c => c.Organisation)
                .WithMany(d => d.UndantagForvantadfil)
                .HasForeignKey(c => c.OrganisationsId);
            modelBuilder.Entity<UndantagForvantadfil>()
                .HasRequired(c => c.AdmDelregister)
                .WithMany(d => d.UndantagForvantadfil)
                .HasForeignKey(c => c.DelregisterId);
            modelBuilder.Entity<UndantagForvantadfil>()
                .HasRequired(c => c.AdmForvantadfil)
                .WithMany(d => d.UndantagForvantadfil)
                .HasForeignKey(c => c.ForvantadfilId);
            modelBuilder.Entity<UndantagForvantadfil>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<UndantagForvantadfil>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<UndantagForvantadfil>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<UndantagForvantadfil>().Property(e => e.AndradAv).HasColumnName("andradav");

        }


        public DbSet<Organisation> Organisation { get; set; }
        public DbSet<Organisationsenhet> Organisationsenhet { get; set; }
        public DbSet<Leverans> Leverans { get; set; }
        public DbSet<LevereradFil> LevereradFil { get; set; }
        public DbSet<AdmRegister> AdmRegister { get; set; }
        public DbSet<AdmDelregister> AdmDelregister { get; set; }
        public DbSet<AdmInsamlingsfrekvens> AdmInsamlingsfrekvens { get; set; }
        public DbSet<AdmFilkrav> AdmFilkrav { get; set; }
        public DbSet<AdmForvantadfil> AdmForvantadfil { get; set; }
        public DbSet<AdmForvantadleverans> AdmForvantadleverans { get; set; }
        public DbSet<Aterkoppling> Aterkoppling { get; set; }
        public DbSet<AdmInformation> AdmInformation { get; set; }
        public DbSet<AdmUppgiftsskyldighet> AdmUppgiftsskyldighet { get; set; }
        public DbSet<AdmEnhetsUppgiftsskyldighet> AdmEnhetsUppgiftsskyldighet { get; set; }
        public DbSet<AdmFAQKategori> AdmFAQKategori { get; set; }
        public DbSet<AdmFAQ> AdmFAQ { get; set; }
        public DbSet<AdmKonfiguration> AdmKonfiguration { get; set; }
        public DbSet<AdmForeskrift> AdmForeskrift { get; set; }
        public DbSet<AdmHelgdag> AdmHelgdag { get; set; }
        public DbSet<AdmSpecialdag> AdmSpecialdag { get; set; }
        public DbSet<Inloggning> Inloggning { get; set; }
        public DbSet<Roll> Roll { get; set; }
        //public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Rapporteringsresultat> RapporteringsResultat { get; set; }
        public DbSet<UppgiftsskyldighetOrganisationstyp> UppgiftsskyldighetOrganisationstyp { get; set; }
        public DbSet<Organisationstyp> Organisationstyp { get; set; }
        public DbSet<AdmOrganisationstyp> AdmOrganisationstyp { get; set; }
        public DbSet<Kontaktpersonstyp> Kontaktpersonstyp { get; set; }
        public DbSet<AdmKontaktpersonstyp> AdmKontaktpersonstyp { get; set; }
        public DbSet<ArendeKontaktperson> ArendeKontaktperson { get; set; }
        public DbSet<DroppadFil> DroppadFil { get; set; }
        public DbSet<Arende> Arende { get; set; }
        public DbSet<ArendeStatus> ArendeStatus { get; set; }
        public DbSet<Arendetyp> Arendetyp { get; set; }
        public DbSet<UndantagEpostadress> UndantagEpostadress { get; set; }
        public DbSet<UndantagForvantadfil> UndantagForvantadfil { get; set; }

    }
}