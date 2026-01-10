
using Microsoft.Data.SqlClient;
using System.Data;
using Models;


namespace Data_Layer.Data;

public class ProductData 
{
    public string ConnectionString { get; }
    public CategoryData CategoryRepo { get; }

    public ProductData
        (
        string connectionString,
        CategoryData categoryRepo
        )
    {
        ConnectionString = connectionString;
        CategoryRepo = categoryRepo;
    }

    public async Task<List<ProductCatalog>> GetProductsCatalog(int categoryId, int take, int lastSeenId)
    {
        List<ProductCatalog> products = new();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("dbo.GetProductCatalog", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@CategoryId", SqlDbType.Int) { Value = categoryId });
        command.Parameters.Add(new SqlParameter("@Take", SqlDbType.Int) { Value = take });
        command.Parameters.Add(new SqlParameter("@LastIdSeen", SqlDbType.Int) { Value = lastSeenId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new ProductCatalog
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                    image = reader.IsDBNull(reader.GetOrdinal("path")) ?
                     null : reader.GetString(reader.GetOrdinal("path"))

                });
            }
        }
        catch (Exception)
        {
            return null;
        }


        return products;
    }


    public async Task<List<ProductCatalog>> GetProductsCatalogForAllCategories(int take, int lastSeenId)
    {
        List<ProductCatalog> products = new();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("dbo.GetProductsCatalogForAllCategories", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@Take", SqlDbType.Int) { Value = take });
        command.Parameters.Add(new SqlParameter("@LastIdSeen", SqlDbType.Int) { Value = lastSeenId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new ProductCatalog
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                    image = reader.IsDBNull(reader.GetOrdinal("path")) ?
                     null : reader.GetString(reader.GetOrdinal("path"))

                });
            }
        }
        catch (Exception)
        {
            return null;
        }


        return products;
    }
    public async Task<ProductDetails> GetProductById(int productId)
    {

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("GetProductDetailsExtended", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new ProductDetails
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                count = reader.GetInt32(reader.GetOrdinal("count")),
                price = reader.GetDecimal(reader.GetOrdinal("price")),
                date = reader.GetDateTime(reader.GetOrdinal("date")),
                sellerName = reader.GetString(reader.GetOrdinal("sellerName")),
                userId = reader.GetInt32(reader.GetOrdinal("userId")),
                stateId = reader.GetInt32(reader.GetOrdinal("stateId")),
                categoryId = reader.GetInt32(reader.GetOrdinal("categoryId")),

                description = reader.IsDBNull(reader.GetOrdinal("description")) ?
                 null : reader.GetString(reader.GetOrdinal("description")),

                image = reader.IsDBNull(reader.GetOrdinal("path")) ?
                 null : reader.GetString(reader.GetOrdinal("path")),
            };

        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<List<ProductCatalog>> GetProductByUserId(int userId)
    {
        List<ProductCatalog> products = new();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("GetProductsByUserId", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new ProductCatalog
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                    image = reader.IsDBNull(reader.GetOrdinal("path")) ?
                     null : reader.GetString(reader.GetOrdinal("path")),

                });
            }
        }
        catch (Exception)
        {
            return null;
        }

        return products;
    }
    public async Task<List<ProductCatalog>> GetMyProducts(int userId)
    {
        List<ProductCatalog> products = new();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand("GetProductsByUserId", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new ProductCatalog
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                    image = reader.IsDBNull(reader.GetOrdinal("path")) ?
                     null : reader.GetString(reader.GetOrdinal("path")),

                });
            }
        }
        catch (Exception)
        {
            return null;
        }

        return products;
    }
    public async Task<List<ProductImage>> GetProductImages(int productId)
    {
        var images = new List<ProductImage>();

        string query = "SELECT id,path FROM ProductImage WHERE productId = @productId";
        using var connection = new SqlConnection(ConnectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@productId", SqlDbType.Int) { Value = productId });

        try
        {
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                images.Add(new ProductImage
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    path = reader.GetString(reader.GetOrdinal("path"))
                });
            }
        }
        catch (Exception)
        {
            return null;
        }

        return images;
    }
    public async Task<decimal> GetMyProductPriceById(int productId, int userId)
    {
        string query = @"SELECT price FROM Products WHERE id=@id AND userId=@userId";

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = productId });
        command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            return (decimal)await command.ExecuteScalarAsync();

        }
        catch (Exception)
        {
            return -1;
        }
    }

    public async Task<int> InsertProduct(InsertProductRequest product, int userId)
    {
        string query = @"INSERT INTO Products
                             (name,description,price,categoryId,date,userId,stateId)
                             Values (@name,@description,@price,@categoryId,GETDATE(),@userId,1);
                             SELECT CAST(scope_identity() AS int)";


        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = product.name });
        sqlcommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar) { Value = userId });
        sqlcommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal) { Value = product.price });
        sqlcommand.Parameters.Add(new SqlParameter("@categoryId", SqlDbType.Int) { Value = product.categoryId });
        sqlcommand.Parameters.Add(new SqlParameter("@description", SqlDbType.VarChar)
        { Value = product.description ?? (object)DBNull.Value });

        try
        {
            await sqlConnect.OpenAsync();
            return Convert.ToInt32(await sqlcommand.ExecuteScalarAsync());
        }
        catch (Exception)
        {
            return 0;
        }

    }
    public async Task<bool> UpdateProduct(UpdateProductRequest product, int userId)
    {
        string query = @"UPDATE Products SET 
                            name=@name, price=@price, description=@description 
                            WHERE id=@id AND userId=@userId";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = product.id });
        sqlcommand.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = product.name });
        sqlcommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal) { Value = product.price });
        sqlcommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
        sqlcommand.Parameters.Add(new SqlParameter("@description", SqlDbType.VarChar)
        { Value = product.description ?? (object)DBNull.Value });

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
    public async Task<bool> UpdateProductState(int productId, int userId, int state)
    {
        string query = "UPDATE Products SET stateId=@stateId WHERE id=@id AND userId=@userId";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = productId });
        sqlcommand.Parameters.Add(new SqlParameter("@stateId", SqlDbType.VarChar) { Value = state });
        sqlcommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar) { Value = userId });

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
    public async Task<bool> SetProductMainImage(int productId, int userId, int imageId)
    {
        if (!await IsMyProduct(productId, userId))
            return false;

        string query = "UPDATE Products SET imageId=@imageId WHERE id = @productId";
        using var connection = new SqlConnection(ConnectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@productId", SqlDbType.Int) { Value = productId });
        command.Parameters.Add(new SqlParameter("@imageId", SqlDbType.VarChar) { Value = imageId });

        try
        {
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception)
        {
            return false;
        }

    }
    public async Task<bool> IsMyProduct(int productId, int userId)
    {
        string query = "SELECT 1 FROM Products WHERE id=@id AND userId=@userId";
        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = productId });
        command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync()) == 1;
        }
        catch (Exception)
        {
            return false;
        }

    }
    public async Task<int> SaveImagePath(string filePath, int productId)
    {
        string query = @"INSERT INTO ProductImage (path, productId) VALUES (@path, @productId);
                            SELECT CAST(scope_identity() AS int);";

        using var sqlConnect = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(query, sqlConnect);

        cmd.Parameters.Add(new SqlParameter("@path", SqlDbType.VarChar) { Value = filePath });
        cmd.Parameters.Add(new SqlParameter("@productId", SqlDbType.Int) { Value = productId });

        try
        {
            await sqlConnect.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<List<string>> GetProductNames()
    {
        List<string> list = new();

        string query = "SELECT name FROM Products";

        using SqlConnection connection = new SqlConnection(ConnectionString);
        using SqlCommand command = new SqlCommand(query, connection);

        await connection.OpenAsync();
        using SqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(reader.GetString(reader.GetOrdinal("name")).ToLower());
        }

        return list;
    }



}
