using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication4.Data;
using WebApplication4.Services;

var builder = WebApplication.CreateBuilder(args);

// 👉 1. Add DbContext (MySQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// 👉 2. Cấu hình CORS - THÊM MỚI
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // React app URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// 👉 3. Cấu hình JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

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
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 👉 4. Add Services
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 👉 5. Configure Pipeline
app.UseSwagger();
app.UseSwaggerUI();

// ⚠️ QUAN TRỌNG: UseCors() phải đặt trước UseAuthentication()
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();