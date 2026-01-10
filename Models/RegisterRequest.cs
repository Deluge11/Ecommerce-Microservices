using System.ComponentModel.DataAnnotations;

namespace Models;

public class RegisterRequest
{
    public string name { get; set; }

    [EmailAddress]
    public string email { get; set; }
    public string password { get; set; }
    public string confirmPassword { get; set; }
}
