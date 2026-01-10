using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class AddProductQuantity
{
    public int stockId { get; set; }
    public int quantity { get; set; }
    public int receiverId { get; set; }
    public DateTime expiryDate { get; set; }

}
