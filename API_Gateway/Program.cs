using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using MMLib.SwaggerForOcelot.DependencyInjection;
using MMLib.SwaggerForOcelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 2️⃣ SwaggerForOcelot với HTTPS self-signed bypass
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddHttpClient("SwaggerForOcelot")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

// 3️⃣ JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWTsetting");
var secretKey = jwtSettings["SecretKey"] ?? "MUONBIETMEONHIEUHON32KYTUVWXYZABCDEF";
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

// 4️⃣ Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Middleware
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Swagger UI
app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});

// ===== Auto start APIs (Dev only) =====
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

        Console.WriteLine($"Starting {api.Name} on port {api.Port}...");

        // ===== Wait until API is ready =====
        var isReady = false;
        using var client = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
        for (int i = 0; i < 20; i++) // try 20 lần, 1s delay each
        {
            try
            {
                var response = client.GetAsync($"https://localhost:{api.Port}/swagger/v1/swagger.json").Result;
                if (response.IsSuccessStatusCode)
                {
                    isReady = true;
                    break;
                }
            }
            catch { }
            await Task.Delay(1000);
        }

        if (!isReady)
            Console.WriteLine($"{api.Name} not ready after 20s");
        else
            Console.WriteLine($"{api.Name} is ready!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to start {api.Name}: {ex.Message}");
    }
}

// 5️⃣ Run Ocelot Gateway
await app.UseOcelot();

app.Run();
