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
    
    public partial class OTTSContext : DbContext
    {
        public OTTSContext()
            : base("name=OTTSContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BLOCKED_MODULES> BLOCKED_MODULES { get; set; }
        public virtual DbSet<DAYS> DAYS { get; set; }
        public virtual DbSet<GROUP_TYPES> GROUP_TYPES { get; set; }
        public virtual DbSet<GROUPS> GROUPS { get; set; }
        public virtual DbSet<GROUPS_LECTURES_LINK> GROUPS_LECTURES_LINK { get; set; }
        public virtual DbSet<GROUPS_MODULES_LINK> GROUPS_MODULES_LINK { get; set; }
        public virtual DbSet<HALLS> HALLS { get; set; }
        public virtual DbSet<LECTURE_TYPE> LECTURE_TYPE { get; set; }
        public virtual DbSet<LECTURES> LECTURES { get; set; }
        public virtual DbSet<MODULES> MODULES { get; set; }
        public virtual DbSet<PLANNING_TYPE> PLANNING_TYPE { get; set; }
        public virtual DbSet<SEMESTERS> SEMESTERS { get; set; }
        public virtual DbSet<SEMIGROUPS> SEMIGROUPS { get; set; }
        public virtual DbSet<SETTINGS> SETTINGS { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TEACHER_PREFERRED_DAYS> TEACHER_PREFERRED_DAYS { get; set; }
        public virtual DbSet<TEACHER_PREFERRED_HALLS> TEACHER_PREFERRED_HALLS { get; set; }
        public virtual DbSet<TEACHER_PREFERRED_MODULES> TEACHER_PREFERRED_MODULES { get; set; }
        public virtual DbSet<TEACHERS> TEACHERS { get; set; }
        public virtual DbSet<TEACHERS_GROUP_TYPES_PRIORITY> TEACHERS_GROUP_TYPES_PRIORITY { get; set; }
        public virtual DbSet<TEACHERS_LECTURES_LINK> TEACHERS_LECTURES_LINK { get; set; }
        public virtual DbSet<TIMETABLE_PLANNING> TIMETABLE_PLANNING { get; set; }
    }
}
