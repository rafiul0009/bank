using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PayBill.Core.Models;
using Polly;

namespace PayBill.Core.Repositories;

public class EkpayBillPaymentRepository : IEkpayBillPaymentRepository
{
    private readonly HttpClient _client = new();
    private readonly EkpayRequestData _request = new();
    private readonly string _nodeId;
    private readonly string _passKey;
    private readonly string _syndicateId;
    private readonly string _tokenUrl;
    private readonly string _userId;

    public EkpayBillPaymentRepository(IConfiguration config)
    {
        _client.Timeout = TimeSpan.FromSeconds(30);
        _userId = config["Ekpay:UserId"]!;
        _passKey = config["Ekpay:PassKey"]!;
        _nodeId = config["Ekpay:NodeId"]!;
        _syndicateId = config["Ekpay:SyndicateId"]!;
        _tokenUrl = config["Ekpay:TokenUrl"]!;
    }

    #region "Get Token"

    public async Task<string> GetTokenAsync()
    {
        var payload = new
        {
            user_id = _userId,
            pass_key = _passKey
        };

        _request.Payload = JsonConvert.SerializeObject(payload, Formatting.Indented);
        return await ExecuteRequestAsync(_request, _tokenUrl);
    }

    #endregion

    #region "Get Fetch Bill Payload"

    public string GetFetchBillPayload(FetchBillPayloader model)
    {
        var datetime = DateTime.UtcNow.AddHours(6).ToString("yyyy-MM-ddTHH:mm:ss+06:00");
        var referenceId = RandomString(20) + DateTime.UtcNow.AddHours(6).ToString("yyyyMMddHHmm");
        var transactionId = RandomString(20) + DateTime.UtcNow.AddHours(6).ToString("yyyyMMddHHmm");

        var payload = new
        {
            hdrs = new
            {
                nm = "FETCH_BLL_REQ",
                ver = "v1.3.0",
                tms = datetime,
                ref_id = referenceId,
                nd_id = _nodeId
            },
            trx = new
            {
                trx_id = transactionId,
                trx_tms = datetime
            },
            bll_inf = new
            {
                bllr_id = model.BillerId,
                bll_no = model.BillNo,
                bll_period = model.BillPeriod,
                meter_no = model.MeterNo,
                bllr_accno = model.CustomerAccountNo,
                bill_mobno = model.CustomerMobileNo,
                bll_typ = model.BillType,
                xchng_code = model.ExchangeCode,
                last_pay_dt = model.LastPayDate,
                mode = model.Mode
            },
            usr_inf = new
            {
                syndct_id = _syndicateId
            }
        };

        var jsonPayload = JsonConvert.SerializeObject(payload, Formatting.Indented);
        var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonPayload));

        return JsonConvert.SerializeObject(new
        {
            Payload = payload,
            EncodedPayload = encodedPayload
        }, Formatting.Indented);
    }

    #endregion

    #region "Get Pay Bill Payload"

    public string GetPayBillPayload(PayBillPayloader model)
    {
        var datetime = DateTime.UtcNow.AddHours(6).ToString("yyyy-MM-ddTHH:mm:ss+06:00");
        var transactionId = RandomString(20) + DateTime.UtcNow.AddHours(6).ToString("yyyyMMddHHmm");

        var payload = new
        {
            hdrs = new
            {
                nm = "UPDT_BLL_PYMNT_REQ",
                ver = "v1.3.0",
                tms = datetime,
                ref_id = model.ReferenceId,
                nd_id = _nodeId
            },
            trx = new
            {
                trx_id = transactionId,
                trx_tms = datetime,
                refno_ack = model.ReferenceNo
            },
            bllr_inf = new
            {
                bllr_id = model.BillerId,
                bll_typ = model.BillType,
                ekpay_fee = model.EkpayFee,
                bll_amnt_ttl = model.TotalAmount,
                mode = "SAPI"
            },
            pyd_inf = new
            {
                pyd_trxn_refid = model.TransactionId,
                pyd_tms = datetime,
                pyd_amnt = model.TotalAmount
            },
            usr_inf = new
            {
                syndct_id = _syndicateId
            }
        };

        var jsonPayload = JsonConvert.SerializeObject(payload, Formatting.Indented);
        var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonPayload));

        return JsonConvert.SerializeObject(new
        {
            Payload = payload,
            EncodedPayload = encodedPayload
        }, Formatting.Indented);
    }

    #endregion

    #region "HTTP Client Services"

    public async Task<string> ExecuteRequestAsync(EkpayRequestData model, string requestUrl)
    {
        if (!string.IsNullOrEmpty(model.Token))
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", model.Token);

        // Define the retry policy
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(2, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        // Execute the API call with the retry policy
        return await retryPolicy.ExecuteAsync(async () =>
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
            {
                request.Content = new StringContent(model.Payload, Encoding.UTF8, "application/json");
                using (var response = await _client.SendAsync(request).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }).ConfigureAwait(false);
    }

    #endregion

    #region "Helper Methods"

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

    #endregion
}