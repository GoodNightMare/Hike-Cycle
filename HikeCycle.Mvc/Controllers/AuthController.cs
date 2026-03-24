using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.Models.db;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;

namespace HikeCycle.Mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly HikeCycledbContext _context;

        public AuthController(HikeCycledbContext context)
        {
            _context = context;
        }

        // --- Helper Function สำหรับ Hash รหัสผ่าน ---
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // แปลง password เป็น byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // แปลง byte array เป็น string เลขฐาน 16 (Hexadecimal)
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. ตรวจสอบว่า Email ซ้ำหรือไม่
            // แบบใหม่ (สะอาดกว่าและ Error น้อยกว่า)
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email นี้ถูกใช้งานแล้วในระบบ" });
            }

            // 2. สร้าง User ใหม่ (บันทึกรหัสผ่านที่ Hash แล้ว)
            var newUser = new User
            {
                Email = request.Email,
                Password = HashPassword(request.Password), // <--- ใช้ Hash ตรงนี้
                Role = "user",
                CreatedAt = DateTime.Now
            };

            _context.Set<User>().Add(newUser);
            await _context.SaveChangesAsync();

            // 3. สร้าง Profile ใหม่ (ตาราง user_profiles)
            var newProfile = new UserProfile
            {
                UserId = newUser.Id,
                FullName = request.FullName,
                Phone = request.Phone,
                Address = request.Address
            };

            _context.UserProfiles.Add(newProfile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "สมัครสมาชิกสำเร็จ" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. แปลงรหัสผ่านที่ส่งมาจากหน้าบ้านเป็น Hash เพื่อนำไปเทียบใน DB
            string hashedInput = HashPassword(request.Password);

            // 2. ค้นหา User โดยใช้ Email และ Hashed Password
            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == hashedInput);

            if (user == null)
            {
                return Unauthorized(new { message = "Email หรือ Password ไม่ถูกต้อง" });
            }

            // 3. ดึงชื่อจาก Profile
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            // 4. ส่งข้อมูลกลับไปให้ React
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                role = user.Role,
                fullName = profile?.FullName ?? "ผู้ใช้งานทั่วไป"
            });
        }

        // GET: api/auth/profile/1
        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return NotFound();

            return Ok(profile);
        }

        // PUT: api/auth/profile/update
        [HttpPut("profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == request.UserId);
            if (profile == null) return NotFound();

            // อัปเดตข้อมูล
            profile.FullName = request.FullName;
            profile.Phone = request.Phone;
            profile.Address = request.Address;

            await _context.SaveChangesAsync();
            return Ok(new { message = "อัปเดตสำเร็จ" });
        }

    }


    // --- DTO Classes (Data Transfer Objects) ---

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateProfileRequest
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}