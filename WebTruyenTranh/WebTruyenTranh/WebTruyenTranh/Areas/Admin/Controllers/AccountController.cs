using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebTruyenTranh.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebTruyenTranh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous] // Cho phép chưa đăng nhập truy cập vào AccountController
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public AccountController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            var user = _context.Account
                        .FirstOrDefault(x => x.Username != null && x.Password != null && x.Username == username && x.Password == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role) // chú ý role đúng theo db
                };

                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(300)
                };

                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Manga", new { area = "Admin" });
                else
                    return RedirectToAction("Index", "Home", new { area = "" });
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu.";
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }
    }
}
