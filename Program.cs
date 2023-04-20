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

// builder.Services.AddAuthorization(options =>
// {
//     options.FallbackPolicy = new AuthorizationPolicyBuilder()
//         .RequireAuthenticatedUser()
//         .Build();

//     options.AddPolicy("RequireAdminRole", policy =>
//     {
//         policy.RequireRole("admin");
//     });

//     options.AddPolicy("RequireRenterRole", policy =>
//     {
//         policy.RequireRole("renter");
//     });

//     options.AddPolicy("RequirePublisherRole", policy =>
//     {
//         policy.RequireRole("publisher");
//     });
// });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddAuthentication(options =>
{
    //JWT身分驗證方案的預設名稱

    //當API收到請求時，ASP.NET Core將自動使用JWT身分驗證方案對請求進行身分驗證
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //自動使用JWT身分驗證方案來回應401
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //設定驗證參數
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //驗證JWT令牌的簽章是否正確
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
        ValidateIssuer = true,//驗證發行者
        ValidIssuer = configuration["Jwt:Issuer"],//預期的發行者
        ValidateAudience = true,//驗證接收者
        ValidAudience = configuration["Jwt:Audience"],//預期的發行者預期接收者
        ClockSkew = TimeSpan.Zero,//時效性
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async ctx => //觸發事件自訂邏輯
        {
            // 獲取角色
            var roleClaim = ctx.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleClaim))
            {
                ctx.Fail("Unauthorized");
            }
            // 身分判斷
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


// 設定CORS
app.UseCors(builder => builder
    .WithOrigins("http://127.0.0.1:5555") // 允許的源網址
    .AllowAnyMethod() // 允許任何HTTP方法
    .AllowAnyHeader() // 允許任何標頭
    .AllowCredentials()); // 允許傳送身分驗證cookie

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();//新增

app.MapControllers();

app.Run();
