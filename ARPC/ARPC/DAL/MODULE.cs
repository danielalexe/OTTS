//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ARPC.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class MODULE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MODULE()
        {
            this.BLOCKED_MODULES = new HashSet<BLOCKED_MODULES>();
            this.LINK_MODULE_GRUPE = new HashSet<LINK_MODULE_GRUPE>();
            this.PREFERINTE_PROFESORI_MODULE = new HashSet<PREFERINTE_PROFESORI_MODULE>();
        }
    
        public int ID_MODUL { get; set; }
        public string DENUMIRE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BLOCKED_MODULES> BLOCKED_MODULES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LINK_MODULE_GRUPE> LINK_MODULE_GRUPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PREFERINTE_PROFESORI_MODULE> PREFERINTE_PROFESORI_MODULE { get; set; }
    }
}
