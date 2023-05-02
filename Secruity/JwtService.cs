using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace api1.Secruity
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this._config = configuration;
            _httpContextAccessor = httpContextAccessor;

        }
        //舊的別看
        public string GenerateToken(string Account, string Role)
        {
            JwtObject jwtObject = new JwtObject
            {
                Account = Account,
                Role = Role,
                Expire = DateTime.Now.AddMinutes(Convert.ToInt32(_config["AppSettings:ExpireMin"])).ToString()
            };
            var paload = jwtObject;
            var header = new { alg = "HS256", typ = "JWT" };

            var headerJson = JsonSerializer.Serialize(header);
            var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerJson));
            var paloadJson = JsonSerializer.Serialize(paload);
            var paloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(paloadJson));

            // var paloadJson=JsonConvert.SerializeObject(paload);
            // var paloadBase64=Convert.ToBase64String(Encoding.UTF8.GetBytes(paloadJson));

            var secretKey = _config["AppSettings:SecretKey"];
            var signature = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerBase64 + paloadBase64 + secretKey));

            var token = headerBase64 + paloadBase64 + signature;
            return token;
        }
        //產生JWT
        public string GenerateJwtToken(string Account,string role)
        {
            // 從 appsettings.json 設定配置
            var secretKey = _config["Jwt:SecretKey"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireTimeInMinutes = int.Parse(_config["Jwt:ExpireTimeInMinutes"]);

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, Account),
                new Claim(ClaimTypes.Role, role)
            });

            // 創 SymmetricSecurityKey ，對JWT進行編碼驗證
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // 創 SigningCredentials ，指定type
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 創 JwtSecurityToken，設定角色、簽章、發布者、接收者等
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claimsIdentity.Claims,
                expires: DateTime.UtcNow.AddMinutes(expireTimeInMinutes),
                signingCredentials: signingCredentials
            );
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //解析JWT
        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            try
            {
                var secretKey = _config["Jwt:SecretKey"];
                var issuer = _config["Jwt:Issuer"];
                var audience = _config["Jwt:Audience"];

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuerSigningKey = true
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                // 將 ClaimsPrincipal 對象分配給 IHttpContextAccessor 實例中的 HttpContext.User 屬性
                _httpContextAccessor.HttpContext.User = principal;
                return principal;

            }
            catch
            {
                return null;
            }
        }

    }
}