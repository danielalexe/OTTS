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
    
    public partial class SEMIGRUPE
    {
        public int ID_SEMIGRUPA { get; set; }
        public string DENUMIRE { get; set; }
        public int ID_GRUPA { get; set; }
        public int PRIORITATE { get; set; }
    
        public virtual GRUPE GRUPE { get; set; }
    }
}
