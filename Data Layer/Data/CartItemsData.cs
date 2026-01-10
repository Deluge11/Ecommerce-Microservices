using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Models;
using Microsoft.Extensions.Logging;

namespace Data_Layer.Data;

public class CartItemsData
{
    public string ConnectionString { get; }
    public CartsData CartsRepo { get; }
    public ILogger<CartItemsData> Logger { get; }

    public CartItemsData
        (
        string connectionString,
        CartsData cartsRepo,
        ILogger<CartItemsData> logger
        )
    {
        ConnectionString = connectionString;
        CartsRepo = cartsRepo;
        Logger = logger;
    }


    public async Task<bool> InsertCartItem(int productId, int userId)
    {
        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand("InsertCartItem", sqlConnect);
        sqlcommand.CommandType = CommandType.StoredProcedure;

        sqlcommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
        sqlcommand.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });

        try
        {
            await sqlConnect.OpenAsync();
            return await sqlcommand.ExecuteNonQueryAsync() > 0;
        }
        catch (SqlException ex) when (ex.Number == 50000)
        {
            Logger.LogWarning("Business error: {Message}", ex.Message);
        }
        catch(SqlException ex)
        {
            Logger.LogError(ex, "Unhandled SQL error");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Database down");
            throw;
        }

        return false;
    }
    public async Task<bool> UpdateCartItem(int cartItemId, int count, int userId)
    {
        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand("UpdateCartItem", sqlConnect);
        sqlcommand.CommandType = CommandType.StoredProcedure;

        sqlcommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
        sqlcommand.Parameters.Add(new SqlParameter("@CartItemId", SqlDbType.Int) { Value = cartItemId });
        sqlcommand.Parameters.Add(new SqlParameter("@Count", SqlDbType.Int) { Value = count });

        try
        {
            await sqlConnect.OpenAsync();
            return await sqlcommand.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return false;
        }

    }
    public async Task<bool> DeleteCartItem(int itemId, int userId)
    {
        string query = "DELETE FROM CartItems WHERE id=@id AND cartId=@cartId";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = itemId });
        sqlcommand.Parameters.Add(new SqlParameter("@cartId", SqlDbType.Int) { Value = userId });

        try
        {
            await sqlConnect.OpenAsync();
            return await sqlcommand.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception)
        {
            return false;
        }


    }
    public async Task<bool> UsePromocode(int cartItemId, string promocode, int userId)
    {
        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand("UsePromoCode", sqlConnect);

        sqlcommand.CommandType = CommandType.StoredProcedure;
        sqlcommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
        sqlcommand.Parameters.Add(new SqlParameter("@CartItemId", SqlDbType.Int) { Value = cartItemId });
        sqlcommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar) { Value = promocode });

        try
        {
            await sqlConnect.OpenAsync();
            return await sqlcommand.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return false;
        }
    }
    public async Task<bool> SyncCartItemsPromocode(int userId)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("dbo.AsyncCartItemsWithProducts", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return false;
        }

    }

    public async Task<List<NewOrderRequest>> GetCartItemQuantities(int userId)
    {
        List<NewOrderRequest> result = new();

        string query = "SELECT productId,count FROM CartItems WHERE cartId = @UserId";

        using var conn = new SqlConnection(ConnectionString);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

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

    public async Task<bool> SyncCartItemsCount(DataTable items, int userId)
    {
        using var conn = new SqlConnection(ConnectionString);
        using var sqlCommand = new SqlCommand("SyncCartItemsCount", conn);


        var tvpParam = new SqlParameter("@Items", SqlDbType.Structured)
        {
            TypeName = "dbo.OrderItemType",
            Value = items
        };

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
        sqlCommand.Parameters.Add(tvpParam);

        try
        {
            await conn.OpenAsync();
            await sqlCommand.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return false;
        }


    }

}
