
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Logging;
using Models;


namespace Data_Layer.Data;

public class PayPalData 
{
    public string ConnectionString { get; }
    public ILogger<PayPalData> Logger { get; }


    public PayPalData
        (
        string connectionString,
        ILogger<PayPalData> _logger
        )
    {
        ConnectionString = connectionString;
        Logger = _logger;
    }

    public async Task<bool> UpdatePaymentStateId(string paymentId, int state)
    {
        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand("UPDATE OrderPaymentDetails SET stateId=@stateId WHERE id=@id", sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@stateId", SqlDbType.Int) { Value = state });
        sqlcommand.Parameters.Add(new SqlParameter("@id", SqlDbType.VarChar) { Value = paymentId });

        try
        {
            await sqlConnect.OpenAsync();
            return await sqlcommand.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            Logger.LogError("Database down Error Massage: {ex}", ex);
            return false;
        }

    }
    public async Task<PaymentDetails> GetPaymentDetails(string paymentId)
    {
        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand("GetPaymentDetailsById", sqlConnect);
        sqlcommand.CommandType = CommandType.StoredProcedure;
        sqlcommand.Parameters.Add(new SqlParameter("@PaymentId", SqlDbType.VarChar) { Value = paymentId });

        await sqlConnect.OpenAsync();
        using SqlDataReader reader = await sqlcommand.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new PaymentDetails
            {
                paymentId = paymentId,
                price = reader.GetDecimal(reader.GetOrdinal("price")),
                userId = reader.GetInt32(reader.GetOrdinal("userId")),
                orderId = reader.GetInt32(reader.GetOrdinal("orderId")),
                stateId = reader.GetInt32(reader.GetOrdinal("stateId")),
                date = reader.GetDateTime(reader.GetOrdinal("orderDate"))
            };

        }
        return null;
    }
    public async Task<bool> SaveOrderPayment(string paymentId, int orderId)
    {
        string query = @"INSERT INTO OrderPaymentDetails (id,orderId,stateId)
                                                        VALUES (@paymentId,@orderId,1)";

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@paymentId", SqlDbType.VarChar) { Value = paymentId });
        command.Parameters.Add(new SqlParameter("@orderId", SqlDbType.Int) { Value = orderId });


        try
        {
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            Logger.LogError("Error saving payment ID {paymentId} With Order Id: {orderId} ,Error Massage: {ex}", paymentId, orderId, ex);
            return false;
        }
    }
    public async Task SaveTransferPayout(string payoutId, int transferId)
    {
        string query = @"INSERT INTO SellerTransferPayoutDetails (payoutId,sellerTransferId)
                                                        VALUES (@payoutId,@sellerTransferId)";

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@payoutId", SqlDbType.VarChar) { Value = payoutId });
        command.Parameters.Add(new SqlParameter("@sellerTransferId", SqlDbType.Int) { Value = transferId });


        try
        {
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error saving payout ID {payoutId} and Transfer Id: {sellerTransferId} ,Error Massage: {ex}", payoutId, transferId, ex);
        }
    }
  
}
