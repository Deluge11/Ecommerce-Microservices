using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Mail;

namespace Data_Layer.Data;

public class EmailData 
{

    public string ConnectionString { get; }
    public EmailData(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public async Task<bool> EmailExists(string email)
    {
        using var conn = new SqlConnection(ConnectionString);
        using var sqlCommand = new SqlCommand("SELECT 1 FROM Users WHERE email = @email", conn);

        sqlCommand.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar) { Value = email });

        try
        {
            await conn.OpenAsync();
            return Convert.ToInt32(await sqlCommand.ExecuteScalarAsync()) > 0;
        }
        catch (Exception)
        {
            return false;
        }
        
    }
   
}
