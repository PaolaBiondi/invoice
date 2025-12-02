using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace eInvoice.domain.Common
{
    public enum MimeCodeQualifier
    {
        PDF = 1,
        XLSX = 2,
        TXT = 3,
        PNG = 4, 
        JPEG = 5,
        SPREAD_SHEET = 6
    }

    public static class MimeCodeQualifierExtensions
    {
        public static string GetMimeType(this MimeCodeQualifier qualifier)
        {
            return qualifier switch
            {
                MimeCodeQualifier.PDF => "application/pdf",
                MimeCodeQualifier.XLSX => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                MimeCodeQualifier.TXT => " text/csv",
                MimeCodeQualifier.PNG => "image/png",
                MimeCodeQualifier.JPEG => "image/jpeg",
                MimeCodeQualifier.SPREAD_SHEET => "application/vnd.oasis.opendocument.spreadsheet",
                _ => throw new ArgumentOutOfRangeException(nameof(qualifier), qualifier, null)
            };
        }
    }
}
