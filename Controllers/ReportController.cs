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
        private readonly ReportDBService _reportService;
        private readonly IWebHostEnvironment _env;


        public ReportController(IConfiguration configuration, MembersDBService membersDBService, ReportDBService reportDBService, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _membersSerivce = membersDBService;
            _env = env;
            _reportService=reportDBService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        #region 新增檢舉
        [HttpPost("AddReport")]
        public IActionResult CreateReport([FromBody]Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("請確認資料");
            }
            report.reporter=User.Identity.Name;
            _reportService.AddReport(report);
            return Ok("檢舉成功");
        }

        #endregion

        #region 停權帳號
        //管理員審核(如果大於5就停權一次)
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult BlockAccount()
        {
            
            return Ok("已停權");
        }
        #endregion


        #region 檢舉列表
        public IActionResult ReportList()
        {
            if (User.IsInRole("admin"))
            {
                //抓檢舉列表
            }
            else
            {
                //抓屬於自己的檢舉列表
            }
            return Ok();
        }
        #endregion
    }
}
