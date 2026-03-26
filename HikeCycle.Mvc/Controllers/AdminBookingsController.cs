using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;

namespace HikeCycle.Mvc.Controllers
{
    public class AdminBookingsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public AdminBookingsController(HikeCycledbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}