namespace Models;

public class CartItemCatalog
{
    public int id { get; set; }
    public int productId { get; set; }
    public string name { get; set; }
    public int count { get; set; }
    public decimal price { get; set; }
    public decimal totalPrice { get; set; }
    public string? image { get; set; }
    public string? promocodeText { get; set; }
    public decimal? priceAfterDiscount { get; set; }
    public string? discountType { get; set; }
}
