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
            // 从 appsettings.json 文件中获取 JWT 配置信息
            var secretKey = _config["Jwt:SecretKey"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireTimeInMinutes = int.Parse(_config["Jwt:ExpireTimeInMinutes"]);


            // 创建一个 ClaimsIdentity 对象，用于描述 JWT 中包含的声明信息
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, Account),
                new Claim(ClaimTypes.Role, role)
            });

            // 创建一个 SymmetricSecurityKey 对象，用于对 JWT 的签名进行验证
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // 创建一个 SigningCredentials 对象，用于指定 JWT 的签名算法和密钥
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 创建一个 JwtSecurityToken 对象，并设置其中包含的声明信息、签名凭证、发布者、受众者和过期时间
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claimsIdentity.Claims,
                expires: DateTime.UtcNow.AddMinutes(expireTimeInMinutes),
                signingCredentials: signingCredentials
            );
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            // 生成 JWT 字符串，并返回
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