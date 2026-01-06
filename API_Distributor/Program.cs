using API_Distributor.Settings;
using BLL;
using DAL.Helper;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Controllers
// =======================
builder.Services.AddControllers();

// =======================
// Database + Repository + BLL
// =======================
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? throw new Exception("Missing DefaultConnection in API_Adm");

// DatabaseHelper
builder.Services.AddSingleton(new DatabaseHelper(cs));

// Repositories
builder.Services.AddScoped<BatchRepository>();
builder.Services.AddScoped<ShipmentRepository>();
builder.Services.AddScoped<WarehouseRepository>();
builder.Services.AddScoped<WarehouseStockRepository>();
builder.Services.AddScoped<ShipmentItemRepository>();
// BLL
builder.Services.AddScoped<BatchBusiness>();
builder.Services.AddScoped<ShipmentBusiness>();
builder.Services.AddScoped<WarehouseBusiness>();


// =======================
// JWT Authentication
// =======================
builder.Services.Configure<JWTsetting>(builder.Configuration.GetSection("JWTsetting"));
var jwt = builder.Configuration.GetSection("JWTsetting").Get<JWTsetting>()
          ?? throw new Exception("JWTsetting ch?a ???c c?u hình ?úng");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // ch? dev
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// =======================
// Swagger
// =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "API_Distributor", Version = "v1", Description = "Qu?n lý Admin" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nh?p: Bearer {token}"
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
            Array.Empty<string>()
        }
    });
});

// =======================
// Build App
// =======================
var app = builder.Build();

// =======================
// Middleware
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_Distributor v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
