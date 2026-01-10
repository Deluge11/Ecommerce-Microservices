
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using Microsoft.Extensions.Logging;
using Models;

namespace Data_Layer.Data;

public class OrderData
{


    public string ConnectionString { get; }
    public ILogger<OrderData> Logger { get; }

    public OrderData
        (
        string connectionString,
        ILogger<OrderData> logger
        )
    {
        ConnectionString = connectionString;
        Logger = logger;
    }

    public async Task<Order> GetOrderById(int orderId, int userId)
    {
        string query = "SELECT * FROM Orders WHERE id = @OrderId AND userId = @userId";

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId });
        command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            return new Order
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                TotalPrice = reader.GetDecimal(reader.GetOrdinal("totalPrice")),
                OrderDate = reader.GetDateTime(reader.GetOrdinal("orderDate"))
            };

        }
        catch (Exception ex)
        {
            return null;
        }

    }
    public async Task<OperationResult<Order>> CreateOrder(int userId)
    {
        var result = new OperationResult<Order>();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("dbo.CreateOrder", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            result.Data = new Order
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                TotalPrice = reader.GetDecimal(reader.GetOrdinal("totalPrice"))
            };
            result.Success = true;
        }
        catch (SqlException ex) when (ex.Number == 50000)
        {
            result.ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {

        }
        return result;
    }
    public async Task<List<Order>> GetOrdersByUserId(int userId)
    {
        var orders = new List<Order>();
        string query = "SELECT * FROM Orders WHERE userId = @UserId";

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                orders.Add(new Order
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                    TotalPrice = reader.GetDecimal(reader.GetOrdinal("totalPrice")),
                    OrderDate = reader.GetDateTime(reader.GetOrdinal("orderDate")),
                });
            }

        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return null;
        }

        return orders;
    }
    public async Task<List<OrderDetails>> GetOrderDetails(int orderId, int userId)
    {
        var orders = new List<OrderDetails>();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("GetOrderDetails", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId });
        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                orders.Add(new OrderDetails
                {
                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                    count = reader.GetInt32(reader.GetOrdinal("count")),
                    totalPrice = reader.GetDecimal(reader.GetOrdinal("totalPrice")),
                    image = reader.IsDBNull(reader.GetOrdinal("image")) ?
                     null : reader.GetString(reader.GetOrdinal("image"))
                });
            }

        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return null;
        }

        return orders;
    }
    public async Task<List<NewOrderRequest>> GetOrderItemQuantities(int orderId)
    {
        List<NewOrderRequest> result = new();

        string query = "SELECT productId,count FROM OrderItems WHERE orderId = @orderId";

        using var conn = new SqlConnection(ConnectionString);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@orderId", SqlDbType.Int) { Value = orderId });

        try
        {
            await conn.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new NewOrderRequest
                {
                    StockId = reader.GetInt32(reader.GetOrdinal("productId")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("count")),
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return null;
        }

    }

}