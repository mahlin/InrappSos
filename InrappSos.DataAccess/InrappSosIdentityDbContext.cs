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
    public class InrappSosIdentityDbContext : IdentityDbContext<AppUserAdmin>
    {
        public InrappSosIdentityDbContext() : base("name=IdentityConnection")
        {
#if DEBUG
            Database.Log = s => Debug.WriteLine(s);
#endif
            Configuration.ProxyCreationEnabled = false;
        }

        public InrappSosIdentityDbContext(string connString) : base(connString)
        {
#if DEBUG
            Database.Log = s => Debug.WriteLine(s);
#endif
            Configuration.ProxyCreationEnabled = false;
        }

        public static InrappSosIdentityDbContext Create()
        {
            return new InrappSosIdentityDbContext();
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
            //modelBuilder.Entity<AppUserAdmin>().ToTable("AspNetUsers");
            modelBuilder.Entity<AppUserAdmin>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<AppUserAdmin>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<AppUserAdmin>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<AppUserAdmin>().Property(e => e.AndradAv).HasColumnName("andradav");
        }

        //public DbSet<AppUserAdmin> AppUserAdmin { get; set; }


    }
}