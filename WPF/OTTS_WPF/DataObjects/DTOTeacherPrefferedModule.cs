using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class DTOTeacherPrefferedModule
    {
        public int iID_MODULE { get; set; }
        public string MODULE_NAME { get; set; }
        public string PRIORITY_BACHELOR { get; set; }
        public string PRIORITY_MASTERS { get; set; }
    }
}
