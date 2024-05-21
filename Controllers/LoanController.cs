using System.Security.Claims;
using Loanapp.Data;
using Loanapp.Models;
using Loanapp.Models.Loans;
using Loanapp.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loanapp.Controllers
{
    public class LoanController : Controller
    {
        private readonly LoanDbContext _context;
        private readonly IEmailService _emailService;

        public LoanController(LoanDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // public IActionResult Index()
        // {
        //     var loans = _context.Loans.ToList();
        //     return View(loans);
        // }
        
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Request.Cookies.TryGetValue("CustomerId", out string customerIdString) && int.TryParse(customerIdString, out int customerId))
            {
                var loans = await _context.Loans.Where(l => l.CustomerId == customerId).ToListAsync();
                return View(loans);
            }
            else
            {
                return RedirectToAction("Login", "Customer");
            }
        }


        

        public IActionResult Create()
        {
            if (HttpContext.Request.Cookies.TryGetValue("CustomerId", out string customerId))
            {
                ViewData["CustomerId"] = customerId;
            }

            else
            {
                return RedirectToAction("Login", "Customer");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanViewModel model)
        {
            if (HttpContext.Request.Cookies.TryGetValue("CustomerId", out string customerId))
            {
                model.CustomerId = int.Parse(customerId);
            }
            else
            {
                ModelState.AddModelError("", "Customer information not found");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var customer = _context.Customers.FirstOrDefault(c => c.Id == model.CustomerId);
                if (customer == null)
                {
                    
                    ModelState.AddModelError("", "Customer does not exist.");
                }

                decimal amountToRepay = model.Principal * (1 + model.Interest / 100);
                int numberOfPayments = model.Tenure * 12;
                decimal monthlyPayment = amountToRepay / numberOfPayments;
                

                var loan = new Loan
                {
                    Principal = model.Principal,
                    AmountToRepay = amountToRepay,
                    Interest = model.Interest,
                    Tenure = model.Tenure,
                    MonthlyPayment = monthlyPayment,
                    CustomerId = model.CustomerId 
                };

                _context.Loans.Add(loan);
                _context.SaveChanges();


                string subject = "loan application created";
                string body = $"Dear {customer.FirstName},\n\nYour loan application has been created successfully. Loan Details;\nPrincipal: {model.Principal}\nInterest: {model.Interest}%\nMonthly Payment: {model.MonthlyPayment}\nTenure: {model.Tenure} years";
                await _emailService.SendEmailAsync(customer.Email, subject, body);
                return RedirectToAction(nameof(System.Index));
            }
            return View();
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, LoanViewModel model)
        {
            if (id != null && id == model.Id && ModelState.IsValid)
            {
                try
                {
                    var loan = _context.Loans.FirstOrDefault(m => m.Id == id);
                    loan.Principal = model.Principal;
                    loan.AmountToRepay = model.Principal * (1 + model.Interest/100);
                    decimal monthlyInterestRate = model.Interest/100/12;
                    int numberOfPayments = model.Tenure * 12;
                    loan.MonthlyPayment = (model.Principal * monthlyInterestRate) /
                                          (1 - (decimal)Math.Pow((double)(1 + monthlyInterestRate), -numberOfPayments));

                    _context.Update(loan);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!id.HasValue || !LoanExists(id.Value))
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
}
