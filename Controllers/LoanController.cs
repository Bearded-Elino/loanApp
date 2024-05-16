using System.Security.Claims;
using Loanapp.Data;
using Loanapp.Models.Loans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanapp.Controllers;

public class LoanController : Controller
{
    private readonly LoanDbContext _context;

    public LoanController(LoanDbContext context)
    {
        _context = context;
    }
    // GET
    public IActionResult Index()
    {
        var loans = _context.Loans.ToList();
        return View(loans);
    }
    // GET: Loan/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = _context.Loans.FirstOrDefault(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Loan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LoanViewModel model)
        {
            if (ModelState.IsValid)
            {
                decimal amountToRepay = model.Principal * (1 + model.Interest / 100);

                // Calculate monthly payment
                decimal monthlyInterestRate = model.Interest / 100 / 12; // Monthly interest rate
                int numberOfPayments = model.Tenure * 12; // Total number of payments
                decimal monthlyPayment = (model.Principal * monthlyInterestRate) / 
                                         (1 - (decimal)Math.Pow((double)(1 + monthlyInterestRate), -numberOfPayments));
                // Map the view model to your loan entity and save it to the database
                var loan = new Loan
                {
                    Principal = model.Principal,
                    AmountToRepay = model.AmountToRepay,
                    Interest = model.Interest,
                    Tenure = model.Tenure,
                    MonthlyPayment = model.MonthlyPayment,
                    CustomerId = GetCustomerId() // Implement logic to get the current user's ID
                };

                _context.Add(loan);
                _context.SaveChanges();
                
                // Redirect to the index action to show all loans after creating a new loan
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Loan/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = _context.Loans.FirstOrDefault(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, LoanViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the existing loan entity with the new data from the view model
                    var loan = _context.Loans.FirstOrDefault(m => m.Id == id);
                    loan.Principal = model.Principal;
                    loan.AmountToRepay = model.AmountToRepay;
                    loan.Interest = model.Interest;
                    loan.Tenure = model.Tenure;
                    loan.MonthlyPayment = model.MonthlyPayment;

                    _context.Update(loan);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!id.HasValue)
                    {
                        return NotFound();
                    }
                    
                    if (!LoanExists(id.Value))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Loan/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = _context.Loans.FirstOrDefault(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var loan = _context.Loans.FirstOrDefault(m => m.Id == id);
            _context.Loans.Remove(loan);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.Id == id);
        }
        
        private int GetCustomerId()
        {
            // Get the current user's ID using ASP.NET Core Identity
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
            // Convert the user ID to an integer if necessary
            if (int.TryParse(userId, out int customerId))
            {
                return customerId;
            }
            else
            {
                return 0;
            }
        }
        
}