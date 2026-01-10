namespace Models;

public class SalesDetails
{
    public int id { get; set; }
    public int count { get; set; }
    public decimal price { get; set; }
    public int orderItemId { get; set; }
    public int customerId { get; set; }
    public int productId { get; set; }
    public int? promocodeId { get; set; }
    public DateTime createDate { get; set; }
}
