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

        // Populate Store Dropdown
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

        // Dropdown Category
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

        // GET: Expenses
        public async Task<IActionResult> Index()
        {
            var expenses = await _context.Expenses.Include(e => e.Store).ToListAsync();
            return View(expenses);
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .Include(e => e.Store)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create()
        {
            PopulateCategoryDropdown();
            PopulateStoresDropdown();
            return View();
        }

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense, IFormFile BillImage)
        {
            if (ModelState.IsValid)
            {
                if (BillImage != null && BillImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bills");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(BillImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await BillImage.CopyToAsync(stream);
                    }

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

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            PopulateCategoryDropdown(expense.Category);
            PopulateStoresDropdown(expense.StoreId);
            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense, IFormFile BillImage)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (BillImage != null && BillImage.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bills");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(BillImage.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await BillImage.CopyToAsync(stream);
                        }

                        expense.BillImagePath = "/bills/" + uniqueFileName;
                    }

                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.Id))
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

            PopulateCategoryDropdown(expense.Category);
            PopulateStoresDropdown(expense.StoreId);
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .Include(e => e.Store)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ViewSummary()
        {
            var categorySummary = _context.Expenses
                .GroupBy(e => e.Category)
                .Select(g => new ExpenseSummaryViewModel
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderBy(s => s.Category)
                .ToList();

            var monthSummaryData = _context.Expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            var monthSummary = monthSummaryData
                .Select(g => new ExpenseSummaryViewModel
                {
                    Month = new DateTime(g.Year, g.Month, 1).ToString("MMMM yyyy"),
                    Category = "",
                    TotalAmount = g.TotalAmount
                })
                .ToList();

            var viewModel = new ExpenseSummaryPageViewModel
            {
                CategorySummary = categorySummary,
                MonthSummary = monthSummary
            };

            return View(viewModel);
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}
