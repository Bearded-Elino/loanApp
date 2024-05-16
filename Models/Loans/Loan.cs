using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Loanapp.Models.Loans;

public class Loan
{
    public int Id { get; set; }
    public decimal Principal { get; set; }
    public decimal AmountToRepay { get; set; }
    public decimal Interest { get; set; }
    public int Tenure { get; set; }
    public decimal MonthlyPayment { get; set; }
    
    public int CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
}