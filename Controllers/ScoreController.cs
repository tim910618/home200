using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api1.Models;
using api1.Service;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ScoreController : ControllerBase
{
    private readonly ScoreService _scoreService;
    private readonly MembersDBService _membersSerivce;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ScoreController(ScoreService scoreService,MembersDBService membersSerivce, IWebHostEnvironment env, IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
    {
        _scoreService = scoreService;
        _membersSerivce = membersSerivce;
        _env = env;
        _configuration = configuration;
        _httpContextAccessor=httpContextAccessor;
    }

    //房客給房東分數

    //給分數
    [HttpPut("GetScore")]
    public IActionResult GetScore([FromQuery]string Account,double Point)
    {
        //先抓別人的Account
        Members Data=_membersSerivce.GetDataByAccount(Account);
        //Service計算
        _scoreService.CalculateScore(Data,Point);
        return Ok("感謝你的評分");
    }

}