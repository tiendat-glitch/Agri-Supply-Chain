    using API_Farmer.Settings;
    using BLL;
    using DAL.Helper;
    using DAL.Repositories;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using System.Text;

    var builder = WebApplication.CreateBuilder(args);

    // Controllers
    builder.Services.AddControllers();

    // Database + Repository + BLL
    var cs = builder.Configuration.GetConnectionString("DefaultConnection")
             ?? throw new Exception("Missing DefaultConnection in API_Farmer");

    builder.Services.AddSingleton(new DatabaseHelper(cs));

    // Repository
    builder.Services.AddScoped<ProductRepository>();
    builder.Services.AddScoped<FarmRepository>();
    builder.Services.AddScoped<BatchRepository>();  
    builder.Services.AddScoped<AuditLogRepository>();
    builder.Services.AddScoped<WarehouseRepository>();
    builder.Services.AddScoped<ShipmentRepository>();
    builder.Services.AddScoped<InspectionRepository>();

    // BLL
    builder.Services.AddScoped<bll_Product>();
    builder.Services.AddScoped<FarmBusiness>();
    builder.Services.AddScoped<BatchBusiness>();   
    builder.Services.AddScoped<AuditLogBusiness>();
    builder.Services.AddScoped<WarehouseBusiness>();
    builder.Services.AddScoped<ShipmentBusiness>();
    builder.Services.AddScoped<InspectionBusiness>();

    // JWT Authentication
    builder.Services.Configure<JWTsetting>(builder.Configuration.GetSection("JWTsetting"));
    var jwt = builder.Configuration.GetSection("JWTsetting").Get<JWTsetting>()
              ?? throw new Exception("JWTsetting chưa cấu hình đúng");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
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

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1",
            new OpenApiInfo { Title = "API_Farmer", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Nhập: Bearer {token}"
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

    // Build App
    var app = builder.Build();

    // Middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();