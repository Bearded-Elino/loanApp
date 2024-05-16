using System.ComponentModel.DataAnnotations;

namespace Loanapp.Models;

public class Login
{
    [StringLength(100)]
    public string Email { get; set; }
    
    [StringLength(50)]
    public string Password { get; set; }
}