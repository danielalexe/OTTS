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
    
    public partial class PLANIFICARE_ORAR
    {
        public int ID_PLANIFICARE_ORAR { get; set; }
        public int ID_ZI { get; set; }
        public int ID_MODUL { get; set; }
        public int ID_SEMIGRUPA { get; set; }
        public int ID_PRELEGERE { get; set; }
        public int ID_PROFESOR { get; set; }
        public int ID_TIP_EXECUTIE { get; set; }
        public int NUMAR_GENERARE { get; set; }
        public int ID_TIP_PLANIFICARE { get; set; }
    
        public virtual TIP_PLANIFICARE TIP_PLANIFICARE { get; set; }
    }
}