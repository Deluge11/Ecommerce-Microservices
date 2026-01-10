using Enums;

namespace Models;

public class SalesRequest
{
    public SalesState state { get; set; }
    public int lastIdSeen { get; set; }
}
