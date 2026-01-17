using Presentation_Layer.Authorization;
using Enums;
using Models;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Presentation_Layer.Extensions;


namespace Presentation_Layer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    public ProductsBusinees ProductsBusiness { get; }

    public ProductsController(ProductsBusinees productsBusiness)
    {
        ProductsBusiness = productsBusiness;
    }


    [HttpGet]
    public async Task<ActionResult<List<ProductCatalog>>> GetProductsPage([FromQuery] int categoryId, [FromQuery] int take, [FromQuery] int lastSeenId)
    {
        var products = await ProductsBusiness.GetProductsCatalog(categoryId, take, lastSeenId);
        return products != null ?
            Ok(products) : BadRequest();
    }



    [HttpGet("get-all-sections")]
    public async Task<IActionResult> GetProductsCatalogForAllCategories([FromQuery] int categoryId, [FromQuery] int take, [FromQuery] int lastSeenId)
    {
        var products = await ProductsBusiness.GetProductsCatalogForAllCategories(take, lastSeenId);

        if (products == null)
        {
            return BadRequest();
        }

        if(products.Count == 0)
        {
            return NoContent();
        }

        var result = new
        {
            Products = products,
            LastSeenId = products[products.Count - 1].id
        };
        return Ok(result);
    }



    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductDetails>> GetProductById(int productId)
    {
        var product = await ProductsBusiness.GetProductById(productId);
        return product != null ?
            Ok(product) : NotFound();
    }



    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ProductCatalog>>> GetProductsByUserId(int userId)
    {
        var products = await ProductsBusiness.GetProductByUserId(userId);
        return products != null ?
            Ok(products) : NotFound();
    }



    [HttpGet("images/{productId}")]
    public async Task<ActionResult<List<ProductImage>>> GetImages(int productId)
    {
        var images = await ProductsBusiness.GetProductImages(productId);
        return images != null ?
            Ok(images) : NotFound();
    }



    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpGet("my-products")]
    public async Task<ActionResult<List<ProductCatalog>>> GetMyProducts()
    {
        var products = await ProductsBusiness.GetMyProducts(User.GetUserId());
        return products != null ?
            Ok(products) : NotFound();
    }

    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPost]
    public async Task<ActionResult<int>> InsertProduct(InsertProductRequest product)
    {
        var result = await ProductsBusiness.InsertProduct(product, User.GetUserId());
        return result.Success ?
            Ok(result) : BadRequest(result.ErrorMessage);
    }



    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPost("add-quantity")]
    public async Task<IActionResult> AddStockQuantity(AddProductQuantity request)
    {
        return await ProductsBusiness.AddStockQuantity(request, User.GetUserId()) ?
            Ok() : BadRequest();
    }



    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPost("{productId}")]
    public async Task<ActionResult> UploadImage(List<IFormFile> images, int productId)
    {
        var result = await ProductsBusiness.UploadImage(images, productId, User.GetUserId());
        return result.Success ?
            Ok(result.Data) : BadRequest(result.ErrorMessage);
    }



    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPut]
    public async Task<ActionResult> UpdateProduct(UpdateProductRequest product)
    {
        return await ProductsBusiness.UpdateProduct(product, User.GetUserId()) ?
            Ok() : BadRequest("Cant access this product");

    }



    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPatch("image/{productId}")]
    public async Task<ActionResult> SetMainImage(int productId, [FromQuery] int imageId)
    {
        return await ProductsBusiness.SetProductMainImage(productId, imageId, User.GetUserId()) ?
            Ok() : BadRequest();
    }



    [CheckPermission(Permission.Products_ChangeProductState)]
    [HttpPatch("{productId}")]
    public async Task<ActionResult> UpdateProductState(int productId, ProductState state)
    {
        return await ProductsBusiness.UpdateProductState(productId, state, User.GetUserId()) ?
            Ok() : BadRequest();
    }


}
