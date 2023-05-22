using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PayBill.API.Endpoint.Controllers.V1;

public partial class BillPaymentController
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
            if (returningFunction.Method.Name.Contains("GetToken"))
                _messages = ex.Message;

            if (returningFunction.Method.Name.Contains("FetchBillInfo"))
                _messages = ex.Message;

            if (returningFunction.Method.Name.Contains("PayBill"))
                _messages = ex.Message;

            return StatusCode(StatusCodes.Status500InternalServerError, _messages);
        }
    }

    private delegate Task<IActionResult> ReturningFunction();
}