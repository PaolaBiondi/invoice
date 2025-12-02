using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eInvoice.Tests
{
    internal class FileNameAttribute: Attribute
    {
        public string FileName { get; }
        public FileNameAttribute(string fileName)
        {
            FileName = fileName;
        }
    }
}
