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
    
    public partial class SALI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SALI()
        {
            this.PREFERINTE_PROFESORI_SALI = new HashSet<PREFERINTE_PROFESORI_SALI>();
        }
    
        public int ID_SALA { get; set; }
        public string DENUMIRE { get; set; }
        public int CAPACITATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PREFERINTE_PROFESORI_SALI> PREFERINTE_PROFESORI_SALI { get; set; }
    }
}