using Microsoft.EntityFrameworkCore;
using BeverageShop.API.Data;
using BeverageShop.API.Models;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Add DbContext
    builder.Services.AddDbContext<BeverageShopDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add Memory Cache
    builder.Services.AddMemoryCache();

    // Add Response Caching
    builder.Services.AddResponseCaching();

    // Add Response Compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddSignalR();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERROR during service configuration: {ex.Message}");
    Console.WriteLine($"Stack: {ex.StackTrace}");
    throw;
}

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow MVC project to access API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.WithOrigins("http://localhost:5001", "https://localhost:7001", "http://localhost:5002")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});

var app = builder.Build();

// Enable static files for uploaded images
app.UseStaticFiles();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Beverage Shop API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

// app.UseHttpsRedirection(); // Disabled for HTTP testing

// Use Response Compression (must be before static files)
app.UseResponseCompression();

// Use Response Caching
app.UseResponseCaching();

app.UseCors("AllowMVC");

app.UseAuthorization();

app.MapControllers();
app.MapHub<BeverageShop.API.Hubs.ChatHub>("/chathub");

Console.WriteLine("API is running...");
Console.WriteLine("Test API at: https://localhost:7000/api/beverages");

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BeverageShopDbContext>();
    
    // Ensure database is created
    context.Database.EnsureCreated();
    
    // Seed data if empty
    if (!context.Beverages.Any())
    {
        Console.WriteLine("Seeding initial data...");
        
        // Seed Beverages
        context.Beverages.AddRange(
            new Beverage { Name = "Trà Sữa Trân Châu", Type = "Trà sữa", Brand = "", Category = "Trà", Price = 45000, Size = "M", Description = "Trà sữa trân châu đường đen thơm ngon", ImageUrl = "https://images.unsplash.com/photo-1525385133512-2f3bdd039054?w=400", Stock = 50, IsAvailable = true, CreatedDate = DateTime.Now },
            new Beverage { Name = "Cà Phê Đen Đá", Type = "Cà phê đen", Brand = "", Category = "Cà phê", Price = 25000, Size = "M", Description = "Cà phê đen nguyên chất đậm đà", ImageUrl = "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=400", Stock = 100, IsAvailable = true, CreatedDate = DateTime.Now },
            new Beverage { Name = "Nước Ép Cam", Type = "Nước ép", Brand = "", Category = "Nước ép", Price = 35000, Size = "L", Description = "Nước ép cam tươi 100%", ImageUrl = "https://images.unsplash.com/photo-1600271886742-f049cd451bba?w=400", Stock = 30, IsAvailable = true, CreatedDate = DateTime.Now },
            new Beverage { Name = "Sinh Tố Bơ", Type = "Sinh tố", Brand = "", Category = "Sinh tố", Price = 40000, Size = "L", Description = "Sinh tố bơ béo ngậy bổ dưỡng", ImageUrl = "https://images.unsplash.com/photo-1623065422902-30a2d299bbe4?w=400", Stock = 25, IsAvailable = true, CreatedDate = DateTime.Now },
            new Beverage { Name = "Trà Đào Cam Sả", Type = "Trà trái cây", Brand = "", Category = "Trà", Price = 50000, Size = "L", Description = "Trà đào cam sả thanh mát", ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400", Stock = 40, IsAvailable = true, CreatedDate = DateTime.Now },
            new Beverage { Name = "Soda Chanh Dây", Type = "Soda", Brand = "", Category = "Soda", Price = 30000, Size = "M", Description = "Soda chanh dây tươi mát", ImageUrl = "https://images.unsplash.com/photo-1546173159-315724a31696?w=400", Stock = 60, IsAvailable = true, CreatedDate = DateTime.Now }
        );
        
        // Seed Admin User
        context.Users.Add(new User
        {
            Username = "admin",
            Email = "admin@beverageshop.com",
            PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("admin123"))),
            FullName = "Administrator",
            Phone = "0123456789",
            Address = "Hà Nội",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedDate = DateTime.Now
        });
        
        context.SaveChanges();
        Console.WriteLine("✅ Data seeded successfully!");
    }
    else
    {
        Console.WriteLine($"✅ Database already has {context.Beverages.Count()} beverages and {context.Users.Count()} users");
    }
}

app.Run();
