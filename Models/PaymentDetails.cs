namespace Models;

public class PaymentDetails
{
    public string paymentId { get; set; }
    public decimal price { get; set; }
    public int userId { get; set; }
    public int orderId { get; set; }
    public int stateId { get; set; }
    public DateTime date { get; set; }
}
