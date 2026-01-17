using Enums;
using Presentation_Layer.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using System.Security.Claims;
using Presentation_Layer.Extensions;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("items")]
[CheckPermission(Permission.Carts_ManageCart)]
public class CartItemsController : ControllerBase
{
    public CartItemBusiness CartItemBusiness { get; }

    public CartItemsController(CartItemBusiness cartItemBusiness)
    {
        CartItemBusiness = cartItemBusiness;
    }


    [HttpPost("{productId}")]
    public async Task<ActionResult> InsertCartItem(int productId)
    {
        return await CartItemBusiness.InsertCartItem(productId, User.GetUserId()) ?
            Ok() : BadRequest("Something went wrong");
    }



    [HttpPatch("plus/{cartItemId}")]
    public async Task<ActionResult> PlusOneCartItem(int cartItemId)
    {
        return await CartItemBusiness.PlusOneCartItem(cartItemId, User.GetUserId()) ?
            Ok() : BadRequest("Something went wrong");
    }

    [HttpPatch("minus/{cartItemId}")]
    public async Task<ActionResult> MinusOneCartItem(int cartItemId)
    {
        return await CartItemBusiness.MinusOneCartItem(cartItemId, User.GetUserId()) ?
            Ok() : BadRequest("Something went wrong");
    }



    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCartItem(int id)
    {
        return await CartItemBusiness.DeleteCartItem(id, User.GetUserId()) ?
            Ok() : BadRequest("Something went wrong");
    }



    [HttpPatch]
    public async Task<ActionResult> UsePromoCode([FromQuery] int cartItemid, [FromQuery] string promocode)
    {
        return await CartItemBusiness.UsePromocode(cartItemid, promocode, User.GetUserId()) ?
            Ok() : BadRequest("Add promo code failed");
    }
}
