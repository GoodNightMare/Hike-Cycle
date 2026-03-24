using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ✅ ต้องมีเพื่อให้ใช้ ToListAsync ได้
using HikeCycle.Models; // ✅ เปลี่ยนให้ตรงกับ Namespace ของคุณ เพื่อให้หา HomeViewModel เจอ
using HikeCycle.Mvc.Models.db; // ✅ เปลี่ยนให้ตรงกับที่อยู่ของ HikeCycledbContext

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