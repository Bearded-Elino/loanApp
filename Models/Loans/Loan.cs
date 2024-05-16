using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Loanapp.Models.Loans;

public class Loan
{
    public int Id { get; set; }
    public decimal Principal { get; set; }
    public decimal AmountToRepay { get; set; }
    public decimal Interest { get; set; }
    public decimal Tenure { get; set; }
    [Required(ErrorMessage = "Please enter the tenure")]
    [Range(1, int.MaxValue, ErrorMessage = "Tenure must be a positive integer")]
    public decimal MonthlyPayment { get; set; }
    
    public int CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
}