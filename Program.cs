using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using api1.Service;
using api1.Secruity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;




var builder = WebApplication.CreateBuilder(args);

// 設定 ConfigurationBuilder
var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = configurationBuilder.Build();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireRole("admin");
    });

    options.AddPolicy("RequireRenterRole", policy =>
    {
        policy.RequireRole("renter");
    });

    options.AddPolicy("RequirePublisherRole", policy =>
    {
        policy.RequireRole("publisher");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero,
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async ctx =>
        {
            // 获取用户角色声明
            var roleClaim = ctx.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleClaim))
            {
                ctx.Fail("Unauthorized");
            }

            // 如果用户不是管理员、房东或租户，则返回未授权错误
            if (roleClaim != "admin" && roleClaim != "publisher" && roleClaim != "renter")
            {
                ctx.Fail("Unauthorized");
            }

            await Task.CompletedTask;
        }
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 設定連線字串 //註冊 IDbConnection
// var connectionString = configuration.GetConnectionString("DB");
// builder.Services.AddSingleton<IDbConnection>(new SqlConnection(connectionString));
//這裡註冊一個實現 IDbConnection 接口的 connectionString 類
//作用愈
var connectionString = configuration.GetConnectionString("DB");
builder.Services.AddSingleton<SqlConnection>(_ => new SqlConnection(connectionString));

builder.Services.AddSingleton<MembersDBService>();
builder.Services.AddSingleton<MailService>();
//builder.Services.AddSingleton<RentalService>();
builder.Services.AddSingleton<ForPaging>();
builder.Services.AddSingleton<HomeDBService>();
builder.Services.AddSingleton<HomeAnyDBService>();
builder.Services.AddSingleton<ReportDBService>();
builder.Services.AddSingleton<HomeDetailDBService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();//新增

app.MapControllers();

app.Run();
