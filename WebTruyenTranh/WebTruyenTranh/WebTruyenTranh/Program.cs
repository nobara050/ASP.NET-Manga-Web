using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebTruyenTranh.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDB"]);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "CombinedAuth";
    options.DefaultChallengeScheme = "CombinedAuth";
})
.AddPolicyScheme("CombinedAuth", "Combined Auth", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        // nếu có cookie Admin thì ưu tiên Admin
        if (context.Request.Cookies.ContainsKey("AdminCookieAuth"))
            return "AdminCookieAuth";
        // ngược lại dùng cookie User
        return "UserCookieAuth";
    };
})
.AddCookie("UserCookieAuth", options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
})
.AddCookie("AdminCookieAuth", options =>
{
    options.LoginPath = "/Admin/Account/Login";
    options.AccessDeniedPath = "/Admin/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

builder.Services.AddDistributedMemoryCache(); // Cần thiết cho session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian hết hạn
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area}/{controller=Manga}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
