

using Models;
using Data_Layer.Data;


namespace Business_Layer.Business;

public class ProductsBusinees
{
    public ProductData ProductData { get; }

    public ProductsBusinees(ProductData productData)
    {
        ProductData = productData;
    }

    public async Task<bool> Add(InsertProductRequest product)
    {
        return await ProductData.InsertProduct(product);
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
    public async Task<bool> Update(UpdateProductRequest product)
    {
        return await ProductData.UpdateProduct(product);
    }

}
