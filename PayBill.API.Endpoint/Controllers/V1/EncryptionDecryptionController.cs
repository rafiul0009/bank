using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayBill.Core.Models;
using PayBill.Core.Services;

namespace PayBill.API.Endpoint.Controllers.V1;

[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public partial class EncryptionDecryptionController : ControllerBase
{
    private readonly IEncryptionDecryptionService _service;

    public EncryptionDecryptionController(IEncryptionDecryptionService service) => _service = service;

    [HttpPost("Encrypt")]
    public Task<IActionResult> Encrypt([FromBody] EncryptionDecryptionModel model) =>
        TryCatch(async () =>
        {
            var response = Ok(await _service.Encrypt(model));
            return Ok(response);
        });

    [HttpPost("Decrypt")]
    public Task<IActionResult> Decrypt([FromBody] EncryptionDecryptionModel model) =>
        TryCatch(async () =>
        {
            var response = Ok(await _service.Decrypt(model));
            return Ok(response);
        });
}