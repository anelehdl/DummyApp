using Microsoft.AspNetCore.Authentication.Cookies;
using DummyApp.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add MVC for Dashboard
builder.Services.AddControllersWithViews();

// Add Infrastructure services (same MongoDB, business services)
builder.Services.AddInfrastructure(builder.Configuration);

// Cookie Authentication for Dashboard
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Index";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// Add HttpClient if Dashboard needs to call API
builder.Services.AddHttpClient("DummyAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
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