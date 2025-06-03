using Microsoft.AspNetCore.Mvc;
using HomeFinance.web.Data;
using HomeFinance.web.Models;


namespace HomeFinance.web.Controllers
{
    public class StoresController : Controller
    {
        private readonly AppDbContext _context;


        public StoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: Stores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Store store)
        {
            if (ModelState.IsValid)
            {
                _context.Stores.Add(store);
                _context.SaveChanges();
                return RedirectToAction("Create", "Expenses");
            }
            return View(store);
        }


    }
}
