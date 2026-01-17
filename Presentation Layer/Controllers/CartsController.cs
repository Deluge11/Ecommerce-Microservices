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
        var count = await CartsBusiness.GetCartItemsCount(User.GetUserId());
        return count != -1 ?
            Ok(count) : BadRequest();
    }



    [HttpGet("items")]
    public async Task<ActionResult<List<CartItemCatalog>>> GetCartItems()
    {
        var items = await CartsBusiness.GetCartItems(User.GetUserId());
        return items != null ?
            Ok(items) : BadRequest();
    }



    [HttpGet]
    public async Task<ActionResult<decimal>> GetTotalPrice()
    {
        var totalPrice = await CartsBusiness.GetTotalPrice(User.GetUserId());
        return totalPrice != -1 ?
            Ok(totalPrice) : BadRequest();
    }



}
