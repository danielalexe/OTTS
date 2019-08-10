using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLink
{
    public partial class OTTSContext:DbContext
    {
        public OTTSContext(string connString)
            : base(connString)
        {

        }
    }
}
