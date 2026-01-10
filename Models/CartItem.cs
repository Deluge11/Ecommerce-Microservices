namespace Models;

public class CartItem
{
    public int id { get; set; }
    public int count { get; set; }
    public decimal price { get; set; }
    public int? promocodeId { get; set; }
    public int cartId{ get; set; }
}
