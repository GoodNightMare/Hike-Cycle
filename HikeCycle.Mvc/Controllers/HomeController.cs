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

        public async Task<IActionResult> Experts()
        {
            var expertUserIds = await _db.UserProfiles
                .Where(p => p.IsExpert)
                .Select(p => p.UserId)
                .ToListAsync();

            var experts = await _db.Users
                .Where(u => expertUserIds.Contains(u.Id))
                .ToListAsync();

            var expertProfiles = await _db.UserProfiles
                .Where(p => expertUserIds.Contains(p.UserId))
                .ToListAsync();
            
            ViewBag.ExpertProfiles = expertProfiles;

            var expertReviews = await _db.Reviews
                .Where(r => expertUserIds.Contains(r.UserId))
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            ViewBag.ExpertReviews = expertReviews;

            var reviewedProductIds = expertReviews.Select(r => r.ProductId).Distinct().ToList();
            var reviewedProducts = await _db.Products
                .Include(p => p.ProductImages)
                .Where(p => reviewedProductIds.Contains(p.Id))
                .ToListAsync();
            ViewBag.ReviewedProducts = reviewedProducts;

            return View(experts);
        }
    }
}