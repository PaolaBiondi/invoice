using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eInvoice.Jobs
{
    public class JobsSettings
    {
        public long RefreshInterval { get; set; } = 5;
        public ushort PurgeAfterMonths { get; set; } = 1;
    }
}
