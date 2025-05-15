using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebTruyenTranh.Data;

namespace WebTruyenTranh.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            var user = _context.Account
                .FirstOrDefault(x => x.Username == username && x.Password == password);

            if (user != null)
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                // Phân biệt scheme
                var scheme = user.Role == "Admin" ? "AdminCookieAuth" : "UserCookieAuth";

                var identity = new ClaimsIdentity(claims, scheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(300)
                };

                await HttpContext.SignInAsync(scheme, principal, props);

                // Lưu thông tin user vào session
                HttpContext.Session.SetString("UserId", user.AccountId.ToString());

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Manga", new { area = "Admin" });

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu.";
            return View();
        }

        [Authorize(AuthenticationSchemes = "UserCookieAuth,AdminCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("UserCookieAuth");
            await HttpContext.SignOutAsync("AdminCookieAuth");
            return RedirectToAction("Login");
        }

    }
}
