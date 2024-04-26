using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using panasonic_machine_checker.data;
using System.Security.Cryptography;


namespace panasonic_machine_checker.Controllers
{
    public class AuthController : Controller
    {

        private DbPanasonicContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, DbPanasonicContext context)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet("/Login")]
        public IActionResult Login()
        {
            return View();
        }

        public class LoginCredentials
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("/DoLogin")]
        public JsonResult DoLogin([FromBody] LoginCredentials credentials)
        {
            var user = _context.Users.Include(u => u.RoleNavigation).First(u => u.Email == credentials.Email);
            if (user != null)
            {
                var userData = _context.Users.FirstOrDefault(u => u.Email == credentials.Email);
                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, credentials.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    var token = GenerateJwtToken(user);
                    return Json(new { success = true, message = "Login successful.", token = token, user = user.RoleNavigation?.Name, user_data = user.Id });
                }
                return Json(new { success = false, message = "Invalid Password." });
            }
            return Json(new { success = false, message = "Invalid Email.", user_email = credentials.Email });
        }

        public static string GenerateSecureSecret()
        {
            var randomBytes = new byte[32]; // 256 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        private string GenerateJwtToken(User user)
        {
            // Ensure your key is long enough and securely stored
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyHereWithSufficientLength"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Role", user.RoleNavigation?.Name ?? "NoRole")
            };

            var token = new JwtSecurityToken(
                issuer: "YourIssuer",
                audience: "YourAudience",
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
