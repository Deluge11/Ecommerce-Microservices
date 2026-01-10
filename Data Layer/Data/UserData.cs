using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.Data
{
    public class UserData 
    {

        public UserData(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public async Task<bool> Add(User user)
        {
            var query = "INSERT INTO Users (id,name,email) VALUES (@id,@name,@email)";
            var sqlConnection = new SqlConnection(ConnectionString);
            var sqlCommand = new SqlCommand(query, sqlConnection);

            sqlCommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = user.id });
            sqlCommand.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = user.name });
            sqlCommand.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar) { Value = user.email });

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
}
