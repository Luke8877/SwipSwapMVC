using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SwipSwapMVC.Data;
using SwipSwapMVC.Models;
//using SwipSwapAuth.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SwipSwapMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;

        public AccountController(AppDbContext context,
                                 IPasswordHasher<User> passwordHasher,
                                 IConfiguration config)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        // ---------------------------
        // REGISTER
        // ---------------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(model);
            }

            model.PasswordHash = _passwordHasher.HashPassword(model, model.Password);
            model.DateCreated = DateTime.Now;

            _context.Users.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // ---------------------------
        // LOGIN
        // ---------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User model)
        {
            // Treat the input field as either username OR email
            string loginInput = model.Username;

            if (string.IsNullOrWhiteSpace(loginInput) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Please enter both username/email and password.");
                return View(model);
            }

            // Find user by username OR email (SYNC)
            var user = _context.Users
                .FirstOrDefault(u => u.Username == loginInput || u.Email == loginInput);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username/email or password.");
                return View(model);
            }

            // Verify password (SYNC)
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                model.Password   // raw input password from the form
            );

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid username/email or password.");
                return View(model);
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Optional: store token (not required unless you're using JWT in UI)
            ViewBag.Token = token;

            Response.Cookies.Append("MyListingsJwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return RedirectToAction("Index", "Dashboard");
        }


        // ---------------------------
        // FORGOT PASSWORD
        // ---------------------------
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(User model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Email not found.");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "Please enter a new password.");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return View(model);
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            _context.SaveChanges();

            ViewBag.Message = "Password reset successfully.";
            return View(new User());
        }



        // ---------------------------
        // GENERATE JWT TOKEN
        // ---------------------------
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.UserId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
