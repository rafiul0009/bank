using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PayBill.API.Endpoint.Controllers.V1;

public partial class EncryptionDecryptionController
{
    private string _messages = string.Empty;

    private async Task<IActionResult> TryCatch(ReturningFunction returningFunction)
    {
        try
        {
            return await returningFunction();
        }
        catch (Exception ex)
        {
            if (returningFunction.Method.Name.Contains("Encrypt"))
                _messages = ex.Message;

            if (returningFunction.Method.Name.Contains("Decrypt"))
                _messages = ex.Message;

            return StatusCode(StatusCodes.Status500InternalServerError, _messages);
        }
    }

    private delegate Task<IActionResult> ReturningFunction();
}