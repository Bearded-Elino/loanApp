using Loanapp.Models;
using Loanapp.Models.Loans;
using Microsoft.EntityFrameworkCore;

namespace Loanapp.Data;

public class LoanDbContext : DbContext
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
    {
        
    }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Loan> Loans { get; set; }
}