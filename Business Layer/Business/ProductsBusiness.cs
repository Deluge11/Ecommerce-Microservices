

using Business_Layer.Sanitizations;
using Enums;
using Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text;
using Options;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Data_Layer.Data;


namespace Business_Layer.Business;

public class ProductsBusinees
{
    public ProductData ProductData { get; }

    public ProductsBusinees(ProductData productData)
    {
        ProductData = productData;
    }


    public async Task<decimal> GetMyProductPriceById(int productId, int userId)
    {
        if (productId < 1)
        {
            return -1;
        }


        if (userId == 0)
        {
            return -1;
        }

        return await ProductData.GetMyProductPriceById(productId, userId);
    }
    public async Task<List<string>> GetProductNames()
    {
        return await ProductData.GetProductNames();
    }

}
