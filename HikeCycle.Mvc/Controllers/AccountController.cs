using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.ViewModels;
using HikeCycle.Mvc.Models.db;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", profile?.FullName ?? "User");
            HttpContext.Session.SetString("UserRole", user.Role);

            // 🎫 1. สร้างบัตรผ่าน (Claims)
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, profile?.FullName ?? "User"),
        new Claim(ClaimTypes.Role, user.Role) // 🚩 สำคัญมาก: ตัวนี้จะทำให้ [Authorize(Roles="...")] ทำงานได้
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 🍪 2. สั่งให้เบราว์เซอร์เก็บ Cookie ยืนยันตัวตน
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            if (user.Role == "admin") return RedirectToAction("Index", "AdminDashboard");
            if (user.Role == "staff") return RedirectToAction("Index", "AdminBookings");
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel request)
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
                                     .OrderByDescending(b => b.Id)
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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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