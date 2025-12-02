namespace eInvoice.domain.Common
{
    /// <summary>
    /// Qualifies InvoiceAdded.SendingStatus column
    /// </summary>
    public enum SendingStatusQualifier : ushort
    {
        NotReady = 0,
        NotSent = 1,
        Sent = 2, // inv_UpdateElectronicInvoice who knows what this means => SendingStatus = CASE WHEN SendingStatus < '4' THEN '2' ELSE '5' END
        /// <summary>
        ///    if (selectedRow.Cells[4].Value.ToString() == "1") copyIndicator = "false";
        ///    else if (selectedRow.Cells[4].Value.ToString() == "4") copyIndicator = "true";
        ///    else copyIndicator = "";
        /// </summary>
        JosipNumber= 4 // legacy code says it is used to indicate copyIndicator ??
    }
}
