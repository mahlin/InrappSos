﻿using System;
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

            //ApplicationRoleAstrid
            modelBuilder.Entity<ApplicationRoleAstrid>().HasKey<string>(r => r.Id).ToTable("AspNetRoles");
            modelBuilder.Entity<ApplicationRoleAstrid>().Property(e => e.BeskrivandeNamn).HasColumnName("beskrivandenamn");
            modelBuilder.Entity<ApplicationRoleAstrid>().Property(e => e.Beskrivning).HasColumnName("beskrivning");
            modelBuilder.Entity<ApplicationRoleAstrid>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<ApplicationRoleAstrid>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<ApplicationRoleAstrid>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<ApplicationRoleAstrid>().Property(e => e.AndradAv).HasColumnName("andradav");
            modelBuilder.Entity<AppUserAdmin>().HasMany<ApplicationUserRoleAstrid>((AppUserAdmin u) => u.UserRoles);
            modelBuilder.Entity<ApplicationRoleAstrid>().HasMany<ApplicationUserRoleAstrid>((ApplicationRoleAstrid u) => u.UserRoles);

            //ApplicationUserRoleAstrid
            modelBuilder.Entity<ApplicationUserRoleAstrid>().HasKey(r => new { UserId = r.UserId, RoleId = r.RoleId }).ToTable("AspNetUserRoles");
            modelBuilder.Entity<ApplicationUserRoleAstrid>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            modelBuilder.Entity<ApplicationUserRoleAstrid>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            modelBuilder.Entity<ApplicationUserRoleAstrid>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            modelBuilder.Entity<ApplicationUserRoleAstrid>().Property(e => e.AndradAv).HasColumnName("andradav");



            //ApplicationRoles
            //modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles");
            //modelBuilder.Entity<ApplicationRole>().Property(e => e.SkapadDatum).HasColumnName("skapaddatum");
            //modelBuilder.Entity<ApplicationRole>().Property(e => e.SkapadAv).HasColumnName("skapadav");
            //modelBuilder.Entity<ApplicationRole>().Property(e => e.AndradDatum).HasColumnName("andraddatum");
            //modelBuilder.Entity<ApplicationRole>().Property(e => e.AndradAv).HasColumnName("andradav");
        }

        public DbSet<ApplicationUserRoleAstrid> ApplicationUserRoleAstrid { get; set; }
        public DbSet<ApplicationRoleAstrid> ApplicationRoleAstrid { get; set; }

    }
}