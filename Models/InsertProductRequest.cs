namespace Models;

public class InsertProductRequest
{
    public int id { get; set; }
    public string name { get; set; }
    public decimal price { get; set; }
    public string? description { get; set; }
    public DateTime createdDate { get; set; }
    public int userId { get; set; }
}
