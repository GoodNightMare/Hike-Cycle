using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.Models.db;
using System.Security.Cryptography;
using System.Text;

namespace HikeCycle.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly HikeCycledbContext _context;

        public AccountController(HikeCycledbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken] // ป้องกัน CSRF
        public async Task<IActionResult> Login(string email, string password)
        {
            string hashedInput = HashPassword(password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedInput);

            if (user == null)
            {
                ViewBag.Error = "อีเมลหรือรหัสผ่านไม่ถูกต้อง";
                return View();
            }

            // ในระบบ MVC จริงๆ ควรใช้ Cookie Authentication 
            // แต่เบื้องต้นสามารถเก็บลง Session หรือแจ้งผลกลับไปได้
            // ตัวอย่าง: การเก็บชื่อไว้โชว์
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", profile?.FullName ?? "User");
            HttpContext.Session.SetString("UserRole", user.Role);

            // Role-based redirection
            if (user.Role == "admin")
            {
                return RedirectToAction("Index", "AdminDashboard");
            }
            else if (user.Role == "staff")
            {
                return RedirectToAction("Index", "AdminOrders");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                TempData["RegError"] = "Email นี้ถูกใช้งานแล้ว";
                return RedirectToAction("Login");
            }

            var newUser = new User
            {
                Email = request.Email,
                Password = HashPassword(request.Password),
                Role = "user",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _context.UserProfiles.Add(new UserProfile
            {
                UserId = newUser.Id,
                FullName = request.FullName,
                Phone = request.Phone,
                Address = request.Address
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "สมัครสมาชิกสำเร็จ กรุณาเข้าสู่ระบบ";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login");
            }

            int userId = int.Parse(userIdStr);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            var bookings = await _context.Bookings
                                     .Where(b => b.UserId == userId)
                                     .Include(b => b.BookingItems)
                                     .ThenInclude(bi => bi.Product)
                                     .Include(b => b.Returns)
                                     .Include(b => b.Reviews)
                                     .OrderByDescending(b => b.StartDate)
                                     .ToListAsync();

            var now = DateTime.Now;
            var viewModel = new AccountProfileViewModel
            {
                User = user,
                Profile = profile,
                AllBookings = bookings,
            };

            return View(viewModel);
        }

        [HttpPost]
public async Task<IActionResult> UpdateProfile(string fullName, string phone, string address)
{
    var userIdStr = HttpContext.Session.GetString("UserId");
    if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login");

    int userId = int.Parse(userIdStr);
    
    // หาโปรไฟล์เดิมในตาราง UserProfiles
    var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    
    if (profile == null)
    {
        // ถ้ายังไม่มีก้อนโปรไฟล์ ให้สร้างใหม่
        profile = new UserProfile { UserId = userId };
        _context.UserProfiles.Add(profile);
    }

    // อัปเดตเฉพาะฟิลด์ที่อนุญาต
    profile.FullName = fullName;
    profile.Phone = phone;
    profile.Address = address;

    await _context.SaveChangesAsync();
    
    // อัปเดต Session ชื่อ (ถ้าต้องการให้ชื่อบน Nav เปลี่ยนทันที)
    HttpContext.Session.SetString("UserName", fullName ?? "User");

    return RedirectToAction("Profile");
}

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}