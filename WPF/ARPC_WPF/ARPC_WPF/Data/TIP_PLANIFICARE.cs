//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ARPC_WPF.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class TIP_PLANIFICARE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIP_PLANIFICARE()
        {
            this.PLANIFICARE_ORAR = new HashSet<PLANIFICARE_ORAR>();
        }
    
        public int ID_TIP_PLANIFICARE { get; set; }
        public string DENUMIRE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLANIFICARE_ORAR> PLANIFICARE_ORAR { get; set; }
    }
}
