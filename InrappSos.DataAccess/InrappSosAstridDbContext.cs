using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Web;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InrappSos.DataAccess
{
    public class InrappSosAstridDbContext : IdentityDbContext<AppUserAdmin>
    {
        
        public InrappSosAstridDbContext() : base("name=IdentityConnection")
        {
#if DEBUG
            Database.Log = s => Debug.WriteLine(s);
#endif
            Configuration.ProxyCreationEnabled = false;
        }

        public InrappSosAstridDbContext(string connString) : base(connString)
        {
#if DEBUG
            Database.Log = s => Debug.WriteLine(s);
#endif
            Configuration.ProxyCreationEnabled = false;
        }

        public static InrappSosAstridDbContext Create()
        {
            return new InrappSosAstridDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            MapEntities(modelBuilder);
        }

        private void MapEntities(DbModelBuilder modelBuilder)
        {
            //AspNetUsers
            modelBuilder.Entity<AppUserAdmin>().ToTable("AspNetUsers");
            modelBuilder.Entity<AppUserAdmin>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AppUserAdmin>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AppUserAdmin>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AppUserAdmin>().Property(e => e.AndradAv).HasColumnName("andradav");

            //ApplicationRole
            modelBuilder.Entity<ApplicationRole>().HasKey<string>(r => r.Id).ToTable("AspNetRoles");
            modelBuilder.Entity<ApplicationRole>().Property(e => e.BeskrivandeNamn).HasColumnName("beskrivandenamn");
            modelBuilder.Entity<ApplicationRole>().Property(e => e.Beskrivning).HasColumnName("beskrivning");
            modelBuilder.Entity<ApplicationRole>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<ApplicationRole>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<ApplicationRole>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<ApplicationRole>().Property(e => e.AndradAv).HasColumnName("andradav");
            modelBuilder.Entity<AppUserAdmin>().HasMany<ApplicationUserRole>((AppUserAdmin u) => u.UserRoles);
            modelBuilder.Entity<ApplicationRole>().HasMany<ApplicationUserRole>((ApplicationRole u) => u.UserRoles);

            //ApplicationUserRole
            modelBuilder.Entity<ApplicationUserRole>().HasKey(r => new { UserId = r.UserId, RoleId = r.RoleId }).ToTable("AspNetUserRoles");
            modelBuilder.Entity<ApplicationUserRole>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<ApplicationUserRole>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<ApplicationUserRole>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<ApplicationUserRole>().Property(e => e.AndradAv).HasColumnName("andradav");

        }

        public DbSet<ApplicationUserRole> ApplicationUserRole{ get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }

    }
}