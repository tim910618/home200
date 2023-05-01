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
        private readonly MailService _mailService;
        private readonly IWebHostEnvironment _env;


        public ReportController(IConfiguration configuration, MembersDBService membersDBService, ReportDBService reportDBService, IWebHostEnvironment env, MailService mailService)
        {
            _configuration = configuration;
            _membersSerivce = membersDBService;
            _env = env;
            _reportService = reportDBService;
            _mailService = mailService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        #region 新增檢舉
        [HttpPost("AddReport")]
        public IActionResult CreateReport([FromBody] Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("請確認資料");
            }
            report.reporter = User.Identity.Name;
            _reportService.AddReport(report);
            return Ok("檢舉成功");
        }

        #endregion

        #region 停權帳號(停權/取消停權)s
        [HttpPost("isBlock")]
        //管理員審核
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult BlockAccount([FromBody] BlockAccount Data)
        {
            if (Data.isblock == false)
            {
                _reportService.BlockCancelBooked(Data.reported);
                string filePath = @"Views/BlockMail.html";
                string TempString = System.IO.File.ReadAllText(filePath);
                string MailBody = _mailService.BlockMailBody(TempString, Data.reported);
                Members members=_membersSerivce.GetDataByAccount(Data.reported);
                _mailService.SentBlockMailBody(MailBody, members.email);
            }
            string Validate = _reportService.isBlockAccount(Data.reported, Data.isblock);
            return Ok(Validate);
        }
        #endregion


        #region 檢舉紀錄 //抓屬於自己的檢舉列表、會員
        [HttpGet("ReportRecord")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ReportRecord([FromQuery] string Account)
        {
            List<Report> DataList = new List<Report>();
            DataList = _reportService.GetRecord(Account);
            //傳入角色
            //讀取檢舉清單ReportList(Account)
            return Ok(DataList);
        }
        #endregion
    }
}
