namespace Models;

public class MerchantAccountingDetails
{
    public int id { get; set; }
    public int sellerId { get; set; }
    public string sellerName { get; set; }
    public decimal priceAfterTax { get; set; }
    public string email { get; set; }
    public string paypalEmail { get; set; }
}
