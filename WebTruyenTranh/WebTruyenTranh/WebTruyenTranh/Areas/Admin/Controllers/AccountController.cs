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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            var user = _context.Account
                        .FirstOrDefault(x => x.Username != null && x.Password != null && x.Username == username && x.Password == password);

            if (user != null)
            {
                // Kiểm tra nếu là admin thì mới cho phép đăng nhập
                if (user.Role != "Admin")
                {
                    ViewBag.Error = "Không đủ quyền truy cập.";
                    return View();
                }

                // Tạo claims cho admin
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "AdminCookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(300)
                };

                // Đăng nhập với AdminCookieAuth
                await HttpContext.SignInAsync("AdminCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                // Lưu thông tin user vào session
                HttpContext.Session.SetString("UserId", user.AccountId.ToString());

                // Redirect đến returnUrl nếu có, hoặc về trang admin nếu không có
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Manga", new { area = "Admin" });
            }

            // Nếu không tìm thấy user hoặc mật khẩu sai
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu.";
            return View();
        }


        [Authorize(AuthenticationSchemes = "AdminCookieAuth", Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("AdminCookieAuth");
            await HttpContext.SignOutAsync("UserCookieAuth");
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }
    }
}
