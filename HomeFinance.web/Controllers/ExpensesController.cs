using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HomeFinance.web.Data;
using HomeFinance.web.Models;

namespace HomeFinance.web.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly AppDbContext _context;

        public ExpensesController(AppDbContext context)
        {
            _context = context;
        }

        private void PopulateStoresDropdown(int? SelectedStoreId = null)
        {
            var stores = _context.Stores
                .Select(s => new SelectListItem
                {
                    Value = s.StoreId.ToString(),
                    Text = s.StoreName
                }).ToList();

            ViewBag.StoreId = new SelectList(stores, "Value", "Text", SelectedStoreId);
        }

        private void PopulateCategoryDropdown(string? selectedCategory = null)
        {
            var categories = new List<SelectListItem>
            {
                new SelectListItem { Value = "Groceries", Text = "Groceries" },
                new SelectListItem { Value = "Outing", Text = "Outing" },
                new SelectListItem { Value = "Utilities", Text = "Utilities" },
                new SelectListItem { Value = "Entertainment", Text = "Entertainment" },
                new SelectListItem { Value = "Other", Text = "Other" }
            };

            ViewBag.CategoryList = new SelectList(categories, "Value", "Text", selectedCategory);
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var expenses = await _context.Expenses
                                         .Include(e => e.Store)
                                         .Where(e => e.AppUserId == userId)
                                         .ToListAsync();

            return View(expenses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.Store)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (expense == null) return NotFound();

            return View(expense);
        }

        public IActionResult Create()
        {
            PopulateCategoryDropdown();
            PopulateStoresDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense, IFormFile? BillImage)
        {
            ModelState.Remove("BillImagePath");

            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                expense.AppUserId = userId;

                if (BillImage != null && BillImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bills");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(BillImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await BillImage.CopyToAsync(stream);

                    expense.BillImagePath = "/bills/" + uniqueFileName;
                }

                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateCategoryDropdown(expense.Category);
            PopulateStoresDropdown(expense.StoreId);
            return View(expense);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            PopulateCategoryDropdown(expense.Category);
            PopulateStoresDropdown(expense.StoreId);
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense updatedExpense, IFormFile? BillImage)
        {
            if (id != updatedExpense.Id)
                return NotFound();

            ModelState.Remove("BillImagePath");

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the original expense from DB
                    var originalExpense = await _context.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

                    if (originalExpense == null)
                        return NotFound();

                    // Preserve AppUserId and BillImagePath unless overridden
                    updatedExpense.AppUserId = originalExpense.AppUserId;

                    if (BillImage != null && BillImage.Length > 0)
                    {
                        // Save new image
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bills");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(BillImage.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await BillImage.CopyToAsync(stream);
                        }

                        updatedExpense.BillImagePath = "/bills/" + uniqueFileName;
                    }
                    else
                    {
                        // No new image uploaded — keep old one
                        updatedExpense.BillImagePath = originalExpense.BillImagePath;
                    }

                    _context.Update(updatedExpense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(updatedExpense.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateCategoryDropdown(updatedExpense.Category);
            PopulateStoresDropdown(updatedExpense.StoreId);
            return View(updatedExpense);
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.Store)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (expense == null) return NotFound();

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult AllBillImages()
{
    var images = _context.Expenses
        .Include(e => e.AppUser)
        .Where(e => !string.IsNullOrEmpty(e.BillImagePath))
        .Select(e => new BillImageViewModel
        {
            BillImagePath = e.BillImagePath,
            Username = e.AppUser != null ? e.AppUser.Username : "Unknown",
            Amount = e.Amount,
            Category = e.Category,
            Date = e.Date
        })
        .ToList();

    return View(images);
}




        public IActionResult ViewSummary()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var categorySummary = _context.Expenses
                .Where(e => e.AppUserId == userId)
                .GroupBy(e => e.Category)
                .Select(g => new ExpenseSummaryViewModel
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderBy(s => s.Category)
                .ToList();

            var monthSummaryData = _context.Expenses
                .Where(e => e.AppUserId == userId)
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            var monthSummary = monthSummaryData
                .Select(g => new ExpenseSummaryViewModel
                {
                    Month = new DateTime(g.Year, g.Month, 1).ToString("MMMM yyyy"),
                    Category = "",
                    TotalAmount = g.TotalAmount
                })
                .ToList();

            return View(new ExpenseSummaryPageViewModel
            {
                CategorySummary = categorySummary,
                MonthSummary = monthSummary
            });
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}
