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
    
    public partial class PRELEGERI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRELEGERI()
        {
            this.LINK_PRELEGERI_GRUPE = new HashSet<LINK_PRELEGERI_GRUPE>();
            this.LINK_PROFESORI_PRELEGERI = new HashSet<LINK_PROFESORI_PRELEGERI>();
        }
    
        public int ID_PRELEGERE { get; set; }
        public string DENUMIRE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LINK_PRELEGERI_GRUPE> LINK_PRELEGERI_GRUPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LINK_PROFESORI_PRELEGERI> LINK_PROFESORI_PRELEGERI { get; set; }
    }
}
