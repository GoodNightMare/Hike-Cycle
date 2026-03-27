using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminUsersController : Controller
    {
        private readonly HikeCycledbContext _db;

        public AdminUsersController(HikeCycledbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var users = await _db.Users
                .OrderBy(u => u.Id)
                .ToListAsync();

            ViewBag.Profiles = await _db.UserProfiles.ToListAsync();

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UserUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", await _db.Users.ToListAsync());

            var user = await _db.Users.FindAsync(model.Id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.Role = model.Role;

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == model.Id);
            if (profile == null)
            {
                profile = new UserProfile { UserId = model.Id };
                _db.UserProfiles.Add(profile);
            }

            profile.FullName = model.FullName;
            profile.Phone = model.Phone;
            profile.Address = model.Address;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == id);
            if (profile != null) _db.UserProfiles.Remove(profile);

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}