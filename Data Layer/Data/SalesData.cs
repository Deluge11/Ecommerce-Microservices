
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using System.Data;


using Models;

namespace Data_Layer.Data;

public class SalesData
{
    private string ConnectionString { get; }

    public SalesData(string connectionString)
    {
        ConnectionString = connectionString;

    }

    public async Task<List<SalesCatalog>> GetMySales(int stateId, int userId, int lastSeenId)
    {
        List<SalesCatalog> sales = new();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("GetSalesCatalog", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@SellerId", SqlDbType.Int) { Value = userId });
        command.Parameters.Add(new SqlParameter("@SalesState", SqlDbType.Int) { Value = stateId });
        command.Parameters.Add(new SqlParameter("@LastIdSeen", SqlDbType.Int) { Value = lastSeenId });
        command.Parameters.Add(new SqlParameter("@Take", SqlDbType.Int) { Value = 10 });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                sales.Add(new SalesCatalog
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    count = reader.GetInt32(reader.GetOrdinal("count")),
                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                    totalprice = reader.GetDecimal(reader.GetOrdinal("totalPricePerProduct")),
                    productId = reader.GetInt32(reader.GetOrdinal("productId")),
                    createDate = reader.GetDateTime(reader.GetOrdinal("createDate")),
                    promocode = reader.IsDBNull(reader.GetOrdinal("code")) ?
                     null : reader.GetString(reader.GetOrdinal("code")),
                });
            }
            return sales;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<List<SalesDetails>> GetAllSales(int stateId, int lastSeenId)
    {
        List<SalesDetails> sales = new();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("GetSalesDetails", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@SalesState", SqlDbType.Int) { Value = stateId });
        command.Parameters.Add(new SqlParameter("@LastIdSeen", SqlDbType.Int) { Value = lastSeenId });
        command.Parameters.Add(new SqlParameter("@Take", SqlDbType.Int) { Value = 10 });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                sales.Add(new SalesDetails
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    count = reader.GetInt32(reader.GetOrdinal("count")),
                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                    customerId = reader.GetInt32(reader.GetOrdinal("customerId")),
                    productId = reader.GetInt32(reader.GetOrdinal("productId")),
                    orderItemId = reader.GetInt32(reader.GetOrdinal("orderItemId")),
                    createDate = reader.GetDateTime(reader.GetOrdinal("createDate")),
                    promocodeId = reader.IsDBNull(reader.GetOrdinal("promocodeId")) ?
                     null : reader.GetInt32(reader.GetOrdinal("promocodeId"))
                });
            }
        }
        catch (Exception)
        {
            return null;
        }


        return sales;
    }
    public async Task<decimal?> GetSalesProfits(int sellerId)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("CalculateSalesPriceBySellerId", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@SellerId", SqlDbType.Int) { Value = sellerId });

        try
        {
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToDecimal(result);

        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<List<MerchantAccounting>> GetMerchantAccounting(int stateId, int lastIdSeen)
    {
        List<MerchantAccounting> result = new();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("GetMerchantAccounting", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@StateId", SqlDbType.Int) { Value = stateId });
        command.Parameters.Add(new SqlParameter("@LastIdSeen", SqlDbType.Int) { Value = lastIdSeen });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new MerchantAccounting
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    sellerId = reader.GetInt32(reader.GetOrdinal("sellerId")),
                    taxPercent = reader.GetDecimal(reader.GetOrdinal("taxPercent")),
                    totalPrice = reader.GetDecimal(reader.GetOrdinal("totalPrice")),
                    priceAfterTax = reader.GetDecimal(reader.GetOrdinal("priceAfterTax")),
                    date = reader.GetDateTime(reader.GetOrdinal("date")),
                });
            }
        }
        catch (Exception)
        {
            return null;
        }

        return result;
    }
    public async Task<List<MerchantAccountingDetails>> GetNewMerchantAccountingDetails()
    {
        using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
        using SqlCommand sqlCommand = new SqlCommand("GetMerchantAccountingDetails", sqlConnection);

        sqlCommand.CommandType = CommandType.StoredProcedure;

        List<MerchantAccountingDetails> result = new();

        try
        {
            await sqlConnection.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new MerchantAccountingDetails
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    sellerId = reader.GetInt32(reader.GetOrdinal("sellerId")),
                    sellerName = reader.GetString(reader.GetOrdinal("name")),
                    email = reader.GetString(reader.GetOrdinal("email")),
                    paypalEmail = reader.GetString(reader.GetOrdinal("paypalEmail")),
                    priceAfterTax = reader.GetDecimal(reader.GetOrdinal("priceAfterTax")),
                });
            }
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task UpdateMerchantAccountState(int id, int stateId)
    {
        string query = "UPDATE SellerTransfer SET transferStateId=@transferStateId WHERE id=@id";

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@transferStateId", SqlDbType.Int) { Value = stateId });
        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

        try
        {
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {

        }
    }
}
