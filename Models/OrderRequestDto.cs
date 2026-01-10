

namespace Models;

public class OrderRequestDto
{
    public int OrderId { get; set; }
    public List<NewOrderRequest> Items { get; set; }
}
