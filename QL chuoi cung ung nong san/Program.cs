using API_Auth.Services;
using API_Auth.Settings;
using BLL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
// ------------------- Add Services -------------------

// Controllers
builder.Services.AddControllers();

// Bind JWT settings từ appsettings.json và register vào DI
builder.Services.Configure<JWTsetting>(builder.Configuration.GetSection("JWTsetting"));

// JWT service
builder.Services.AddScoped<JwtTokenService>();

// Business layer
builder.Services.AddScoped<UserBusiness>();

// Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_Auth", Version = "v1" });

    // JWT Bearer setup
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// ------------------- JWT Authentication -------------------

// Lấy config để cấu hình middleware
var jwtConfig = builder.Configuration.GetSection("JWTsetting").Get<JWTsetting>();
if (jwtConfig == null || string.IsNullOrEmpty(jwtConfig.Key))
    throw new Exception("JWT Key is null! Check appsettings.json");

var key = Encoding.UTF8.GetBytes(jwtConfig.Key);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        // Debug event
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT failed: {context.Exception}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT validated!");
                foreach (var claim in context.Principal.Claims)
                    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                return Task.CompletedTask;
            }
        };
    });

// ------------------- Build app -------------------
var app = builder.Build();
app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");

    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception middleware caught: {ex}");
        throw;
    }

    Console.WriteLine($"Outgoing response: {context.Response.StatusCode}");
});


// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_Auth v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
