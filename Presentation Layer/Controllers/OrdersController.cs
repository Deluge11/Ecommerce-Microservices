using Presentation_Layer.Authorization;
using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Presentation_Layer.Extensions;


namespace Presentation_Layer.Controllers;


[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    public ILogger<OrdersController> logger { get; }
    public OrdersBusiness OrdersBusiness { get; }

    public OrdersController (
        ILogger<OrdersController> _logger,
        OrdersBusiness ordersBusiness
        )
    {
        logger = _logger;
        OrdersBusiness = ordersBusiness;
    }



    [CheckPermission(Permission.Orders_ViewOwnOrders)]
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetMyOrders()
    {
        var orders = await OrdersBusiness.GetMyOrders(User.GetUserId());
        return orders != null ?
            Ok(orders) : NotFound();
    }



    [CheckPermission(Permission.Orders_ViewOwnOrders)]
    [HttpGet("{orderId}")]
    public async Task<ActionResult<List<OrderDetails>>> GetMyOrderDetails(int orderId)
    {
        var orders = await OrdersBusiness.GetOrderDetails(orderId, User.GetUserId());
        return orders != null ?
            Ok(orders) : NotFound();
    }



    [CheckPermission(Permission.Orders_ViewUserOrders)]
    [HttpGet("UserOrder/{id}")]
    public async Task<ActionResult<List<Order>>> GetOrdersByUserId(int id)
    {
        var orders = await OrdersBusiness.GetOrdersByUserId(id);
        return orders != null ?
            Ok(orders) : NotFound();
    }


}
