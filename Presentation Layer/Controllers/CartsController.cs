using Enums;
using Presentation_Layer.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Business_Layer.Business;
using Presentation_Layer.Extensions;

namespace Presentation_Layer.Controllers;


[ApiController]
[Route("[controller]")]
[CheckPermission(Permission.Carts_ManageCart)]
public class CartsController : ControllerBase
{
    public CartsBusiness CartsBusiness { get; }

    public CartsController(CartsBusiness cartsBusiness)
    {
        CartsBusiness = cartsBusiness;
    }



    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCartItemsCount()
    {
        int userId = User.GetUserId();
        var count = await CartsBusiness.GetCartItemsCount(userId);
        return count != -1 ?
            Ok(count) : BadRequest();
    }



    [HttpGet("items")]
    public async Task<ActionResult<List<CartItemCatalog>>> GetCartItems()
    {
        int userId = User.GetUserId();
        var items = await CartsBusiness.GetCartItems(userId);
        return items != null ?
            Ok(items) : BadRequest();
    }



    [HttpGet]
    public async Task<ActionResult<decimal>> GetTotalPrice()
    {
        int userId = User.GetUserId();
        var totalPrice = await CartsBusiness.GetTotalPrice(userId);
        return totalPrice != -1 ?
            Ok(totalPrice) : BadRequest();
    }



}
