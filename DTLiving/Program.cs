using DTLiving.Context;
using DTLiving.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DTContext>(options
    => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your_issuer",
            ValidAudience = "your_audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"))
        };
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseDefaultFiles(); // 進入靜態檔案

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 在這裡添加自動創建員工的代碼
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DTContext>();

        // 檢查是否已有員工記錄，如果沒有則創建一位員工
        if (!context.Employer.Any())
        {
            var newEmployee = new Employer
            {
                StaffId = "81100000",
                Gender = "Male",
                ClerkName = "Administrator",
                born = "1997/01/01",
                ClerkPhone = "0978635734",
                ClerkAddress = "新竹縣湖口鄉中正路2段263巷26號",
                SetupTime = DateTime.Now
            };

            context.Add(newEmployee);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        // 處理例外情況
        Console.WriteLine("An error occurred while seeding the database: " + ex.Message);
    }
}

app.Run();
