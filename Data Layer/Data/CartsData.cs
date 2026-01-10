
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using Models;

namespace Data_Layer.Data;

public class CartsData 
{
    public string ConnectionString { get; }
    public ILogger<CartsData> Logger { get; }


    public CartsData
        (
        string connectionString,
        ILogger<CartsData> logger
        )
    {
        ConnectionString = connectionString;
        Logger = logger;
    }
    public async Task<int> GetCartItemsCount(int userId)
    {
        string query = "SELECT SUM(count) FROM CartItems WHERE cartId = @userId";
        using var conn = new SqlConnection(ConnectionString);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

        try
        {
            await conn.OpenAsync();
            var result = await sqlCommand.ExecuteScalarAsync();
            if(result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return -1;
        }
    }
    public async Task<List<CartItemCatalog>> GetCartItems(int userId)
    {
        List<CartItemCatalog> cartItems = new();

        using var conn = new SqlConnection(ConnectionString);
        using var sqlCommand = new SqlCommand("GetCartItemsCatalog", conn);

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await conn.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                cartItems.Add(new CartItemCatalog
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    productId = reader.GetInt32(reader.GetOrdinal("productId")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    count = reader.GetInt32(reader.GetOrdinal("count")),
                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                    totalPrice = reader.GetDecimal(reader.GetOrdinal("totalPrice")),

                    priceAfterDiscount = reader.IsDBNull(reader.GetOrdinal("priceAfterDiscount")) ?
                    null : reader.GetDecimal(reader.GetOrdinal("priceAfterDiscount")),

                    promocodeText = reader.IsDBNull(reader.GetOrdinal("code")) ?
                    null : reader.GetString(reader.GetOrdinal("code")),

                    discountType = reader.IsDBNull(reader.GetOrdinal("discountType")) ?
                    null : reader.GetString(reader.GetOrdinal("discountType")),

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

        return cartItems;
    }
    public async Task<decimal> GetTotalPrice(int userId)
    {
        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand("dbo.GetCartTotalPrice", sqlConnect);

        sqlcommand.CommandType = CommandType.StoredProcedure;
        sqlcommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await sqlConnect.OpenAsync();
            var result = await sqlcommand.ExecuteScalarAsync();
            if(result != null && result != DBNull.Value)
            {
                return (decimal)result;
            }
            return 0;
        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return -1;
        }
    }

}
