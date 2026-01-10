namespace Models;

public class Product
{
    public int id { get; set; }
    public string name { get; set; }
    public int count { get; set; }
    public decimal price { get; set; }
    public DateTime date { get; set; }
    public string? description { get; set; }
    public int stateId { get; set; }
    public int userId { get; set; }
    public int categoryId { get; set; }
}
