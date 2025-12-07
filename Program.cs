using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using SwipSwapMVC.Data;
using SwipSwapMVC.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// MVC
// ------------------------------------------------------------
builder.Services.AddControllersWithViews();

// ------------------------------------------------------------
// Database - EF Core SQL Server
// ------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ------------------------------------------------------------
// Password Hasher (supports login functionality coming later)
// ------------------------------------------------------------
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ------------------------------------------------------------
// JWT Authentication for user dashboard + account pages
// ------------------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("MyListingsScheme", options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["MyListingsJwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };

    options.RequireHttpsMetadata = false; // For development only
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ------------------------------------------------------------
// Stripe API initialization
// ------------------------------------------------------------
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


var app = builder.Build();

// ------------------------------------------------------------
// Production error & security pipeline
// ------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ------------------------------------------------------------
// Middleware pipeline
// ------------------------------------------------------------
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // <–– enabled for login + JWT support
app.UseAuthorization();

// ------------------------------------------------------------
// Route registrations
// ------------------------------------------------------------
app.MapControllers(); // enables webhook: [Route("stripe/webhook")]

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
