//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class PLANNING_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PLANNING_TYPE()
        {
            this.TIMETABLE_PLANNING = new HashSet<TIMETABLE_PLANNING>();
        }
    
        public int iID_PLANNING_TYPE { get; set; }
        public string nvNAME { get; set; }
        public Nullable<bool> bACTIVE { get; set; }
        public Nullable<System.DateTime> dtCREATE_DATE { get; set; }
        public Nullable<System.DateTime> dtLASTMODIFIED_DATE { get; set; }
        public Nullable<int> iCREATE_USER { get; set; }
        public Nullable<int> iLASTMODIFIED_USER { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIMETABLE_PLANNING> TIMETABLE_PLANNING { get; set; }
    }
}