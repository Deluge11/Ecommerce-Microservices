namespace Models;

public class InsertProductRequest
{
    public string name { get; set; }
    public decimal price { get; set; }
    public string? description { get; set; }
    public int categoryId { get; set; }
    public decimal weight { get; set; }
}
