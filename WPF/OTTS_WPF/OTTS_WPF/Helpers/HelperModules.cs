using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTTS_WPF.Helpers
{
    public class HelperModules
    {
        public static string GetModuleInterval(string nvMODULE_NAME)
        {
            switch (nvMODULE_NAME)
            {
                case "M1":
                    return "8:00 - 9:40";
                case "M2":
                    return "9:50 - 11:30";
                case "M3":
                    return "11:40 - 13:20";
                case "M4":
                    return "13:30 - 15:10";
                case "M5":
                    return "15:20 - 17:00";
                case "M6":
                    return "17:10 - 18:50";
                case "M7":
                    return "19:00 - 20:40";
                default:
                    return "";
            }
        }
    }
}
