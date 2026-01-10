using Enums;

namespace Models;

public class PromoCode
{
    public int id { get; set; }
    public string code { get; set; }
    public int productId { get; set; }
    public decimal discount { get; set; }
    public int count { get; set; }
    public DateTime expiryDate { get; set; }
    public DiscountType discountType { get; set; }
    public bool isEnable { get; set; }
}
