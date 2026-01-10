using System.Data;
using System.Text.Json;
using System.Text;
using Models;
using Options;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Data_Layer.Data;

namespace Business_Layer.Business;

public class CartItemBusiness
{
    public CartItemsData CartItemData { get; }
    public InventoryKeyGenerator InventoryKeyGenerator { get; }
    public ILogger<CartItemBusiness> Logger { get; }
    public StoreUrls StoreUrls { get; }
    public HttpClient HttpClient { get; }

    public CartItemBusiness(
        CartItemsData cartItemData,
        InventoryKeyGenerator inventoryKeyGenerator,
        ILogger<CartItemBusiness> logger,
        StoreUrls storeUrls,
        HttpClient httpClient
        )
    {
        CartItemData = cartItemData;
        InventoryKeyGenerator = inventoryKeyGenerator;
        Logger = logger;
        StoreUrls = storeUrls;
        HttpClient = httpClient;
    }


    public async Task<bool> SyncCartItemsPromocode(int userId)
    {
        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.SyncCartItemsPromocode(userId);
    }

    public async Task<bool> DeleteCartItem(int itemId, int userId)
    {
        if (itemId < 1)
        {
            return false;
        }

        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.DeleteCartItem(itemId, userId);
    }

    public async Task<bool> InsertCartItem(int productId, int userId)
    {
        if (productId < 1)
        {
            return false;
        }

        if (userId == 0)
        {
            return false;
        }
        return await CartItemData.InsertCartItem(productId, userId);
    }

    public async Task<bool> PlusOneCartItem(int cartItemId, int userId)
    {
        if (cartItemId < 1)
        {
            return false;
        }

        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.UpdateCartItem(cartItemId, 1, userId);
    }
    public async Task<bool> MinusOneCartItem(int cartItemId, int userId)
    {
        if (cartItemId < 1)
        {
            return false;
        }


        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.UpdateCartItem(cartItemId, -1, userId);
    }

    public async Task<bool> UsePromocode(int productId, string promocode, int userId)
    {
        if (promocode == null)
        {
            return false;
        }

        promocode = promocode.Trim();

        if (promocode.Length < 1 || productId < 1)
        {
            return false;
        }

        if (userId == 0)
        {
            return false;
        }

        return await CartItemData.UsePromocode(productId, promocode, userId);
    }

    public async Task<List<NewOrderRequest>> GetCartItemQuantities(int userId)
    {
        if (userId == 0)
        {
            return null;
        }

        return await CartItemData.GetCartItemQuantities(userId);
    }

    public async Task<bool> SyncCartItemsCount(List<NewOrderRequest> items, int userId)
    {
        if (items == null || items.Count < 1)
        {
            return false;
        }

        if (userId == 0)
        {
            return false;
        }

        DataTable cartItemsTable = ToDataTable(items);

        if (cartItemsTable == null)
        {
            return false;
        }

        return await CartItemData.SyncCartItemsCount(cartItemsTable, userId);
    }

    public async Task<bool> SyncCartItemsWithStocks(int userId)
    {
        if (userId == 0)
        {
            return false;
        }

        var cartItemsQuantities = await GetCartItemQuantities(userId);

        if (cartItemsQuantities == null || cartItemsQuantities.Count < 1)
        {
            return false;
        }

        try
        {
            string token = InventoryKeyGenerator.GenerateJwt();

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var json = JsonSerializer.Serialize(cartItemsQuantities);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PatchAsync(StoreUrls.SyncOrderRequest, content);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var responseJson = await response.Content.ReadAsStringAsync();
            var modifiedItems = JsonSerializer.Deserialize<List<NewOrderRequest>>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (modifiedItems == null)
            {
                return false;
            }

            return await SyncCartItemsCount(modifiedItems, userId);
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "Failed to sync cart items with external service | UserId: {UserId}", userId);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while syncing cart items | UserId: {UserId}", userId);
            throw;
        }
    }

    protected DataTable ToDataTable(List<NewOrderRequest> items)
    {
        if (items == null || items.Count < 1)
        {
            return null;
        }

        var table = new DataTable();
        table.Columns.Add("mappingProductId", typeof(int));
        table.Columns.Add("quantity", typeof(int));

        foreach (var item in items)
        {
            table.Rows.Add(item.StockId, item.Quantity);
        }

        return table;
    }
}
