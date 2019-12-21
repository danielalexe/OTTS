using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class DTOPlanningExport
    {
        public List<DTOPlanningRow> lLIST_EXPORT { get; set; }
        public int iID_GROUP { get; set; }
        public int iID_SEMIGROUP { get; set; }
        public int iID_GROUP_TYPE { get; set; }
        public int iYEAR { get; set; }
        public string nvSEMIGROUP_NAME { get; set; }
    }
}
