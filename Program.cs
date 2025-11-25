using Microsoft.EntityFrameworkCore;
using SwipSwapMVC.Data;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Register MVC controllers + Razor views
// ------------------------------------------------------------
builder.Services.AddControllersWithViews();

// ------------------------------------------------------------
// Register EF Core with SQL Server
// ------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ------------------------------------------------------------
// Configure Stripe API (server-side secret key)
// This must be set BEFORE Stripe services are used.
// ------------------------------------------------------------
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// ------------------------------------------------------------
// Error handling + security headers for production
// ------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ------------------------------------------------------------
// HTTP pipeline configuration
// ------------------------------------------------------------
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// NOTE:
// Authentication would go here later: app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------------
// Enable attribute-routed controllers (needed for webhook endpoint)
// e.g., [Route("stripe/webhook")]
// ------------------------------------------------------------
app.MapControllers();

// ------------------------------------------------------------
// Default MVC route for pages
// ------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
