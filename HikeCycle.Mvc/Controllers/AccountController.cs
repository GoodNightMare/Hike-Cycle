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
        private readonly HikeCycledbContext _db;

        public AccountController(HikeCycledbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            string hashedInput = HashPassword(password);

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedInput);

            if (user == null)
            {
                ViewBag.Error = "อีเมลหรือรหัสผ่านไม่ถูกต้อง";
                return View();
            }

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", profile?.FullName ?? "ไม่ระบุ");
            HttpContext.Session.SetString("UserRole", user.Role);

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, profile?.FullName ?? "ไม่ระบุ"),
                    new Claim(ClaimTypes.Role, user.Role)
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            if (user.Role == "admin") return RedirectToAction("Index", "AdminDashboard");
            if (user.Role == "staff") return RedirectToAction("Index", "AdminBookings");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel request)
        {
            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
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

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            _db.UserProfiles.Add(new UserProfile
            {
                UserId = newUser.Id,
                FullName = request.FullName,
                Phone = request.Phone,
                Address = request.Address
            });
            await _db.SaveChangesAsync();

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

            var user = await _db.Users.FindAsync(userId);

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            var bookings = await _db.Bookings
                                     .Where(b => b.UserId == userId)
                                     .Include(b => b.BookingItems)
                                     .ThenInclude(bi => bi.Product)
                                     .Include(b => b.Returns)
                                     .Include(b => b.Reviews)
                                     .OrderByDescending(b => b.Id)
                                     .ToListAsync();

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

            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {

                profile = new UserProfile { UserId = userId };
                _db.UserProfiles.Add(profile);
            }

            profile.FullName = fullName;
            profile.Phone = phone;
            profile.Address = address;

            await _db.SaveChangesAsync();

            HttpContext.Session.SetString("UserName", fullName ?? "ไม่ระบุ");

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