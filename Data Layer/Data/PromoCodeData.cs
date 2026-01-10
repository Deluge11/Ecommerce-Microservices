
using Microsoft.Data.SqlClient;
using System.Data;
using Enums;


using Models;


namespace Data_Layer.Data;

public class PromoCodeData 
{

    public string ConnectionString { get; }


    public PromoCodeData(string connectionString)
    {
        ConnectionString = connectionString;
    }


    public async Task<List<PromoCode>> GetPromoCodes(int userId)
    {
        List<PromoCode> list = new();
        string query = @"SELECT * FROM PromoCodes WHERE userId=@userId";

        using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
        using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

        sqlCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

        try
        {
            await sqlConnection.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PromoCode
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    code = reader.GetString(reader.GetOrdinal("code")),
                    discount = reader.GetDecimal(reader.GetOrdinal("discount")),
                    productId = reader.GetInt32(reader.GetOrdinal("productId")),
                    discountType = (DiscountType)reader.GetInt32(reader.GetOrdinal("stateId")),
                    expiryDate = reader.GetDateTime(reader.GetOrdinal("expiryDate")),
                    count = reader.GetInt32(reader.GetOrdinal("count")),
                    isEnable = reader.GetBoolean(reader.GetOrdinal("isEnable")),

                });
            }
            return list;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<OperationResult<bool>> AddPromoCode(AddPromocode promoCode, int userId)
    {
        var result = new OperationResult<bool>();

        string query = @"INSERT INTO PromoCodes (code,userId,productId,discount,count,expiryDate,stateId,isEnable)
                                                VALUES (@code,@userId,@productId,@discount,@count,@expiryDate,@stateId,@isEnable);";

        using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
        using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

        sqlCommand.Parameters.Add(new SqlParameter("@code", SqlDbType.VarChar) { Value = promoCode.code });
        sqlCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
        sqlCommand.Parameters.Add(new SqlParameter("@productId", SqlDbType.Int) { Value = promoCode.productId });
        sqlCommand.Parameters.Add(new SqlParameter("@discount", SqlDbType.Decimal) { Value = promoCode.discount });
        sqlCommand.Parameters.Add(new SqlParameter("@count", SqlDbType.Int) { Value = promoCode.count });
        sqlCommand.Parameters.Add(new SqlParameter("@expiryDate", SqlDbType.DateTime) { Value = promoCode.expiryDate });
        sqlCommand.Parameters.Add(new SqlParameter("@stateId", SqlDbType.Int) { Value = promoCode.discountType });
        sqlCommand.Parameters.Add(new SqlParameter("@isEnable", SqlDbType.Bit) { Value = promoCode.isEnable });

        try
        {
            await sqlConnection.OpenAsync();
            result.Success = await sqlCommand.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception)
        {
            result.Success = false;
            result.ErrorType = ErrorType.ServerIsDown;
        }
        return result;
    }
    public async Task<bool> TogglePromocode(int promocodeId, int userId)
    {

        string query = @"UPDATE PromoCodes
                            SET isEnable = CASE 
                                WHEN isEnable = 0 THEN 1
                                WHEN isEnable = 1 THEN 0
                            END
                            WHERE id=@id AND userId=@userId";

        using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
        using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

        sqlCommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = promocodeId });
        sqlCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

        try
        {
            await sqlConnection.OpenAsync();
            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
