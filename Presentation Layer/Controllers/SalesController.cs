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
public class SalesController : ControllerBase
{
    private SalesBusiness SalesBusiness { get; }

    public SalesController(SalesBusiness salesBusiness)
    {
        SalesBusiness = salesBusiness;
    }



    [CheckPermission(Permission.Sales_ViewOwnSales)]
    [HttpGet("my-sales")]
    public async Task<IActionResult> GetMySales([FromQuery] SalesState state, [FromQuery] int lastIdSeen)
    {
        int userId = User.GetUserId();
        var sales = await SalesBusiness.GetMySales(state, lastIdSeen,userId);
        return sales != null ?
            Ok(sales) : NotFound();
    }



    [CheckPermission(Permission.Sales_ViewAllSales)]
    [HttpGet]
    public async Task<IActionResult> GetAllSales([FromQuery] SalesState state, [FromQuery] int lastIdSeen)
    {
        var sales = await SalesBusiness.GetAllSales(state, lastIdSeen);
        return sales != null ?
            Ok(sales) : NotFound();
    }



    [CheckPermission(Permission.Sales_ViewOwnSales)]
    [HttpGet("profits")]
    public async Task<IActionResult> GetMySalesProfits()
    {
        int userId = User.GetUserId();
        var price = await SalesBusiness.GetSalesProfits(userId);
        return price != null ?
            Ok(price) : NotFound();
    }



    [CheckPermission(Permission.Sales_ViewAllSales)]
    [HttpGet("profits/{sellerId}")]
    public async Task<IActionResult> GetSalesProfitsBySellerId(int sellerId)
    {
        var price = await SalesBusiness.GetSalesProfits(sellerId);
        return price != null ?
            Ok(price) : NotFound();
    }



    [CheckPermission(Permission.Sales_ViewAllSales)]
    [HttpGet("MerchantAccounting")]
    public async Task<IActionResult> GetMerchantAccountingDetails([FromQuery] MerchantAccountingState state, [FromQuery] int lastIdSeen)
    {
        var result = await SalesBusiness.GetMerchantAccounting(state, lastIdSeen);
        return result != null ?
            Ok(result) : NotFound();
    }
}
