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
    public class ReportController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MembersDBService _membersSerivce;
        private readonly JwtService _jwtService;
        private readonly MailService _mailService;
        private readonly IWebHostEnvironment _env;


        public ReportController(IConfiguration configuration, MembersDBService membersDBService, JwtService jwtService, MailService mailService, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _membersSerivce = membersDBService;
            _jwtService = jwtService;
            _mailService = mailService;
            _env = env;

        }

        #region 新增檢舉
        public IActionResult Insert()
        {
            //
            return Ok("檢舉成功");
        }
        #endregion

        #region 停權帳號
        //如果大於5就停權一次
        #endregion

        
        #region 檢舉列表
        public IActionResult ReportList()
        {
            if(User.IsInRole("admin"))
            {
                //抓檢舉列表
            }else
            {
                //抓屬於自己的檢舉列表
            }
            return Ok();
        }
        #endregion
    }
}
