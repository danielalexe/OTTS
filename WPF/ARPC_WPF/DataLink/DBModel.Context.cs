﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLink
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ARPCContext : DbContext
    {
        public ARPCContext()
            : base("name=ARPCContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BLOCKED_MODULES> BLOCKED_MODULES { get; set; }
        public virtual DbSet<GRUPE> GRUPE { get; set; }
        public virtual DbSet<LINK_MODULE_GRUPE> LINK_MODULE_GRUPE { get; set; }
        public virtual DbSet<LINK_PRELEGERI_GRUPE> LINK_PRELEGERI_GRUPE { get; set; }
        public virtual DbSet<LINK_PROFESORI_PRELEGERI> LINK_PROFESORI_PRELEGERI { get; set; }
        public virtual DbSet<MODULE> MODULE { get; set; }
        public virtual DbSet<PLANIFICARE_ORAR> PLANIFICARE_ORAR { get; set; }
        public virtual DbSet<PREFERINTE_PROFESORI_MODULE> PREFERINTE_PROFESORI_MODULE { get; set; }
        public virtual DbSet<PREFERINTE_PROFESORI_SALI> PREFERINTE_PROFESORI_SALI { get; set; }
        public virtual DbSet<PREFERINTE_PROFESORI_ZILE> PREFERINTE_PROFESORI_ZILE { get; set; }
        public virtual DbSet<PRELEGERI> PRELEGERI { get; set; }
        public virtual DbSet<PROFESORI> PROFESORI { get; set; }
        public virtual DbSet<SALI> SALI { get; set; }
        public virtual DbSet<SEMIGRUPE> SEMIGRUPE { get; set; }
        public virtual DbSet<SETARI> SETARI { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TIP_EXECUTIE> TIP_EXECUTIE { get; set; }
        public virtual DbSet<TIP_GRUPA> TIP_GRUPA { get; set; }
        public virtual DbSet<TIP_PLANIFICARE> TIP_PLANIFICARE { get; set; }
        public virtual DbSet<ZILE> ZILE { get; set; }
    }
}
