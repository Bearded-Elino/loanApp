using System.ComponentModel.DataAnnotations;

namespace Loanapp.Models.Loans;

public class LoanViewModel
{
    public int Id { get; set; }
    [Required]
    public decimal Principal { get; set; }
    [Required]
    public decimal AmountToRepay { get; set; }
    public decimal Interest { get; set; }
    [Required]
    public int Tenure { get; set; }
    [Required]
    public decimal MonthlyPayment { get; set; }
}