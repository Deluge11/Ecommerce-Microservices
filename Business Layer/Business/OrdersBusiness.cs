
using System.Text;
using System.Text.Json;
using Data_Layer.Data;
using Options;
using Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace Business_Layer.Business;

public class OrdersBusiness
{
    public OrdersBusiness(
        OrderData orderData,
        CartItemBusiness cartItemBusiness,
        InventoryKeyGenerator inventoryKeyGenerator,
        ILogger<OrdersBusiness> logger,
        StoreUrls storeUrls,
        HttpClient httpClient
        )
    {
        StoreUrls = storeUrls;
        OrderData = orderData;
        CartItemBusiness = cartItemBusiness;
        InventoryKeyGenerator = inventoryKeyGenerator;
        Logger = logger;
        HttpClient = httpClient;
    }

    public OrderData OrderData { get; }
    public CartItemBusiness CartItemBusiness { get; }
    public InventoryKeyGenerator InventoryKeyGenerator { get; }
    public ILogger<OrdersBusiness> Logger { get; }
    public StoreUrls StoreUrls { get; }
    public HttpClient HttpClient { get; }



    public async Task<OperationResult<Order>> CreateOrder(int userId)
    {
        OperationResult<Order> operationResult = new();

        if (userId == 0)
        {
            operationResult.ErrorMessage = "Invalid user id";
            return operationResult;
        }


        OperationResult<Order> createOrderOp = await OrderData.CreateOrder(userId);

        if (!createOrderOp.Success || createOrderOp.Data == null)
        {
            //if (await CartItemBusiness.SyncCartItemsPromocode(userId))
            //{
            //    operationResult.ErrorMessage = "The quantity of products has been modified to match the quantity of the promo codes.";
            //}
            //else
            //{
            //    operationResult.ErrorMessage = "Something went Wrong";
            //}

            operationResult.ErrorMessage = createOrderOp.ErrorMessage;

            return operationResult;
        }

        bool BookedOrderSuccess = await CreateStoreOrder(createOrderOp.Data.Id);

        if (!BookedOrderSuccess)
        {
            if (await CartItemBusiness.SyncCartItemsWithStocks(userId))
            {
                operationResult.ErrorMessage = "The quantity of products has been modified to match the quantity of the stocks.";
            }
            else
            {
                operationResult.ErrorMessage = "Something went Wrong";
            }

            return operationResult;
        }

        operationResult.Success = true;
        operationResult.Data = createOrderOp.Data;
        return operationResult;
    }


    public async Task<Order> GetOrderById(int orderId, int userId)
    {
        if (orderId < 1)
        {
            return null;
        }


        if (userId == 0)
        {
            return null;
        }

        return await OrderData.GetOrderById(orderId, userId);
    }

    public async Task<List<OrderDetails>> GetOrderDetails(int orderId, int userId)
    {
        if (orderId < 1)
        {
            return null;
        }


        if (userId == 0)
        {
            return null;
        }

        return await OrderData.GetOrderDetails(orderId, userId);
    }

    public async Task<List<Order>> GetMyOrders(int userId)
    {

        if (userId <= 0)
        {
            return [];
        }

        return await OrderData.GetOrdersByUserId(userId);
    }

    public async Task<List<Order>> GetOrdersByUserId(int userId)
    {
        if (userId <= 0)
        {
            return [];
        }

        return await OrderData.GetOrdersByUserId(userId);
    }
    public async Task<bool> CreateStoreOrder(int orderId)
    {
        if (orderId <= 0)
        {
            return false;
        }

        var items = await GetOrderItemQuantities(orderId);

        if (items == null || items.Count == 0)
        {
            return false;
        }

        try
        {
            string token = InventoryKeyGenerator.GenerateJwt();

            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(items);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(StoreUrls.BookingStocksRequest + $"{orderId}", content);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "Failed to create order with store service | Order Id: {orderId}", orderId);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while createing order with store service | Order Id: {orderId}", orderId);
            throw;
        }
    }

    public async Task<bool> ConfrimOrderInStore(int orderId)
    {
        if (orderId <= 0)
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

            var response = await HttpClient.PatchAsync(StoreUrls.ConfrimOrder + $"{orderId}", null);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "Failed to confirm order with store service | Order Id: {orderId}", orderId);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while confirming order with store service | Order Id: {orderId}", orderId);
            throw;
        }
    }

    private async Task<List<NewOrderRequest>> GetOrderItemQuantities(int orderId)
    {
        if (orderId <= 0)
        {
            return [];
        }

        return await OrderData.GetOrderItemQuantities(orderId);
    }


}