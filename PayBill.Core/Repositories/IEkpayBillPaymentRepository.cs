using PayBill.Core.Models;

namespace PayBill.Core.Repositories;

public interface IEkpayBillPaymentRepository
{
    Task<string> GetTokenAsync();
    string GetFetchBillPayload(FetchBillPayloader model);
    string GetPayBillPayload(PayBillPayloader model);
    Task<string> ExecuteRequestAsync(EkpayRequestData model, string requestUrl);
}