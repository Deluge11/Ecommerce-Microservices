using Enums;

namespace Models;

public class OperationResult<T>
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public ErrorType? ErrorType { get; set; }
    public T? Data { get; set; }
}
