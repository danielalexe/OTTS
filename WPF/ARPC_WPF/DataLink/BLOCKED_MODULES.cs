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
    
    public partial class BLOCKED_MODULES
    {
        public int iID_BLOCKED_MODULE { get; set; }
        public int iID_TEACHER { get; set; }
        public int iID_MODULE { get; set; }
        public int iID_DAY { get; set; }
        public Nullable<bool> bACTIVE { get; set; }
        public Nullable<System.DateTime> dtCREATE_DATE { get; set; }
        public Nullable<System.DateTime> dtLASTMODIFIED_DATE { get; set; }
        public Nullable<int> iCREATE_USER { get; set; }
        public Nullable<int> iLASTMODIFIED_USER { get; set; }
    
        public virtual MODULES MODULES { get; set; }
        public virtual TEACHERS TEACHERS { get; set; }
        public virtual DAYS DAYS { get; set; }
    }
}
