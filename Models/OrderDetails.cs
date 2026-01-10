namespace Models;

public class OrderDetails
{
    public decimal price { get; set; }
    public decimal totalPrice { get; set; }
    public int count { get; set; }
    public string? image { get; set; }
}
