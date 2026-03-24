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
            HttpContext.Session.SetString("UserName", profile?.FullName ?? "User");
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Home");
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

            var newUser = new User {
                Email = request.Email,
                Password = HashPassword(request.Password),
                Role = "user",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _context.UserProfiles.Add(new UserProfile {
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
    // 🚩 จุดเช็คที่ 1: ชื่อ Key ในวงเล็บต้องตรงกับที่ Set ไว้ตอน Login
    var userIdStr = HttpContext.Session.GetString("UserId"); 

    if (string.IsNullOrEmpty(userIdStr)) 
    {
        // ถ้ามันวิ่งมาบรรทัดนี้ แสดงว่าดึงค่า "UserId" ไม่ได้ มันเลยส่งคุณไปหน้า Login
        return RedirectToAction("Login"); 
    }

    int userId = int.Parse(userIdStr);
    var user = await _context.Users.FindAsync(userId);
    var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

    ViewBag.Profile = profile;
    return View(user);
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