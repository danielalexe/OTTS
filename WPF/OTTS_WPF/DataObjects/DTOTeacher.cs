using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class DTOTeacher
    {
        public int iID_TEACHER { get; set; }
        public string SURNAME { get; set; }
        public string NAME { get; set; }
        public int BACHELOR_PRIORITY { get; set; }
        public int MASTERS_PRIORITY { get; set; }
        public string nvCOMBO_DISPLAY { get; set; }
    }
}
