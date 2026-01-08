using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Diagnostics;
using System.Text;
using MMLib.SwaggerForOcelot.DependencyInjection;
using MMLib.SwaggerForOcelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 2. SwaggerForOcelot
builder.Services.AddSwaggerForOcelot(builder.Configuration);

// 3. JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWTsetting");
var secretKey = jwtSettings["SecretKey"]
    ?? "MUONBIETMEONHIEUHON32KYTUVWXYZABCDEF";

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 4. Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// Build App
var app = builder.Build();

// Middleware pipeline (CHUẨN)

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

<<<<<<< Updated upstream
// Controllers riêng nếu có
app.MapControllers();
// ===== Redirect endpoints để dễ truy cập Swagger cho từng role =====
app.MapGet("/auth", async ctx => ctx.Response.Redirect("/auth/swagger", false));
app.MapGet("/admin", async ctx => ctx.Response.Redirect("/admin/swagger", false));
app.MapGet("/farmer", async ctx => ctx.Response.Redirect("/farmer/swagger", false));
app.MapGet("/distributor", async ctx => ctx.Response.Redirect("/distributor/swagger", false));
app.MapGet("/retailer", async ctx => ctx.Response.Redirect("/retailer/swagger", false));
=======
// Redirect tiện lợi
app.MapGet("/auth", ctx =>
{
    ctx.Response.Redirect("/auth/swagger", false);
    return Task.CompletedTask;
});
app.MapGet("/admin", ctx =>
{
    ctx.Response.Redirect("/admin/swagger", false);
    return Task.CompletedTask;
});
app.MapGet("/farmer", ctx =>
{
    ctx.Response.Redirect("/farmer/swagger", false);
    return Task.CompletedTask;
});
app.MapGet("/distributor", ctx =>
{
    ctx.Response.Redirect("/distributor/swagger", false);
    return Task.CompletedTask;
});
app.MapGet("/retailer", ctx =>
{
    ctx.Response.Redirect("/retailer/swagger", false);
    return Task.CompletedTask;
});

// SwaggerForOcelot UI
>>>>>>> Stashed changes

app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});

// Auto start APIs (DEV ONLY)
var apis = new[]
{
    new { Name = "API_Auth", Path = "../API_Auth/API_Auth.csproj", Port = 7246 },
    new { Name = "API_Admin", Path = "../API_Adm/API_Adm.csproj", Port = 7079 },
    new { Name = "API_Farmer", Path = "../API_Farmer/API_Farmer.csproj", Port = 7200 },
    new { Name = "API_Distributor", Path = "../API_Distributor/API_Distributor.csproj", Port = 7035 },
    new { Name = "API_Retailer", Path = "../API_Retailer/API_Retailer.csproj", Port = 7070 }
};

foreach (var api in apis)
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project {api.Path} --urls https://localhost:{api.Port}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        });
        Console.WriteLine($"Started {api.Name} on port {api.Port}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to start {api.Name}: {ex.Message}");
    }
}

// Ocelot Gateway
await app.UseOcelot();

app.Run();
