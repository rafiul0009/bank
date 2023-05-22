namespace PayBill.Core.Models;

public class PayBillPayloader : FetchBillPayloader
{
    public string ReferenceNo { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string TotalAmount { get; set; } = string.Empty;
    public string EkpayFee { get; set; } = string.Empty;
}