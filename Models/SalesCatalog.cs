namespace Models;

public class SalesCatalog
{
    public int id { get; set; }
    public int productId { get; set; }
    public int count { get; set; }
    public decimal price { get; set; }
    public decimal totalprice { get; set; }
    public string? promocode { get; set; }
    public DateTime createDate { get; set; }
}
