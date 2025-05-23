using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using Bookit.Data;
using Bookit.Services;
using Bookit.Helpers;
using Bookit.Repositories;
using Bookit.Service;
using Bookit.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(connectionString),
    ServiceLifetime.Scoped);
    
// Configure Authentication & Authorization
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey) || Encoding.UTF8.GetBytes(jwtKey).Length < 32)
{
    throw new Exception("JWT Key is missing or too short! It must be at least 256 bits (32 characters).");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // 👇 ADD THIS to support JWT over WebSocket (SignalR)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/chathub")) // change this if your hub path is different
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Change this to your frontend URL
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials(); // Important for JWT authentication
        });
});
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();


// Register Authorization
builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<LibrarianServices>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<BorrowedBooksRepository>();

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();


// Configure Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware Order Matters!
// app.UseHttpsRedirection();
app.UseRouting();

// Use CORS before Authentication & Authorization
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/Chathub");
app.Run();
