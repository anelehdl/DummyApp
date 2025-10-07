using Microsoft.AspNetCore.Authentication.Cookies;
using Dashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC for Dashboard
builder.Services.AddControllersWithViews();

// Register the Dashboard Auth Service
builder.Services.AddScoped<DashboardAuthService>();

// Cookie Authentication for Dashboard
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Dashboard/Index";
        options.LogoutPath = "/Dashboard/Logout";
        options.AccessDeniedPath = "/Dashboard/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// Add HttpClient to call API
builder.Services.AddHttpClient("DummyAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7218");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();