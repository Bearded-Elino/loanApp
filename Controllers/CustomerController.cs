using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Loanapp.Data;
using Loanapp.Models;
using Loanapp.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Loanapp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly LoanDbContext _context;
        private readonly IEmailService _emailService;

        public CustomerController(LoanDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }
        
        
        // POST: Customer/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Address,Password,NextOfKin,Phone,BVN")] Customer customer)
        {
            var existingEmail = await _context.Customers.FirstOrDefaultAsync(u => u.Email == customer.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email exists already!");
            }

            var existingPhone = await _context.Customers.FirstOrDefaultAsync(u => u.Phone == customer.Phone);
            if (existingPhone != null)
            {
                ModelState.AddModelError("Phone", "Phone number already exists");
            }

            var existingBvn = await _context.Customers.FirstOrDefaultAsync(u => u.BVN == customer.BVN);
            if (existingBvn != null)
            {
                ModelState.AddModelError("Bvn", "BVN already exists");
            }

            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                string subject = "Welcome to Loan App";
                string body = $"Dear {customer.FirstName}, \n\nThank you for registering with LoanApp";
                await _emailService.SendEmailAsync(customer.Email, subject, body);
                    
                return RedirectToAction(nameof(RegisterSuccess));
            }

            return View(customer);

        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        

        // POST: Customer/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Address,Password,NextOfKin,Phone,BVN")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            return View(customer);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer =
                    _context.Customers.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (customer != null)
                {
                    HttpContext.Response.Cookies.Append("CustomerId", customer.Id.ToString(), new CookieOptions()
                        {
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                        }
                    );
                    
                    return RedirectToAction("Index", "Loan");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

            }

            return View(model);
        }
        
    }
}
