namespace PayBill.Core.Models;

public class EkpayRequestData
{
    public string Token { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}