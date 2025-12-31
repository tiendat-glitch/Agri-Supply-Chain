using BLL;

var builder = WebApplication.CreateBuilder(args);

// 1. DI
builder.Services.AddScoped<ProductBusiness>();
builder.Services.AddScoped<UserBusiness>();

// 2. Controller + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5501",
                "http://127.0.0.1:5501")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// 4. Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 5. Middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// 6. Controllers
app.MapControllers();

// 7. Run app
app.Run();