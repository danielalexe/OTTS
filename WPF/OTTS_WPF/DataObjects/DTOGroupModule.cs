using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class DTOGroupModule
    {
        public int iID_GROUPS_MODULES_LINK { get; set; }
        public int iID_MODULE { get; set; }
        public int iID_GROUP { get; set; }
        public string MODULE_NAME { get; set; }
        public int iID_GENERATOR_PRIORITY { get; set; }
    }
}
