using Presentation_Layer.Authorization;
using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Presentation_Layer.Extensions;

namespace Presentation_Layer.Controllers;

[CheckPermission(Permission.Promocodes_ManagePromocode)]
[ApiController]
[Route("[controller]")]
public class PromocodeController : ControllerBase
{

    public PromoCodeBusiness PromoCodeBusiness { get; }

    public PromocodeController(PromoCodeBusiness promoCodeBusiness)
    {
        PromoCodeBusiness = promoCodeBusiness;
    }


    [HttpPost]
    public async Task<IActionResult> AddPromoCode(AddPromocode promoCode)
    {
        var result = await PromoCodeBusiness.AddPromoCode(promoCode, User.GetUserId());
        return result.Success ?
            Ok() : BadRequest(result.ErrorMessage);
    }



    [HttpGet]
    public async Task<IActionResult> GetMyPromoCodes()
    {
        var result = await PromoCodeBusiness.GetPromoCodes(User.GetUserId());
        return result == null || result.Count < 1 ?
            BadRequest() : Ok(result);
    }



    [HttpPatch("{id}")]
    public async Task<IActionResult> TogglePromoCode(int id)
    {
        return await PromoCodeBusiness.TogglePromocode(id, User.GetUserId()) ?
            Ok() : BadRequest();
    }

}
