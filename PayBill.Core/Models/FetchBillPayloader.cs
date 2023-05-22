namespace PayBill.Core.Models;

public class FetchBillPayloader
{
    public string BillerId { get; set; } = string.Empty;
    public string BillType { get; set; } = string.Empty;
    public string BillNo { get; set; } = string.Empty;
    public string MeterNo { get; set; } = string.Empty;
    public string BillPeriod { get; set; } = string.Empty;
    public string CustomerAccountNo { get; set; } = string.Empty;
    public string CustomerMobileNo { get; set; } = string.Empty;
    public string LastPayDate { get; set; } = string.Empty;
    public string ExchangeCode => "88";
    public string Mode => "SAPI";
}