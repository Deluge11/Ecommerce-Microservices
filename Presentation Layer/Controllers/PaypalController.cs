using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Enums;
using Presentation_Layer.Authorization;
using Business_Layer.Business;
using Presentation_Layer.Extensions;




namespace Presentation_Layer.Controllers;

[ApiController]
[Route("[controller]")]
public class PayPalController : ControllerBase
{
    public ILogger<PayPalController> logger { get; }
    public PayPalBusiness PayPalBusiness { get; }

    public PayPalController(ILogger<PayPalController> logger, PayPalBusiness payPalBusiness)
    {
        this.logger = logger;
        PayPalBusiness = payPalBusiness;
    }



    [CheckPermission(Permission.Paypal_MakePayPalPayment)]
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder()
    {
        int userId = User.GetUserId();
        var result = await PayPalBusiness.CreateOrder(userId);

        if (result.Success)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.ErrorMessage);
    }



    [CheckPermission(Permission.Paypal_MakePayPalPayment)]
    [HttpGet("confirm-payment")]
    public async Task<IActionResult> ConfirmPayment([FromQuery] string token)
    {
        return await PayPalBusiness.ConfirmPayment(token) ?
            Ok() : BadRequest();
    }



    [CheckPermission(Permission.Paypal_MakePayPalPayment)]
    [HttpGet("payment-cancelled")]
    public async Task<IActionResult> CancelPayment([FromQuery] string token)
    {
        return await PayPalBusiness.CancelPayment(token) ?
            Ok() : BadRequest();
    }



    [HttpPost("webhook")]
    public async Task<IActionResult> ReceiveWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var body = JsonDocument.Parse(await reader.ReadToEndAsync());
        var headers = HttpContext.Request.Headers;

        await PayPalBusiness.Webhook(body, headers);
        return Ok();
    }

}

