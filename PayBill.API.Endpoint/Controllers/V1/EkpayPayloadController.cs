using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayBill.Core.Models;
using PayBill.Core.Repositories;
using System.Threading.Tasks;

namespace PayBill.API.Endpoint.Controllers.V1;

[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public partial class EkpayPayloadController : ControllerBase
{
    private readonly IEkpayBillPaymentRepository _repository;

    public EkpayPayloadController(IEkpayBillPaymentRepository repository) => _repository = repository;

    [HttpPost("GetFetchBillPayload")]
    public Task<IActionResult> GetFetchBillPayload([FromBody] FetchBillPayloader model) =>
        TryCatch(() =>
        {
            var response = _repository.GetFetchBillPayload(model);
            return Task.FromResult<IActionResult>(Ok(response));
        });

    [HttpPost("GetPayBillPayload")]
    public Task<IActionResult> GetPayBillPayload([FromBody] PayBillPayloader model) =>
        TryCatch(() =>
        {
            var response = _repository.GetPayBillPayload(model); return Task.FromResult<IActionResult>(Ok(response));
        });
}