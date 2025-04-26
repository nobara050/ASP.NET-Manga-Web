using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebTruyenTranh.Data;

namespace WebTruyenTranh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        //private readonly SignInManager<IdentityUser> _signInManager;
        //private readonly UserManager<IdentityUser> _userManager;
        //public UserController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        //{
        //    _signInManager = signInManager;
        //    _userManager = userManager;
        //}

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public AccountController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        //public IActionResult Login()
        //{
        //    return View();
        //}

        //// Đăng nhập
        //[HttpPost]
        //public async Task<IActionResult> Login(string username, string password)
        //{
        //    var user = await _userManager.FindByNameAsync(username);
        //    if (user != null)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {
        //            // Redirect to Admin area if user is in role "admin"
        //            if (await _userManager.IsInRoleAsync(user, "admin"))
        //                return RedirectToAction("Index", "Manga", new { area = "Admin" });
        //            else
        //                return RedirectToAction("Index", "Home");
        //        }
        //    }

        //    ModelState.AddModelError("", "Invalid login attempt");
        //    return View();
        //}

    }
}
