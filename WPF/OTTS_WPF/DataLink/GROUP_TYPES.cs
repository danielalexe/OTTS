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
    
    public partial class GROUP_TYPES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GROUP_TYPES()
        {
            this.GROUPS = new HashSet<GROUPS>();
            this.TEACHERS_GROUP_TYPES_PRIORITY = new HashSet<TEACHERS_GROUP_TYPES_PRIORITY>();
        }
    
        public int iID_GROUP_TYPE { get; set; }
        public string nvNAME { get; set; }
        public bool bACTIVE { get; set; }
        public System.DateTime dtCREATE_DATE { get; set; }
        public Nullable<System.DateTime> dtLASTMODIFIED_DATE { get; set; }
        public int iCREATE_USER { get; set; }
        public Nullable<int> iLASTMODIFIED_USER { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUPS> GROUPS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEACHERS_GROUP_TYPES_PRIORITY> TEACHERS_GROUP_TYPES_PRIORITY { get; set; }
    }
}
