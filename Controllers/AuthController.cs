using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using api1.ViewModel;
using api1.Service;
using api1.Secruity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using api1.Models;
using System.Web;

namespace api1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MembersDBService _membersSerivce;
        private readonly JwtService _jwtService;
        private readonly MailService _mailService;
        private readonly IWebHostEnvironment _env;


        public AuthController(IConfiguration configuration, MembersDBService membersDBService, JwtService jwtService, MailService mailService, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _membersSerivce = membersDBService;
            _jwtService = jwtService;
            _mailService = mailService;
            _env = env;

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] MembersRegisterViewModel registerMember)
        {
            if (ModelState.IsValid)
            {
                string accountExists = _membersSerivce.AccountCheck(registerMember.newMember.account);
                if (string.IsNullOrWhiteSpace(accountExists))
                {
                    // Save profile image
                    if (registerMember.ProfileImage != null && registerMember.ProfileImage.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(registerMember.ProfileImage.FileName);
                        var path = Path.Combine("MembersImg", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await registerMember.ProfileImage.CopyToAsync(stream);
                        }
                        //registerMember.newMember.img = path;
                        registerMember.newMember.img = fileName;
                    }
                    else
                    {
                        return BadRequest(new { message = "請上傳照片" });
                    }
                    registerMember.newMember.password = registerMember.password;
                    string authCode = _mailService.GetValidateCode();
                    registerMember.newMember.authcode = authCode;
                    _membersSerivce.Register(registerMember.newMember);
                    string filePath = @"Views/RegisterEmail.html";
                    string TempString = System.IO.File.ReadAllText(filePath);
                    var scheme = Request.Scheme;
                    //var host = Request.Host.ToUriComponent();
                    var host="127.0.0.1:5555";
                    var pathBase = Request.PathBase.ToUriComponent();
                    var controller = "Members";
                    var action = "EmailValidate";
                    var account = HttpUtility.UrlEncode(registerMember.newMember.account);
                    var authCode2 = HttpUtility.UrlEncode(registerMember.newMember.authcode);

                    var url = $"{scheme}://{host}{pathBase}/{controller}/{action}?Account={account}&AuthCode={authCode2}";

                    string mailBody = _mailService.GetRegisterMailBody(TempString, registerMember.newMember.name, url.ToString());
                    _mailService.SendRegisterMail(mailBody, registerMember.newMember.email);
                    return Ok(new { message = "註冊成功，請去收信以來驗證EMAIL" });
                }
                else
                {
                    return BadRequest("已被註冊");
                }
            }
            registerMember.password = null;
            registerMember.passwordCheck = null;
            return BadRequest(registerMember);
        }

        [AllowAnonymous]
        [HttpGet("emailValidate")]
        public IActionResult EmailValidate([FromQuery] EmailValidate Data)
        {
            string EmailValidateStr = _membersSerivce.EmailValidate(Data.account, Data.authcode);
            return Ok(EmailValidateStr);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel Data)
        {
            string Validate = _membersSerivce.LoginCheck(Data.Account, Data.Password);
            if (Validate == "")
            {
                string Role = _membersSerivce.GetRole(Data.Account);
                var token = _jwtService.GenerateJwtToken(Data.Account, Role);
                Response.Cookies.Append("jwtToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                });
                Members members = _membersSerivce.GetDataByAccount(Data.Account);
                //base64
                // string imagePath = members.img;
                // string imageFullPath = Path.Combine("MembersImg", imagePath);
                // byte[] imageBytes = System.IO.File.ReadAllBytes(imageFullPath);
                // string base64String = Convert.ToBase64String(imageBytes);
                //members.img=base64String;
                members.password=null;
                return Ok(new { token, members });
            }
            else
            {
                return BadRequest(Validate);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("請登入");
            }
            else
            {
                Response.Cookies.Delete("jwtToken");
                return Ok("登出成功");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("changePassword")]
        public IActionResult ChangePassword(ChangePasswordViewModel Data)
        {
            string ChangePasswordStr = _membersSerivce.ChangePassword(Data.Account, Data.Password, Data.NewPassword);
            return Ok(ChangePasswordStr);
        }


        [AllowAnonymous]
        [HttpPost("forgetPasswordMail")]
        public IActionResult ForgetPasswordMail(ForgetPasswordViewModel Data)
        {
            Members memberForget = _membersSerivce.GetDataByAccount(Data.Account);
            string isAccount = _membersSerivce.ForgetPasswordAccount(Data.Account);
            if (string.IsNullOrWhiteSpace(isAccount))
            {
                string password = _membersSerivce.ForgetPassword();
                _membersSerivce.ForgetPassword(Data.Account, password);
                string filePath = @"Views/NewPassword.html";
                string TempString = System.IO.File.ReadAllText(filePath);
                string MailBody = _mailService.ForgetMailBody(TempString, memberForget.name, password);
                _mailService.ForgetSendRegisterMail(MailBody, memberForget.email);
                return Ok("忘記密碼解決");
            }
            else
            {
                return BadRequest("忘記密碼解決失敗");
            }
        }

        //查詢基本檔案
        [AllowAnonymous]
        [HttpGet("profile/{account}")]
        public IActionResult Profile(string? account)
        {
            Members Data = _membersSerivce.GetDataByAccount(account);
            if (Data == null)
            {
                return BadRequest("查無此人");
            }
            return Ok(Data);
        }

        #region 管理者抓帳號清單
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [HttpGet("memberList")]
        public IActionResult GetMemberList()
        {
            List<MemberListViewModel> Data = new List<MemberListViewModel>();
            Data = _membersSerivce.GetDataList();
            return Ok(Data);
        }
        #endregion


        #region 編輯資料
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("EditProfile")]
        public async Task<IActionResult> EditProfileAsync([FromForm] EditMembersViewModel UpdateData)
        {
            if (User.Identity.IsAuthenticated)
            {
                Members Data = _membersSerivce.GetDataByAccount(User.Identity.Name);
                Data.name = UpdateData.name;
                if (UpdateData.img_upload != null && UpdateData.img_upload.Length > 0)
                {
                    if (!string.IsNullOrEmpty(Data.img))
                    {
                        var imagePath = Path.Combine("MembersImg", Data.img);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(UpdateData.img_upload.FileName);
                    var path = Path.Combine("MembersImg", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await UpdateData.img_upload.CopyToAsync(stream);
                    }
                    //registerMember.newMember.img = path;
                    Data.img = fileName;
                }
                else
                {
                    return BadRequest(new { message = "請上傳照片" });
                }
                Data.phone = UpdateData.phone;
                _membersSerivce.UpdatePro(Data);
                return Ok("修改成功");
            }
            else
            {
                return Ok("請去登入");
            }

        }
        #endregion

        #region 信用分數
        public IActionResult Score()
        {
            return Ok();
        }
        #endregion
    }
}
