using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using HikeCycle.Mvc.ViewModels;
using HikeCycle.Mvc.Models.db; 

namespace HikeCycle.Controllers
{
    public class HomeController : Controller
    {
        private readonly HikeCycledbContext _db;

        public HomeController(HikeCycledbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel();

            var promotions = await _db.Promotions.Where(p => p.Active).ToListAsync();
            if (promotions.Any())
            {
                viewModel.Banners = promotions.Select(p => $"{p.Title} : {p.Description}").ToList();
            }
            else
            {
                viewModel.Banners.Add("Hike-Cycle : อุปกรณ์เดินป่าคุณภาพดี");
            }

            viewModel.Routes = await _db.RecommendedRoutes.Where(r => r.IsActive).ToListAsync();

            return View(viewModel);
        }
    }
}