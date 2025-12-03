using BLL;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Lấy connection string từ appsettings.json ---
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// --- 2. Đăng ký BLL vào DI ---
builder.Services.AddSingleton(new ProductBusiness(connectionString));

// --- 3. Thêm controller và Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 4. CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7111",
                "http://127.0.0.1:5501",
                "https://localhost:7028")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// --- 5. Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Swagger ở /
    });

    // --- Mở Swagger tự động ---
    var swaggerUrl = "https://localhost:7246"; // Thay port đúng terminal hiển thị
    try
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = swaggerUrl,
            UseShellExecute = true
        });
    }
    catch { }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// --- 6. Map controllers ---
app.MapControllers();

// --- 7. Run ứng dụng ---
app.Run();
