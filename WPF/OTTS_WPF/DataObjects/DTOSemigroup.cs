using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class DTOSemigroup
    {
        public int iID_SEMIGROUP { get; set; }
        public string SEMIGROUP_NAME { get; set; }
        public int iID_GROUP { get; set; }
        public string GROUP_NAME { get; set; }
        public int PRIORITY { get; set; }
        public string nvCOMBO_DISPLAY { get; set; }
    }
}
