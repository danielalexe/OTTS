using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class DTOTeachersLectures
    {
        public int iID_TEACHERS_LECTURES_LINK { get; set; }
        public int iID_TEACHER { get; set; }
        public string TEACHER_NAME { get; set; }
        public int iID_LECTURE { get; set; }
        public string LECTURE_NAME { get; set; }
        public int iID_LECTURE_TYPE { get; set; }
        public string LECTURE_TYPE { get; set; }
        public int HOURS { get; set; }
        public int? iID_MAXIMUM_ALLOCATION_VALUE { get; set; }
        public string MAXIMUM_ALLOCATION { get; set; }
    }
}
