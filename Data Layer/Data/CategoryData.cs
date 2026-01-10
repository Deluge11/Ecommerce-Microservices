using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Models;

namespace Data_Layer.Data;

public class CategoryData 
{
    public string ConnectionString { get; }
    public ILogger<CategoryData> Logger { get; }

    public CategoryData(
        string connectionString,
        ILogger<CategoryData> logger
        )
    {
        ConnectionString = connectionString;
        Logger = logger;
    }

    public async Task<List<Category>> GetAll()
    {
        var categories = new List<Category>();
        string query = "SELECT * FROM Categories";

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(query, conn);
        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(new Category
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
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

        return categories;
    }
    public async Task<Category> GetById(int id)
    {
        Category category = null;
        string query = "SELECT * FROM Categories WHERE id = @id";

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return new Category();

            return new Category
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                image = reader.IsDBNull(reader.GetOrdinal("image")) ?
                null : reader.GetString(reader.GetOrdinal("image"))
            };
        }
        catch (Exception ex)
        {
            Logger.LogError("Database dwon Error Massage:{ex}", ex);
            return null;
        }
    }
    public async Task<bool> Add(string name)
    {

        string query = "INSERT INTO Categories (name) VALUES (@name)";
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = name });

        try
        {
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<bool> Update(int categoyId, string categoryName)
    {
        string query = "UPDATE Categories SET name = @name WHERE id = @id";
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = categoyId });
        cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = categoryName });

        try
        {
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task SetCategoryImage(string filePath, int categoryId)
    {
        string query = "UPDATE Categories SET image=@image WHERE id=@id";

        using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
        using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

        sqlCommand.Parameters.Add(new SqlParameter("@image", SqlDbType.VarChar) { Value = filePath });
        sqlCommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = categoryId });

        try
        {
            await sqlConnection.OpenAsync();
            await sqlCommand.ExecuteNonQueryAsync();
        }
        catch (Exception)
        {

        }

    }
}