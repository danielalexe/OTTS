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
    
    public partial class GROUPS_LECTURES_LINK
    {
        public int iID_GROUPS_LECTURES_LINK { get; set; }
        public int iID_GROUP { get; set; }
        public int iID_LECTURE { get; set; }
        public bool bACTIVE { get; set; }
        public System.DateTime dtCREATE_DATE { get; set; }
        public Nullable<System.DateTime> dtLASTMODIFIED_DATE { get; set; }
        public int iCREATE_USER { get; set; }
        public Nullable<int> iLASTMODIFIED_USER { get; set; }
    
        public virtual GROUPS GROUPS { get; set; }
        public virtual LECTURES LECTURES { get; set; }
    }
}