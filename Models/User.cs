using System.ComponentModel.DataAnnotations;

namespace Models;

public class User
{
    public int id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
}
