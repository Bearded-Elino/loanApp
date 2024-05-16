using System.ComponentModel.DataAnnotations;

namespace Loanapp.Models.Loans;

public class LoanViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Enter Principal Amount")]
    public decimal Principal { get; set; }
    [Required]
    public decimal AmountToRepay { get; set; }
    public decimal Interest { get; set; }
    [Required(ErrorMessage = "Please enter the tenure")]
    [Range(1, int.MaxValue, ErrorMessage = "Tenure must be a positive integer")]
    public int Tenure { get; set; }
    
    [Required(ErrorMessage = "Please select a customer")]
    public int CustomerId { get; set; }

    public decimal MonthlyPayment { get; set; }
}