using System.ComponentModel.DataAnnotations;

namespace Loanapp.Models;

public class LoginViewModel
{
    [StringLength(100)]
    [Required]
    public string Email { get; set; }
    
    [StringLength(50)]
    [Required]
    public string Password { get; set; }
}