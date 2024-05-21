using System.ComponentModel.DataAnnotations;

namespace Loanapp.Models;

public class CustomerViewModel
{
    public string FirstName { get; set; }
    [StringLength(50)]
    public string LastName { get; set; }
    [StringLength(150)]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    [StringLength(200)]
    public string Address { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [StringLength(80)]
    public string Password { get; set; }
    
    [StringLength(50)]
    public string NextOfKin { get; set; }
    [StringLength(11)]
    [RegularExpression(@"d{11}$")]
    public string Phone { get; set; }
    [StringLength(18)]
    public string BVN { get; set; }

}