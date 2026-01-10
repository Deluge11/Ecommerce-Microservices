
using Data_Layer.Data;
using Models;
namespace Business_Layer.Business;

public class CartsBusiness 
{
    public CartsData CartsData { get; }

    public CartsBusiness(CartsData cartsData)
    {
        CartsData = cartsData;
    }


    public async Task<List<CartItemCatalog>> GetCartItems(int userId)
    {
        if(userId == 0)
        {
            return null;
        }

        return await CartsData.GetCartItems(userId);
    }

    public async Task<int> GetCartItemsCount(int userId)
    {

        if (userId == 0)
        {
            return 0;
        }

        return await CartsData.GetCartItemsCount(userId);
    }

    public async Task<decimal> GetTotalPrice(int userId)
    {

        if (userId == 0)
        {
            return 0;
        }

        return await CartsData.GetTotalPrice(userId);
    }

}
