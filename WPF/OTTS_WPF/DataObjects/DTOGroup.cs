using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class DTOGroup
    {
        public int iID_GROUP { get; set; }
        public string NAME { get; set; }
        public int NUMBER_OF_STUDENTS { get; set; }
        public int iID_GROUP_TYPE { get; set; }
        public string GROUP_TYPE { get; set; }
        public int YEAR { get; set; }

        public string nvCOMBO_DISPLAY { get; set; }
    }
}
