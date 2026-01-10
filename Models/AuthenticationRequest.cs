using System.ComponentModel.DataAnnotations;

namespace Models;

public class AuthenticationRequest
{
    [Required]
    public string email { get; set; }
    [Required]
    public string password { get; set; }
}
