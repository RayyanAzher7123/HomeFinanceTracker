using Microsoft.AspNetCore.Mvc;

namespace HomeFinance.web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
