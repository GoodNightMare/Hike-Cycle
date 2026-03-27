using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminUsersController : Controller
    {
        private readonly HikeCycledbContext _context;

        public AdminUsersController(HikeCycledbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            // ดึงข้อมูล User พร้อมกับ Profile (ถ้ามี) 
            // และเรียงลำดับ ID จากมากไปน้อย (ล่าสุดขึ้นก่อน)
            var users = await _context.Users
                .OrderBy(u => u.Id)
                .ToListAsync();

            // ดึงข้อมูล Profile มาเพื่อนำไป Join ใน View หรือจะใช้ ViewModel ก็ได้
            // ในที่นี้ขอส่ง Users ไป และใช้การดึง Profile ใน View เพื่อความง่าย
            ViewBag.Profiles = await _context.UserProfiles.ToListAsync();

            return View(users);
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UpdateUser(UserUpdateViewModel model)
{
    if (!ModelState.IsValid) return View("Index", await _context.Users.ToListAsync());

    var user = await _context.Users.FindAsync(model.Id);
    if (user == null) return NotFound();

    // 1. อัปเดตข้อมูลตาราง Users
    user.Email = model.Email;
    user.Role = model.Role;

    // 2. อัปเดตข้อมูลตาราง UserProfiles
    var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == model.Id);
    if (profile == null)
    {
        profile = new UserProfile { UserId = model.Id };
        _context.UserProfiles.Add(profile);
    }
    
    profile.FullName = model.FullName;
    profile.Phone = model.Phone;
    profile.Address = model.Address;

    await _context.SaveChangesAsync();
    
    // 🚩 ส่งกลับไปหน้า Index (หน้าจะ Refresh)
    return RedirectToAction(nameof(Index));
}

        // เพิ่ม API สำหรับลบ User
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // ลบ Profile ก่อน (ถ้ามี) เนื่องจากสัมพันธ์กัน
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == id);
            if (profile != null) _context.UserProfiles.Remove(profile);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}