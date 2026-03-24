using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using HikeCycle.Models;
using HikeCycle.Mvc.Models.db; 

namespace HikeCycle.Controllers
{
    public class HomeController : Controller
    {
        // 1. ประกาศตัวแปร _context
        private readonly HikeCycledbContext _context;

        // 2. รับ Context ผ่าน Constructor (Dependency Injection)
        public HomeController(HikeCycledbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel();

            // ดึงข้อมูล Banner
            var promotions = await _context.Promotions.Where(p => p.Active).ToListAsync();
            if (promotions.Any())
            {
                viewModel.Banners = promotions.Select(p => $"{p.Title} : {p.Description}").ToList();
            }
            else
            {
                viewModel.Banners.Add("Hike-Cycle : อุปกรณ์เดินป่าคุณภาพดี");
            }

            // ดึงข้อมูล Routes
            viewModel.Routes = await _context.RecommendedRoutes.Where(r => r.IsActive).ToListAsync();

            return View(viewModel);
        }
    }
}