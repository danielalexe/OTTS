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
    
    public partial class LECTURES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LECTURES()
        {
            this.GROUPS_LECTURES_LINK = new HashSet<GROUPS_LECTURES_LINK>();
            this.TEACHERS_LECTURES_LINK = new HashSet<TEACHERS_LECTURES_LINK>();
        }
    
        public int iID_LECTURE { get; set; }
        public string nvNAME { get; set; }
        public int iID_SEMESTER { get; set; }
        public bool bACTIVE { get; set; }
        public System.DateTime dtCREATE_DATE { get; set; }
        public Nullable<System.DateTime> dtLASTMODIFIED_DATE { get; set; }
        public int iCREATE_USER { get; set; }
        public Nullable<int> iLASTMODIFIED_USER { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUPS_LECTURES_LINK> GROUPS_LECTURES_LINK { get; set; }
        public virtual SEMESTERS SEMESTERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEACHERS_LECTURES_LINK> TEACHERS_LECTURES_LINK { get; set; }
    }
}
