namespace Models;

public class MerchantAccounting
{
    public int id { get; set; }
    public int sellerId { get; set; }
    public decimal totalPrice { get; set; }
    public decimal taxPercent { get; set; }
    public decimal priceAfterTax { get; set; }
    public DateTime date { get; set; }
}
