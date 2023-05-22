using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PayBill.Core.Models;
using PayBill.Core.Repositories;

namespace PayBill.API.Endpoint.Controllers.V1;

[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public partial class BillPaymentController : ControllerBase
{
    private readonly IEkpayBillPaymentRepository _repository;
    private readonly string _fetchBillUrl;
    private readonly string _payBillUrl;

    public BillPaymentController(IConfiguration config, IEkpayBillPaymentRepository repository)
    {
        _repository = repository;
        _fetchBillUrl = config["Ekpay:FetchBillUrl"]!;
        _payBillUrl = config["Ekpay:PayBillUrl"]!;
    }

    [HttpPost("GetToken")]
    public Task<IActionResult> GetToken() =>
        TryCatch(async () =>
        {
            var response = await _repository.GetTokenAsync();
            return Ok(response);
        });

    [HttpPost("FetchBillInfo")]
    public Task<IActionResult> FetchBillInfo([FromBody] EkpayRequestData model) =>
        TryCatch(async () =>
        {
            var response = await _repository.ExecuteRequestAsync(model, _fetchBillUrl);
            return Ok(response);
        });

    [HttpPost("PayBill")]
    public Task<IActionResult> PayBill([FromBody] EkpayRequestData model) =>
        TryCatch(async () =>
        {
            var response = await _repository.ExecuteRequestAsync(model, _payBillUrl);
            return Ok(response);
        });
}