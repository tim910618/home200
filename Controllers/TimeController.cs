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

[ApiController]
[Route("api/[controller]")]
public class TimeController : ControllerBase
{
    private readonly TimeDBService _timeService;
    private readonly MembersDBService _MemberService;

    public TimeController(TimeDBService timeDBService,MembersDBService membersDBService)
    {
        _timeService = timeDBService;
        _MemberService=membersDBService;
    }

    #region 房東設定可預約時間
    [HttpPost("AddBookTime")]
    public IActionResult AddBookTime([FromBody] BookTime Data)
    {
        Data.publisher = "admin";
        // bookTime.publisher=User.Identity.Name;
        // if(User.Identity.Name==null){
        //     return BadRequest("請去登入");
        // }
        _timeService.AddBookTime(Data);
        return Ok("新增時間成功");
    }
    #endregion

    #region 房東取得可預約時間表
    [HttpGet("BookTime")]
    public IActionResult BookTimeData()
    {
        string account = "admin";
        // bookTime.publisher=User.Identity.Name;
        // if(User.Identity.Name==null){
        //     return BadRequest("請去登入");
        // }
        BookTime Data = _timeService.GetBookTime(account);
        return Ok(Data);
    }
    #endregion

    #region 取得單天的預約時間
    [HttpGet("BookOfDay/{date}")]
    public IActionResult BookOfDay(DateTime Date)
    {
        //先取得Rental的屋主是誰帶入Account
        string account = "admin";
        BookTime bookTime = _timeService.GetBookOfDay(account);
        // 取得當日是星期幾
        DayOfWeek dayOfWeek = Date.DayOfWeek;

        // 根據星期幾取得可預約時間的字串
        string availableTimes = "";
        switch (dayOfWeek)
        {
            case DayOfWeek.Monday:
                availableTimes = bookTime.mon;
                break;
            case DayOfWeek.Tuesday:
                availableTimes = bookTime.tue;
                break;
            case DayOfWeek.Wednesday:
                availableTimes = bookTime.wed;
                break;
            case DayOfWeek.Thursday:
                availableTimes = bookTime.thu;
                break;
            case DayOfWeek.Friday:
                availableTimes = bookTime.fri;
                break;
            case DayOfWeek.Saturday:
                availableTimes = bookTime.sat;
                break;
            case DayOfWeek.Sunday:
                availableTimes = bookTime.sun;
                break;
            default:
                throw new ArgumentException("無效的日期天數");
        }
        // 將可預約時間字串轉成陣列並回傳
        string[] times = availableTimes.Split(';');
        return Ok(times);
    }
    #endregion
}
